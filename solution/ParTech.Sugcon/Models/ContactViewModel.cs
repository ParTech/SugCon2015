namespace ParTech.Sugcon.Models
{
    using System;
    using ParTech.Sugcon.Extensions;
    using Sitecore.Analytics;
    using Sitecore.Analytics.Model;
    using Sitecore.Analytics.Model.Entities;
    using Sitecore.Analytics.Tracking;

    public class ContactViewModel
    {
        public ContactViewModel()
            : this(null)
        {
        }

        public ContactViewModel(Contact contact)
        {
            this.Initialize(contact);
        }

        public Guid ContactId { get; set; }

        public ContactIdentificationLevel IdentificationLevel { get; set; }

        public string Identifier { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        private void Initialize(Contact contact)
        {
            if (contact == null)
            {
                return;
            }

            this.ContactId = contact.ContactId;
            this.IdentificationLevel = contact.Identifiers.IdentificationLevel;
            this.Identifier = contact.Identifiers.Identifier;

            this.InitializeFacets(contact);
        }

        private void InitializeFacets(Contact contact)
        {
            var personal = contact.GetFacet<IContactPersonalInfo>();    // Extension method.
            this.FirstName = personal.FirstName;
            this.LastName = personal.Surname;

            var addresses = contact.GetFacet<IContactAddresses>();
            var defaultAddress = addresses.Entries.GetOrCreate();
            this.City = defaultAddress.City;

            var emails = contact.GetFacet<IContactEmailAddresses>();
            var defaultEmail = emails.Entries.GetOrCreate();
            this.Email = defaultEmail.SmtpAddress;
        }
    }
}