using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// A helper class to get a property.
    /// </summary>
    public static class PropertyExpressions
    {
        /// <summary>
        /// Gets the property of a specified object.
        /// </summary>
        /// <typeparam name="TProperty">the runtime type of the property.</typeparam>
        /// <param name="source">the object to which the selected property belongs.</param>
        /// <param name="predicate">An expression which returns a property.</param>
        /// <returns>A <see cref="PropertyInfo"/> representing the selected property.</returns>
        public static PropertyInfo GetProperty<TProperty>(object source, Expression<Func<TProperty>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            Type type = source.GetType();

            MemberExpression memberExpression = predicate.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    predicate.ToString()));

            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    predicate.ToString()));

            if (type != propertyInfo.ReflectedType &&
                !type.IsSubclassOf(propertyInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    predicate.ToString(),
                    type));

            return propertyInfo;
        }

        /// <summary>
        /// Gets the property name of a specified property.
        /// </summary>
        /// <typeparam name="TProperty">The runtime type of the property.</typeparam>
        /// <param name="source">The object to which the selected property belongs.</param>
        /// <param name="predicate">An expression which returns a property.</param>
        /// <returns>The name of the property.</returns>
        /// <exception cref="ArgumentNullException">When source or predicate is null.</exception>
        /// <exception cref="ArgumentException">When predicate returns any type other than a property.</exception>
        public static string GetPropertyName<TProperty>(object source, Expression<Func<TProperty>> predicate)
        {
            return GetProperty(source, predicate).Name;
        }

    }
}
