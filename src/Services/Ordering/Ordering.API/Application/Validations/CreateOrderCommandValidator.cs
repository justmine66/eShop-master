using FluentValidation;
using Ordering.API.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Ordering.API.Application.Commands.CreateOrderCommand;

namespace Ordering.API.Application.Validations
{
    //
    //摘要:
    //    创建订单命令验证器
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            //送货地址验证
            RuleFor(command => command.City).NotEmpty();
            RuleFor(command => command.Street).NotEmpty();
            RuleFor(command => command.State).NotEmpty();
            RuleFor(command => command.Country).NotEmpty();
            RuleFor(command => command.ZipCode).NotEmpty();
            //银行卡验证
            RuleFor(command => command.CardNumber).NotEmpty().Length(12, 19);
            RuleFor(command => command.CardHolderName).NotEmpty();
            RuleFor(command => command.CardExpiration).NotEmpty().Must(BeValidExpirationDate).WithMessage("银行卡已过期。");
            RuleFor(command => command.CardSecurityNumber).NotEmpty().Length(3);
            RuleFor(command => command.CardTypeId).NotEmpty();
            //订单项验证
            RuleFor(command => command.OrderItems).Must(ContainOrderItems).WithMessage("订单项列表为空");
        }

        //是否在有限期内
        private bool BeValidExpirationDate(DateTime dateTime)
        {
            return dateTime >= DateTime.UtcNow;
        }
        //必须包含订单项列表
        private bool ContainOrderItems(IEnumerable<OrderItemDTO> orderItems)
        {
            return orderItems.Any();
        }
    }
}
