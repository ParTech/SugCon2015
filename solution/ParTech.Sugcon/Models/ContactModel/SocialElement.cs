namespace ParTech.Sugcon.Models.ContactModel
{
    using System;
    using Sitecore.Analytics.Model.Framework;

    [Serializable]
    public class SocialElement : Element, ISocialElement
    {
        public SocialElement()
        {
            this.EnsureAttribute<string>("Twitter");
            this.EnsureAttribute<string>("YouTube");
            this.EnsureAttribute<string>("Google");
        }

        public string Twitter
        {
            get { return this.GetAttribute<string>("Twitter"); }
            set { this.SetAttribute<string>("Twitter", value); }
        }

        public string YouTube
        {
            get { return this.GetAttribute<string>("YouTube"); }
            set { this.SetAttribute<string>("YouTube", value); }
        }

        public string Google
        {
            get { return this.GetAttribute<string>("Google"); }
            set { this.SetAttribute<string>("Google", value); }
        }
    }
}
