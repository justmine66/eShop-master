using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class CardType
        : Enumeration
    {
        public static CardType unionPlay = new CardType(1, "unionPlay");
        public static CardType aPlay = new CardType(2, "APlay");
        public static CardType weChat = new CardType(3, "WeChat");

        protected CardType() { }
        public CardType(int id, string name)
            : base(id, name)
        { }

        public static IEnumerable<CardType> List() => new[] { unionPlay, aPlay, weChat };

        public static CardType FromName(string name)
        {
            var state = List().SingleOrDefault(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for CardTypes{string.Join(",", List())}");
            }

            return state;
        }
        public static CardType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Possible values for CardType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
