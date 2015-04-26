using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Data.Linq
{
    public class ContextualFilterExpression<C, T> : IFilterExpression<C, T>
    {
        private Func<C, Expression<Func<T, bool>>> exp = null;

        public ContextualFilterExpression(Func<C, Expression<Func<T, bool>>> expression)
        {
            exp = expression;
        }

        public IQueryable<T> ApplyFilter(C context, IQueryable<T> query)
        {
            return query.Where(exp(context));
        }
    }
}
