namespace ParTech.Sugcon.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// Extension methods for IEnumerable classes.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Executes the <paramref name="action"/> for each element
        /// </summary>
        /// <typeparam name="T">Type of the objects in the collection</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="action">The action.</param>
        /// <returns>Returns a IEnumerable{``0}</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }

            return source;
        }
    }
}