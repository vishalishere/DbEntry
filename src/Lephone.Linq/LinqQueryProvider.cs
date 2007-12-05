﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Lephone.Data;

namespace Lephone.Linq
{
    public class LinqQueryProvider<T, TKey> : IOrderedQueryable<T>, IQueryProvider where T : LinqObjectModel<T, TKey>
    {
        private Expression expression;

        public LinqQueryProvider(Expression expression)
        {
            this.expression = expression;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            MethodCallExpression mce = (MethodCallExpression)this.expression;
            UnaryExpression ue = (UnaryExpression)mce.Arguments[1];
            Expression<Func<T, bool>> condition = (Expression<Func<T, bool>>)ue.Operand;
            var list = DbEntry.From<T>().Where(condition).Select();
            return ((IEnumerable<T>)list).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get
            {
                return expression ?? Expression.Constant(this);
            }
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        #endregion

        #region IQueryProvider Members

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new LinqQueryProvider<T, TKey>(expression) as IQueryable<TElement>;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new LinqQueryProvider<T, TKey>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}