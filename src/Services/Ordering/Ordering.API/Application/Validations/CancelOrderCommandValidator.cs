using FluentValidation;
using Ordering.API.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Validations
{
    //
    //摘要:
    //    取消订单命令验证器
    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("订单号为空");
        }
    }
}
