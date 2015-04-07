using System;
using System.Linq;

namespace nCode.Data.Linq
{
    public interface IFilterExpression<C, T>
    {
        IQueryable<T> ApplyFilter(C context, IQueryable<T> query);
    }
}
