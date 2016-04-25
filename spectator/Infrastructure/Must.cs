using System;
using System.Linq.Expressions;

namespace spectator.Infrastructure
{
    public class Must
    {
        public static void NotBeNull(Expression<Func<object>> objectExpression)
        {
            var obj = objectExpression.Compile()();

            if (obj != null)
            {
                return;
            }

            var memberExpression = objectExpression.Body as MemberExpression;
            var objectName = memberExpression.Member.Name;

            throw new ArgumentNullException(objectName);
        }
    }
}
