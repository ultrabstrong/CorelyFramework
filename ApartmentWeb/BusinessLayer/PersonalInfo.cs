using rm = Resources.BusinessLayer.Application;
using vrm = Resources.BusinessLayer.ApplicationValidation;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer
{
    public class PersonalInfo
    {
        #region Constructor

        public PersonalInfo()
        {

        }

        #endregion

        #region Properties

        public string DisplayName { get; set; }

        public bool AllowElectiveRequire { get; set; }

        public string ElectiveRequireDisplay { get; set; }

        [Display(Name = nameof(rm.PERSONAL_FIRSTNAME), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.PERSONAL_FIRSTNAME), ErrorMessageResourceType = typeof(vrm))]
        public string FirstName { get; set; }

        [Display(Name = nameof(rm.PERSONAL_MIDDLENAME), ResourceType = typeof(rm))]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.PERSONAL_MIDDLENAME), ErrorMessageResourceType = typeof(vrm))]
        public string MiddleName { get; set; }

        [Display(Name = nameof(rm.PERSONAL_LASTNAME), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.PERSONAL_LASTNAME), ErrorMessageResourceType = typeof(vrm))]
        public string LastName { get; set; }

        [Display(Name = nameof(rm.PERSONAL_PHONENUM), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.PERSONAL_PHONENUM), ErrorMessageResourceType = typeof(vrm))]
        public string PhoneNum { get; set; }

        [Display(Name = nameof(rm.PERSONAL_SSN), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.PERSONAL_SSN), ErrorMessageResourceType = typeof(vrm))]
        public string SSN { get; set; }

        [Display(Name = nameof(rm.PERSONAL_DL_NUM), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.PERSONAL_DL_NUM), ErrorMessageResourceType = typeof(vrm))]
        public string DriverLicense { get; set; }

        [Display(Name = nameof(rm.PERSONAL_DL_STATE), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.PERSONAL_DL_STATE), ErrorMessageResourceType = typeof(vrm))]
        public string DriverLicenseStateOfIssue { get; set; }

        [Display(Name = nameof(rm.PERSONAL_EMAIL), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.PERSONAL_EMAIL), ErrorMessageResourceType = typeof(vrm))]
        public string Email { get; set; }

        #endregion
    }
}
