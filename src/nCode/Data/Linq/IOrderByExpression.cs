using System;
using System.Linq;

namespace nCode.Data.Linq
{
    public interface IOrderByExpression<T>
    {
        IQueryable<T> ApplyOrdering(System.Linq.IQueryable<T> query);
    }
}
