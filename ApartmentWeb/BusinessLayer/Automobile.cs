using rm = Resources.BusinessLayer.Application;
using vrm = Resources.BusinessLayer.ApplicationValidation;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Enums;
using BusinessLayer.Validation;

namespace BusinessLayer
{
    public class Automobile 
    {

        #region Constructor
        public Automobile()
        {

        }

        #endregion

        #region Properties

        public string DisplayName { get; set; }

        public bool AllowElectiveRequire { get; set; }

        [Display(Name = nameof(rm.APP_HAS_AUTO), ResourceType = typeof(rm))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_HAS_AUTO), ErrorMessageResourceType = typeof(vrm))]
        public YesNo ElectiveRequireValue { get; set; }

        [Display(Name = nameof(rm.AUTO_MAKE), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.AUTO_MAKE), typeof(vrm))]
        public string Make { get; set; }

        [Display(Name = nameof(rm.AUTO_MODEL), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.AUTO_MODEL), typeof(vrm))]
        public string Model { get; set; }

        [Display(Name = nameof(rm.AUTO_YEAR), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.AUTO_YEAR), typeof(vrm))]
        public string Year { get; set; }

        [Display(Name = nameof(rm.AUTO_STATE), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.AUTO_STATE), typeof(vrm))]
        public string State { get; set; }

        [Display(Name = nameof(rm.AUTO_LICENSE_NUM), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.AUTO_LICENSE_NUM), typeof(vrm))]
        public string LicenseNum { get; set; }

        [Display(Name = nameof(rm.AUTO_COLOR), ResourceType = typeof(rm))]
        [RequireIfEnum(nameof(ElectiveRequireValue), YesNo.Yes, nameof(vrm.AUTO_COLOR), typeof(vrm))]
        public string Color { get; set; }

        #endregion
    }
}
