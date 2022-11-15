using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Sage300HH2.Core
{
    [Serializable]
    public class RequestExecutionStatus
    {
        #region Properties

        public string Id { get; set; }

        public string CreatedOn { get; set; }

        public string TransmittedOn { get; set; }

        public string ReceivedOn { get; set; }

        public string DisabledOn { get; set; }

        public string CompletedOn { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{nameof(Id)} : {Id} , {nameof(CreatedOn)} : {CreatedOn} , {nameof(TransmittedOn)} : {TransmittedOn} , {nameof(ReceivedOn)} : {ReceivedOn} , {nameof(DisabledOn)} : {DisabledOn} , {nameof(CompletedOn)} : {CompletedOn}";
        }

        #endregion
    }
}
