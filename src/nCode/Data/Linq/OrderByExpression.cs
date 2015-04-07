using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Data.Linq
{
    public class OrderByExpression<T, U> : IOrderByExpression<T>
    {
        public OrderByExpression(Expression<Func<T, U>> myExpression)
        {

            exp = myExpression;

        }

        public IQueryable<T> ApplyOrdering(IQueryable<T> query)
        {
            return query.OrderBy(exp);
        }

        private Expression<Func<T, U>> exp = null;
    }
}
