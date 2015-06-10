namespace ParTech.Sugcon.Models.ContactModel
{
    using System;
    using Sitecore.Analytics.Model.Framework;

    [Serializable]
    public class BookElement : Element, IBookElement
    {
        public BookElement()
        {
            this.EnsureAttribute<string>("Title");
            this.EnsureAttribute<string>("Isbn");
            this.EnsureAttribute<bool>("Available");
        }

        public string Title
        {
            get { return this.GetAttribute<string>("Title"); }
            set { this.SetAttribute<string>("Title", value); }
        }

        public string Isbn
        {
            get { return this.GetAttribute<string>("Isbn"); }
            set { this.SetAttribute<string>("Isbn", value); }
        }

        public bool Available
        {
            get { return this.GetAttribute<bool>("Available"); }
            set { this.SetAttribute<bool>("Available", value); }
        }
    }
}