using rm = Resources.BusinessLayer.Application;
using vrm = Resources.BusinessLayer.ApplicationValidation;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Enums;
using BusinessLayer.Validation;

namespace BusinessLayer
{
    public class PersonalReference
    {
        #region Constructor

        public PersonalReference()
        {

        }

        #endregion

        #region Properties

        public string DisplayName { get; set; }

        public bool AllowElectiveRequire { get; set; }

        public string ElectiveRequireDisplay { get; set; }

        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_ADD_PERSONAL_REF), ErrorMessageResourceType = typeof(vrm))]
        public YesNo ElectiveRequireValue { get; set; }

        [Display(Name = nameof(rm.REF_NAME), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.REF_NAME), typeof(vrm))]
        public string Name { get; set; }

        [Display(Name = nameof(rm.REF_RELATION), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.REF_NAME), typeof(vrm))]
        public string Relationship { get; set; }

        [Display(Name = nameof(rm.REF_PHONE), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.REF_NAME), typeof(vrm))]
        public string PhoneNum { get; set; }
        
        #endregion
    }
}
