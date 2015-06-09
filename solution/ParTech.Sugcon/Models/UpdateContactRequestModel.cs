namespace ParTech.Sugcon.Models
{
    using System;

    public class UpdateContactRequestModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public Guid? ContactId { get; set; }

        public string Identifier { get; set; }
    }
}