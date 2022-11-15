using System;

namespace Corely.Sage300HH2.Core
{
    [Serializable]
    public class Account : IVersioned
    {
        #region Properties

        public string Id { get; set; }

        public int Version { get; set; }

        public string Code { get; set; }

        public string CostRequirement { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public bool IsArchived { get; set; }

        public string Name { get; set; }

        public string ParentCode { get; set; }

        public string ParentId { get; set; }

        public string ParentName { get; set; }

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
