namespace ParTech.Sugcon.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using ParTech.Sugcon.Utilities;
    using Sitecore.Analytics.Model.Framework;

    /// <summary>
    /// Extension methods for the IElementCollection interface implementations.
    /// </summary>
    public static class ElementCollectionExtensions
    {
        /// <summary>
        /// Updated the collection by adding or replacing with new values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="removeExistingEntries">if set to <c>true</c> [remove existing entries].</param>
        public static void UpdateCollection<T>(this IElementCollection<T> collection, IEnumerable<T> replacement, bool removeExistingEntries = true) 
            where T : class, IElement
        {
            // Remove the existing entries.
            if (removeExistingEntries)
            {
                Enumerable.Range(0, collection.Count)
                    .ForEach(i => collection.Remove(0));
            }

            // Add the replacement entries.
            if (replacement != null)
            {
                replacement.ForEach(r => 
                {
                    T entry = collection.Create();
                    ObjectUtil.CopyObject<T>(r, entry);
                });
            }
        }
    }
}