namespace ParTech.Sugcon.Extensions
{
    using Sitecore.Analytics.Model.Framework;

    /// <summary>
    /// Extensions methods for the IElementDictionary classes.
    /// </summary>
    public static class ElementDictionaryExtensions
    {
        /// <summary>
        /// Gets or creates the element with specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T GetOrCreate<T>(this IElementDictionary<T> dictionary, string key = "default")
            where T : class, IElement
        {
            return dictionary.Contains(key)
                ? dictionary[key]
                : dictionary.Create(key);
        }
    }
}