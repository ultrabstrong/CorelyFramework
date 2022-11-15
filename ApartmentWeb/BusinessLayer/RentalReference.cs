using BusinessLayer.Enums;
using BusinessLayer.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using rm = Resources.BusinessLayer.Application;
using vrm = Resources.BusinessLayer.ApplicationValidation;

namespace BusinessLayer
{
    public class RentalReference
    {
        #region Constructor

        public RentalReference()
        {

        }

        #endregion

        #region Properties

        public string DisplayName { get; set; }

        public bool AllowElectiveRequire { get; set; }

        public string ElectiveRequireDisplay { get; set; }

        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_ADD_RENT_REF), ErrorMessageResourceType = typeof(vrm))]
        public YesNo ElectiveRequireValue { get; set; }

        [Display(Name = nameof(rm.RENTREF_ADDRESS), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.RENTREF_ADDRESS), typeof(vrm))]
        public Address Address { get; set; } = new Address();

        [Display(Name = nameof(rm.RENTREF_NAME), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.RENTREF_NAME), typeof(vrm))]
        public string LandlordName { get; set; }

        [Display(Name = nameof(rm.RENTREF_PHONE), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.RENTREF_PHONE), typeof(vrm))]
        public string LandlordPhoneNum { get; set; }

        [Display(Name = nameof(rm.RENTREF_START), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.RENTREF_START), typeof(vrm))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? Start { get; set; } 

        [Display(Name = nameof(rm.RENTREF_END), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.RENTREF_END), typeof(vrm))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? End { get; set; }

        [Display(Name = nameof(rm.RENTREF_MOVING_REASON), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.RENTREF_MOVING_REASON), typeof(vrm))]
        public string ReasonForMoving { get; set; }

        #endregion
    }
}
