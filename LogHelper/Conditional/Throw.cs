using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// A helper class to check constraints
    /// </summary>
    public static class Throw
    {
        /// <summary>
        /// Throws an exception when the field in question is null.
        /// </summary>
        /// <typeparam name="T">The runtime type of the field.</typeparam>
        /// <param name="predicate">An expression of type <see cref="Func{T}"/> which returns a field.</param>
        /// <exception cref="ArgumentNullException">Thrown when the field is null.</exception>
        public static void IfNull<T>(Expression<Func<T>> predicate)
        {
            FieldInfo fieldInfo = FieldExpressions.GetField(predicate);


            object value = fieldInfo
                .GetValue(((ConstantExpression)((MemberExpression)predicate.Body).Expression).Value);

            if (value == null)
                throw new ArgumentNullException(fieldInfo.Name);
        }


        /// <summary>
        /// Throws an exception when the collection based field in question is null or empty.
        /// </summary>
        /// <typeparam name="T">The runtime type of the field.</typeparam>
        /// <param name="predicate">An expression of type <see cref="Func{T}" /> which returns a field./></param>
        // TODO: document the exceptions thrown
        public static void IfNullOrEmpty<T>(Expression<Func<T>> predicate)
        {
            FieldInfo fieldInfo = FieldExpressions.GetField(predicate);

            Do.If(!typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType), () => {
                throw new InvalidOperationException(string.Format("Field {0} is not an IEnumerable.",
                    fieldInfo.Name));
            });


            object value = fieldInfo
                .GetValue(((ConstantExpression)((MemberExpression)predicate.Body).Expression).Value);

            if (value == null)
                throw new ArgumentNullException(fieldInfo.Name);

            IEnumerable enumerable = (IEnumerable)value;

            IEnumerator enumerator = enumerable.GetEnumerator();

            enumerator.Reset();

            int i = 0;

            while (enumerator.MoveNext() && i == 0)
                i++;

            if (i == 0)
                throw new InvalidOperationException(string.Format("Field {0} should not be empty.", fieldInfo.Name));
        }
    }
}
