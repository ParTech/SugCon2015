namespace ParTech.Sugcon.Models.ContactModel
{
    using System.Collections.Generic;
    using Sitecore.Analytics.Model.Framework;

    public interface IUserDataFacet : IFacet
    {
        string Profession { get; set; }

        ISocialElement Social { get; set; }

        List<IBookElement> Books { get; set; }
    }
}