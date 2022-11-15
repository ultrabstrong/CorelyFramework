using rm = Resources.BusinessLayer.Application;
using vrm = Resources.BusinessLayer.ApplicationValidation;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Validation;
using BusinessLayer.Enums;

namespace BusinessLayer
{
    public class Address
    {
        #region Constructor

        public Address()
        {

        }

        #endregion

        #region Properties

        [Display(Name = nameof(rm.ADDRESS_STREET), ResourceType = typeof(rm))]
        //[RequiredIf("ElectiveRequireValue", YesNo.Yes, ErrorMessageResourceName = nameof(vrm.ADDRESS_STREET), ErrorMessageResourceType = typeof(vrm))]
        public string Street { get; set; }

        [Display(Name = nameof(rm.ADDRESS_CITY), ResourceType = typeof(rm))]
        //[RequiredIf("ElectiveRequireValue", YesNo.Yes, ErrorMessageResourceName = nameof(vrm.ADDRESS_CITY), ErrorMessageResourceType = typeof(vrm))]
        public string City { get; set; }

        [Display(Name = nameof(rm.ADDRESS_STATE), ResourceType = typeof(rm))]
        //[RequiredIf("ElectiveRequireValue", YesNo.Yes, ErrorMessageResourceName = nameof(vrm.ADDRESS_STATE), ErrorMessageResourceType = typeof(vrm))]
        public string State { get; set; }

        [Display(Name = nameof(rm.ADDRESS_ZIP), ResourceType = typeof(rm))]
        //[RequiredIf("ElectiveRequireValue", YesNo.Yes, ErrorMessageResourceName = nameof(vrm.ADDRESS_ZIP), ErrorMessageResourceType = typeof(vrm))]
        public string Zip { get; set; }

        #endregion
    }
}
