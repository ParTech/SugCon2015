namespace ParTech.Sugcon.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Web.Mvc;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using ParTech.Sugcon.Extensions;
    using ParTech.Sugcon.Models;
    using ParTech.Sugcon.Models.ContactModel;
    using Sitecore.Analytics;
    using Sitecore.Analytics.Data;
    using Sitecore.Analytics.DataAccess;
    using Sitecore.Analytics.Model;
    using Sitecore.Analytics.Model.Entities;
    using Sitecore.Analytics.Pipelines.CommitSession;
    using Sitecore.Analytics.Processing.ProcessingPool;
    using Sitecore.Analytics.Tracking;
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;
    using MongoDbQuery = MongoDB.Driver.Builders.Query;

    public class DemoController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new ContactViewModel(Tracker.Current.Contact);

            return this.View("~/Views/Index.cshtml", viewModel);
        }

        public ActionResult ForgetContact()
        {
            // Manually remove the cookie and clear the session.
            // Sitecore has not implemented this in their API yet (rev 141003)
            string cookieName = "SC_ANALYTICS_GLOBAL_COOKIE";
            var cookie = this.Response.Cookies[cookieName];

            if (cookie != null)
            {
                cookie.Expires = DateTime.UtcNow.AddDays(-1.0);
            }

            this.Request.Cookies.Remove(cookieName);

            this.Session.Abandon();
            
            return this.RedirectToAction("Index");
        }

        public ActionResult EndSession()
        {
            Tracker.Current.Contact.ContactSaveMode = ContactSaveMode.AlwaysSave;

            this.Session.Abandon();

            return this.RedirectToAction("Index");
        }

        public ActionResult CommitSession()
        {
            Tracker.Current.Contact.ContactSaveMode = ContactSaveMode.AlwaysSave;

            CommitSessionPipeline.Run(new CommitSessionPipelineArgs
            {
                Session = Tracker.Current.Session
            });

            return this.RedirectToAction("Index");
        }

        public ActionResult UpdateContact()
        {
            var viewModel = new ContactViewModel(Tracker.Current.Contact);

            return this.View("~/Views/UpdateContact.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult UpdateContact(UpdateContactRequestModel request)
        {
            var contact = Tracker.Current.Contact;

            contact.ContactSaveMode = ContactSaveMode.AlwaysSave;

            var personal = contact.GetFacet<IContactPersonalInfo>();
            personal.FirstName = request.FirstName ?? personal.FirstName;
            personal.Surname = request.LastName ?? personal.Surname;

            var addresses = contact.GetFacet<IContactAddresses>();
            var defaultAddress = addresses.Entries.GetOrCreate();
            defaultAddress.City = request.City ?? defaultAddress.City;

            var emails = contact.GetFacet<IContactEmailAddresses>();
            var defaultEmail = emails.Entries.GetOrCreate();
            defaultEmail.SmtpAddress = request.Email ?? defaultEmail.SmtpAddress;

            this.ApplyCustomData(contact);

            return this.RedirectToAction("Index");
        }

        public ActionResult IdentifyContact()
        {
            var viewModel = new ContactViewModel(Tracker.Current.Contact);

            return this.View("~/Views/IdentifyContact.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult IdentifyContact(string identifier)
        {
            Tracker.Current.Session.Identify(identifier);

            return this.RedirectToAction("Index");
        }

        public ActionResult LoadContactFromXDb()
        {
            return this.View("~/Views/LoadContact.cshtml", new ContactViewModel());
        }

        [HttpPost]
        public ActionResult LoadContactFromXDb(Guid? contactId, string identifier)
        {
            Contact contact = null;

            if (contactId.HasValue && contactId != Guid.Empty)
            {
                contact = this.LoadContactReadOnly(contactId.Value);
            }
            else if (!string.IsNullOrEmpty(identifier))
            {
                contact = this.LoadContactReadOnly(identifier);
            }

            var viewModel = new ContactViewModel(contact);

            return this.View("~/Views/LoadContact.cshtml", viewModel);
        }

        public ActionResult SaveContactToXDb()
        {
            return this.View("~/Views/SaveContact.cshtml", new ContactViewModel());
        }

        [HttpPost]
        public ActionResult SaveContactToXDb(UpdateContactRequestModel request)
        {
            var contact = this.LoadOrCreateContact(request.Identifier);

            var personal = contact.GetFacet<IContactPersonalInfo>();
            personal.FirstName = request.FirstName ?? personal.FirstName;
            personal.Surname = request.LastName ?? personal.Surname;

            var addresses = contact.GetFacet<IContactAddresses>();
            var defaultAddress = addresses.Entries.GetOrCreate();
            defaultAddress.City = request.City ?? defaultAddress.City;

            var emails = contact.GetFacet<IContactEmailAddresses>();
            var defaultEmail = emails.Entries.GetOrCreate();
            defaultEmail.SmtpAddress = request.Email ?? defaultEmail.SmtpAddress;

            this.SaveAndReleaseContact(contact);

            return this.View("~/Views/SaveContact.cshtml", new ContactViewModel(contact));
        }

        public ActionResult SaveContactToXDbWithLocking()
        {
            return this.View("~/Views/SaveContact.cshtml", new ContactViewModel());
        }

        [HttpPost]
        public ActionResult SaveContactToXDbWithLocking(UpdateContactRequestModel request)
        {
            // Create contact or load contact readonly to get the ContactId.
            // We cannot directly TryLoadContact if we only have the identifier.
            // This will load the contact directly from xDB (not from shared session).
            // Do not start manipulating this Contact because it's meant to be readonly (although saving it will work in most cases...)
            // If you do, you will override anything that was manipulated in the session.
            var contact = this.LoadOrCreateContact(request.Identifier); 

            try
            {
                var contactManager = Factory.CreateObject("tracking/contactManager", true) as ContactManager;

                // Load the Contact from the shared session so we can manipulate it.
                // If it's not in the session yet, this will load it from xDB and store it in the session first.
                Log.Debug(string.Format("[DemoController] TryLoadContact: {0}", contact.ContactId));

                var lockAttempt = contactManager.TryLoadContact(contact.ContactId, 1);

                if (lockAttempt.Status == LockAttemptStatus.Success)
                {
                    contact = lockAttempt.Object;
                    
                    var personal = contact.GetFacet<IContactPersonalInfo>();
                    personal.FirstName = request.FirstName ?? personal.FirstName;
                    personal.Surname = request.LastName ?? personal.Surname;

                    var addresses = contact.GetFacet<IContactAddresses>();
                    var defaultAddress = addresses.Entries.GetOrCreate();
                    defaultAddress.City = request.City ?? defaultAddress.City;

                    var emails = contact.GetFacet<IContactEmailAddresses>();
                    var defaultEmail = emails.Entries.GetOrCreate();
                    defaultEmail.SmtpAddress = request.Email ?? defaultEmail.SmtpAddress;

                    this.SaveAndReleaseContact(contact);

                    return this.View("~/Views/SaveContact.cshtml", new ContactViewModel(contact));
                }
                
                throw new Exception(string.Format("Failed to get lock. Status: {0}", lockAttempt.Status));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to get lock: {0}", ex.Message));
            }
        }

        public ActionResult RebuildContacts()
        {
            int skip = 0;
            int limit = 1000;

            var sw = Stopwatch.StartNew();
            var contactIds = new List<Guid>();

            this.Response.Buffer = false;

            var processingPool = (ProcessingPool)Assert.ResultNotNull<object>(Factory.CreateObject("aggregationProcessing/processingPools/contact", true));
            var mongoQuery = MongoDbQuery.Exists("Identifiers.Identifier");

            while ((contactIds = this.QueryContacts(mongoQuery, skip, limit)).Count > 0)
            {
                if (contactIds.Count < limit)
                {
                    limit = contactIds.Count;
                }

                this.Response.Write(string.Format("Retrieved record {0} to {1}<br />", skip, skip + limit));

                contactIds.ForEach(contactId =>
                {
                    var workItem = new ProcessingPoolItem(contactId.ToByteArray());

                    workItem.Properties.Add("Reason", "Updated");

                    processingPool.Add(workItem, null);
                });

                skip += limit;
            }

            this.Response.Write(string.Format("Added {0} contacts to processing pool in {1} seconds", skip, sw.ElapsedMilliseconds / 1000));

            return this.Content(string.Empty);
        }

        private Contact LoadContactReadOnly(Guid contactId)
        {
            var contactManager = Factory.CreateObject("tracking/contactManager", true) as ContactManager;

            return contactManager.LoadContactReadOnly(contactId);

            // Can also be done with contactRepository:
            // 
            // var repository = Factory.CreateObject("contactRepository", true) as ContactRepositoryBase;
            // return repository.LoadContactReadOnly(contactId);
        }

        private Contact LoadContactReadOnly(string identifier)
        {
            var contactManager = Factory.CreateObject("tracking/contactManager", true) as ContactManager;

            return contactManager.LoadContactReadOnly(identifier);
        }

        private Contact LoadOrCreateContact(string identifier)
        {
            var contact = this.LoadContactReadOnly(identifier)
                ?? this.CreateContact(identifier);

            return contact;
        }

        private Contact CreateContact(string identifier)
        {
            var contactRepository = Factory.CreateObject("contactRepository", true) as ContactRepositoryBase;

            var contact = contactRepository.CreateContact(Guid.NewGuid());

            contact.Identifiers.IdentificationLevel = ContactIdentificationLevel.Known;
            contact.Identifiers.Identifier = identifier;
            contact.ContactSaveMode = ContactSaveMode.AlwaysSave;

            var contactManager = Factory.CreateObject("tracking/contactManager", true) as ContactManager;

            contactManager.SaveAndReleaseContactToXdb(contact);

            return contact;
        }

        private void SaveAndReleaseContact(Contact contact)
        {
            contact.ContactSaveMode = ContactSaveMode.AlwaysSave;

            var contactManager = Factory.CreateObject("tracking/contactManager", true) as ContactManager;

            // This will save the contact in the session.
            // Because we loaded it directly, it's stored in the shared session which won't be flushed until the session ends.
            // We can't end the session because it's inaccessible at this level so the change won't be saved to xDB straight away.
            contactManager.SaveAndReleaseContact(contact);

            // This sends the contact directly to the submit queue.
            // The change will be saved to xDB straight away.
            // This is not required in a production situation, but really useful during development.
            contactManager.SaveAndReleaseContactToXdb(contact);

            // You might think that just SaveAndReleaseContactToXdb it's enough to save directly to xDB.
            // But that won't update the shared session which is a major pitfall.
            // The contact will not be added to the reporting db and the analytics index, plus your data will be overridden when the session is flushed.
        }

        private List<Guid> QueryContacts(IMongoQuery query, int skip, int limit)
        {
            string connection = ConfigurationManager.ConnectionStrings["analytics"].ConnectionString;
            var url = new MongoUrl(connection);

            var server = new MongoClient(connection)
                .GetServer();

            var database = server.GetDatabase(url.DatabaseName);
            var contacts = database.GetCollection<BsonDocument>("Contacts");

            var result = contacts.Find(query)
                .SetSkip(skip)
                .SetLimit(limit);

            return result.Select(x => (Guid)x["_id"])
                .ToList();
        }

        private void ApplyCustomData(Contact contact)
        {
            /*
            var personal = contact.GetFacet<IContactPersonalInfo>();
            personal.FirstName = "Ruud";
            personal.Surname = "van Falier";

            var addresses = contact.GetFacet<IContactAddresses>();
            var defaultAddress = addresses.Entries.GetOrCreate();
            defaultAddress.City = "'s-Hertogenbosch";

            var emails = contact.GetFacet<IContactEmailAddresses>();
            var defaultEmail = emails.Entries.GetOrCreate();
            defaultEmail.SmtpAddress = "ruud@partechit.nl";
            */

            var userDataFacet = contact.GetFacet<IUserDataFacet>();

            userDataFacet.Profession = "Sitecore thug";
            userDataFacet.Social = new SocialElement
            {
                Google = "http://www.google.com",
                Twitter = "http://www.twitter.com",
                YouTube = "http://www.youtube.com"
            };

            userDataFacet.Books = new List<IBookElement>
            {
                new BookElement { Title = "Professional Sitecore Development", Isbn = "123", Available = true },
                new BookElement { Title = "Snow white", Isbn = "456", Available = true },
                new BookElement { Title = "Java sucks", Isbn = "789", Available = false }
            };
        }
    }
}