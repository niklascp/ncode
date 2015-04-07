using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Data.Linq
{
    public class FilterExpression<C, T> : IFilterExpression<C, T>
    {
        private Expression<Func<T, bool>> exp = null;

        public FilterExpression(Expression<Func<T, bool>> expression)
        {
            exp = expression;
        }

        public IQueryable<T> ApplyFilter(C context, IQueryable<T> query)
        {
            return query.Where(exp);
        }

        public static implicit operator FilterExpression<C, T>(Expression<Func<T, bool>> expression)
        {
            return new FilterExpression<C, T>(expression);
        }
    }
}
