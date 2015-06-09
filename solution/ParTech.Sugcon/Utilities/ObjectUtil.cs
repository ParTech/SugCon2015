namespace ParTech.Sugcon.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ObjectUtil
    {
        /// <summary>
        /// Copies the property values of an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public static void CopyObject<T>(T source, T target)
            where T : class
        {
            if (source == null)
            {
                return;
            }

            foreach (var property in GetPublicProperties(typeof(T)))
            {
                if (property.CanRead && property.CanWrite)
                {
                    property.SetValue(target, property.GetValue(source));
                }
            }
        }

        /// <summary>
        /// Copies the property values of an object to another object of a different type (but with the same properties).
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public static void CopyObject<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
        {
            if (source == null)
            {
                return;
            }

            foreach (var sourceProperty in GetPublicProperties(typeof(TSource)))
            {
                if (sourceProperty.CanRead)
                {
                    // Find matching property on TTarget
                    var targetProperty = GetPublicProperties(typeof(TTarget))
                        .FirstOrDefault(x => x.Name == sourceProperty.Name);

                    if (targetProperty != null && targetProperty.CanWrite)
                    {
                        targetProperty.SetValue(target, sourceProperty.GetValue(source));
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the public (inherited) properties from a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();
                var considered = new List<Type>();
                var queue = new Queue<Type>();

                considered.Add(type);
                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();

                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface))
                        {
                            continue;
                        }

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}