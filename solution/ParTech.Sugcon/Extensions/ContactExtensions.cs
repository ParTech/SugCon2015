namespace ParTech.Sugcon.Extensions
{
    using System.Linq;
    using Sitecore.Analytics.Model.Framework;
    using Sitecore.Analytics.Tracking;

    public static class ContactExtensions
    {
        /// <summary>
        /// Gets the first facet of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contact"></param>
        /// <returns>Facet of type <typeparamref name="T" /> or null.</returns>
        public static T GetFacet<T>(this Contact contact)
            where T : class, IFacet
        {
            return (T)contact.Facets.Values.FirstOrDefault(x => x is T);
        }
    }
}