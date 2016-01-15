using System;
using System.Linq.Expressions;

namespace spectator.Infrastructure
{
    public static class ObjectExtensions
    {
        public static void MustNotBeNull(this object obj, Expression<Func<object>> objectExpression)
        {
            if (obj == null)
            {
                var memberExpression = objectExpression.Body as MemberExpression;
                var objectName = memberExpression.Member.Name;

                throw new ArgumentNullException(objectName);
            }
        }

        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }
    }
}