using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq.Expressions
{
    /// <summary>
    /// A helper class to get fields.
    /// </summary>
    public static class FieldExpressions
    {
        /// <summary>
        /// Gets the <see cref="FieldInfo"/> of the specified field.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="predicate">An expression which returns a field.</param>
        /// <returns>A <see cref="FieldInfo"/> representing the selected field.</returns>
        public static FieldInfo GetField<TField>(Expression<Func<TField>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");


            MemberExpression memberExpression = predicate.Body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    predicate));

            FieldInfo fieldInfo = memberExpression.Member as FieldInfo;

            if (fieldInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property not a field.",
                    predicate));

            return fieldInfo;
        }
    }
}
