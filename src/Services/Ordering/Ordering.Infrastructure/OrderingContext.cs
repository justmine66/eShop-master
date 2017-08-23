using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.SeedWork;
using Ordering.Infrastructure.Idempotency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure
{
    /// <summary>
    /// 订单上下文
    /// </summary>
    public class OrderingContext
        : DbContext, IUnitOfWork
    {
        const string DEFAULT_SCHEMA = "ordering";//数据库默认架构

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<PaymentMethod> Payments { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

        private readonly IMediator _mediator;

        public OrderingContext(DbContextOptions options, IMediator mediator)
            : base(options)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientRequest>(ConfigureRequests);
            modelBuilder.Entity<Address>(ConfigureAddress);
            modelBuilder.Entity<PaymentMethod>(ConfigurePayment);
            modelBuilder.Entity<Order>(ConfigureOrder);
            modelBuilder.Entity<OrderItem>(ConfigureOrderItems);
            modelBuilder.Entity<CardType>(ConfigureCardTypes);
            modelBuilder.Entity<OrderStatus>(ConfigureOrderStatus);
            modelBuilder.Entity<Buyer>(ConfigureBuyer);
        }

        //客户端请求表配置
        void ConfigureRequests(EntityTypeBuilder<ClientRequest> requestConfiguration)
        {
            requestConfiguration.ToTable("requests", DEFAULT_SCHEMA);
            requestConfiguration.HasKey(cr => cr.Id);
            requestConfiguration.Property(cr => cr.Name).IsRequired();
            requestConfiguration.Property(cr => cr.Time).IsRequired();
        }

        //地址表配置
        void ConfigureAddress(EntityTypeBuilder<Address> addressConfiguration)
        {
            addressConfiguration.ToTable("address", DEFAULT_SCHEMA);
            addressConfiguration.Property("Id").IsRequired();//阴影属性
            addressConfiguration.HasKey("Id");
        }

        //买家表配置
        void ConfigureBuyer(EntityTypeBuilder<Buyer> buyerConfiguration)
        {
            buyerConfiguration.ToTable("buyers", DEFAULT_SCHEMA);

            buyerConfiguration.HasKey(b => b.Id);
            buyerConfiguration.Property(b => b.Id).ForSqlServerUseSequenceHiLo("buyerseq", DEFAULT_SCHEMA);

            buyerConfiguration.Ignore(b => b.DomainEvents);

            buyerConfiguration.Property(b => b.IdentityGuid)
                .HasMaxLength(200)
                .IsRequired();

            buyerConfiguration.HasIndex("IdentityGuid").IsUnique(true);

            buyerConfiguration.HasMany(b => b.PaymentMethods)
                .WithOne()
                .HasForeignKey("BuyerId")
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = buyerConfiguration.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        //支付方式表配置
        void ConfigurePayment(EntityTypeBuilder<PaymentMethod> paymentConfiguration)
        {
            paymentConfiguration.ToTable("paymentmethods", DEFAULT_SCHEMA);

            paymentConfiguration.HasKey(b => b.Id);

            paymentConfiguration.Ignore(b => b.DomainEvents);

            paymentConfiguration.Property(b => b.Id)
                .ForSqlServerUseSequenceHiLo("paymentseq", DEFAULT_SCHEMA);

            paymentConfiguration.Property("BuyerId").IsRequired();

            paymentConfiguration.Property<string>("CardHolderName")
                .HasMaxLength(200)
                .IsRequired();

            paymentConfiguration.Property<string>("Alias")
                .HasMaxLength(200)
                .IsRequired();

            paymentConfiguration.Property<string>("CardNumber")
                .HasMaxLength(25)
                .IsRequired();

            paymentConfiguration.Property<DateTime>("Expiration")
                .IsRequired();

            paymentConfiguration.Property<int>("CardTypeId")
                .IsRequired();

            paymentConfiguration.HasOne(p => p.CardType)
                .WithMany()
                .HasForeignKey("CardTypeId");
        }

        //银行卡类型表配置
        void ConfigureCardTypes(EntityTypeBuilder<CardType> cardTypesConfiguration)
        {
            cardTypesConfiguration.ToTable("cardtypes", DEFAULT_SCHEMA);

            cardTypesConfiguration.HasKey(ct => ct.Id);

            cardTypesConfiguration.Property(ct => ct.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            cardTypesConfiguration.Property(ct => ct.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        //订单表配置
        void ConfigureOrder(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", DEFAULT_SCHEMA);

            orderConfiguration.HasKey(o => o.Id);

            orderConfiguration.Ignore(b => b.DomainEvents);

            orderConfiguration.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("orderseq", DEFAULT_SCHEMA);

            orderConfiguration.Property<DateTime>("OrderDate").IsRequired();
            orderConfiguration.Property<int?>("BuyerId").IsRequired(false);
            orderConfiguration.Property<int>("OrderStatusId").IsRequired();
            orderConfiguration.Property<int?>("PaymentMethodId").IsRequired(false);
            orderConfiguration.Property<string>("Description").IsRequired(false);

            var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            orderConfiguration.HasOne<PaymentMethod>()
                .WithMany()
                .HasForeignKey("PaymentMethodId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            orderConfiguration.HasOne<Buyer>()
                .WithMany()
                .HasForeignKey("BuyerId")
                .IsRequired(false);

            orderConfiguration.HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey("OrderStatusId");
        }

        //订单项表配置
        void ConfigureOrderItems(EntityTypeBuilder<OrderItem> orderItemConfiguration)
        {
            orderItemConfiguration.ToTable("orderItems", DEFAULT_SCHEMA);

            orderItemConfiguration.HasKey(o => o.Id);

            orderItemConfiguration.Ignore(b => b.DomainEvents);

            orderItemConfiguration.Property(o => o.Id)
                .ForSqlServerUseSequenceHiLo("orderitemseq");

            orderItemConfiguration.Property<int>("OrderId")
                .IsRequired();

            orderItemConfiguration.Property<decimal>("Discount")
                .IsRequired();

            orderItemConfiguration.Property<int>("ProductId")
                .IsRequired();

            orderItemConfiguration.Property<string>("ProductName")
                .IsRequired();

            orderItemConfiguration.Property<decimal>("UnitPrice")
                .IsRequired();

            orderItemConfiguration.Property<int>("Units")
                .IsRequired();

            orderItemConfiguration.Property<string>("PictureUrl")
                .IsRequired(false);
        }

        //订单状态表配置
        void ConfigureOrderStatus(EntityTypeBuilder<OrderStatus> orderStatusConfiguration)
        {
            orderStatusConfiguration.ToTable("orderstatus", DEFAULT_SCHEMA);

            orderStatusConfiguration.HasKey(o => o.Id);

            orderStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            orderStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        /// <summary>
        /// 保存所有实体
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>是否保存成功</returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await this._mediator.DispatchDomainEventAsync(this);

            var result = await base.SaveChangesAsync();

            return true;
        }

    }
}
