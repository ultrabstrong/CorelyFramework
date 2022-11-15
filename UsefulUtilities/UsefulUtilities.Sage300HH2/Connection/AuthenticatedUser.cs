using System;

namespace UsefulUtilities.Sage300HH2.Connection
{
    [Serializable]
    public class AuthenticatedUser
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Id { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }
    }
}
