using Corely.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Sage300HH2.Core
{
    [Serializable]
    public class Vendor : IVersioned
    {
        #region Properties

        public string Id { get; set; }

        public int Version { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Address4 { get; set; }

        public string City { get; set; }

        public string Code { get; set; }

        public string Country { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string DefaultExpenseAccount { get; set; }

        public string DefaultStandardCategory { get; set; }

        public string DefaultStandardCategoryCode { get; set; }

        public string FaxNumber { get; set; }

        public bool HasExternalId { get; set; }

        public bool IsActive { get; set; }

        public bool IsArchived { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string PostalCode { get; set; }

        public string State { get; set; }

        public Type Type { get; set; }

        public string TypeId { get; set; }

        #endregion

        #region Methods

        public int GetVersion() => Version;

        public override string ToString()
        {
            return $"{Code} | {Name}";
        }

        #endregion
    }
}
