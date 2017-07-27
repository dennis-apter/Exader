using System;
using System.Collections.Generic;

namespace Exader
{
    public interface IOrderable
    {
        IComparable Order { get; }
    }

    public class OrderableComparer : IComparer<IOrderable>
    {
        private readonly int nullsFactor;
        private readonly int orderFactor;

        public int Compare(IOrderable x, IOrderable y)
        {
            if (x != null && y != null)
            {
                if (x.Order == null)
                {
                    if (y.Order == null) return 0;

                    return -1 * orderFactor * y.Order.CompareTo(x.Order);
                }

                return orderFactor * x.Order.CompareTo(y.Order);
            }

            if (x != y)
            {
                return nullsFactor * (x == null ? 1 : -1);
            }

            return 0;
        }

        private OrderableComparer(bool nullsAtLast, bool descending = false)
        {
            nullsFactor = nullsAtLast ? 1 : -1;
            orderFactor = descending ? -1 : 1;
        }

        public static readonly OrderableComparer NullsAtFirst = new OrderableComparer(false);
        public static readonly OrderableComparer NullsAtLast = new OrderableComparer(true);
        public static readonly OrderableComparer NullsAtFirstDescending = new OrderableComparer(false, true);
        public static readonly OrderableComparer NullsAtLastDescending = new OrderableComparer(true, true);
    }
}
