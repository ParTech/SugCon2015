namespace ParTech.Sugcon.Models.ContactModel
{
    using Sitecore.Analytics.Model.Framework;

    public interface ISocialElement : IElement
    {
        string Twitter { get; set; }

        string YouTube { get; set; }

        string Google { get; set; }
    }
}