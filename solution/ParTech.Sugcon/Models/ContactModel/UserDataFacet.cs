namespace ParTech.Sugcon.Models.ContactModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ParTech.Sugcon.Extensions;
    using ParTech.Sugcon.Utilities;
    using Sitecore.Analytics.Model.Framework;

    [Serializable]
    public class UserDataFacet : Facet, IUserDataFacet
    {
        public UserDataFacet()
        {
            this.EnsureAttribute<string>("Profession");
            this.EnsureElement<ISocialElement>("Social");
            this.EnsureCollection<IBookElement>("Books");
        }

        public string Profession
        {
            get { return this.GetAttribute<string>("Profession"); }
            set { this.SetAttribute<string>("Profession", value); }
        }

        public ISocialElement Social
        {
            get { return this.GetElement<ISocialElement>("Social"); }
            set { ObjectUtil.CopyObject<ISocialElement>(value, this.Social); }
        }

        public List<IBookElement> Books
        {
            get { return this.GetCollection<IBookElement>("Books").ToList<IBookElement>(); }
            set { this.GetCollection<IBookElement>("Books").UpdateCollection(value.ToList()); }
        }
    }
}