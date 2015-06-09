namespace ParTech.Sugcon.Models.ContactModel
{
    using Sitecore.Analytics.Model.Framework;

    public interface IBookElement : IElement
    {
        string Title { get; set; }

        string Isbn { get; set; }

        bool Available { get; set; }
    }
}