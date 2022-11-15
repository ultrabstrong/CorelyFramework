using System.ComponentModel.DataAnnotations;
using rm = Resources.BusinessLayer.MaintenanceRequest;
using vrm = Resources.BusinessLayer.MaintenanceValidation;

namespace BusinessLayer
{
    public class MaintenanceRequest
    {
        #region Constructor

        public MaintenanceRequest() { }

        #endregion

        #region Properties

        [Display(Name = nameof(rm.MAINTENANCE_RENTAL_ADDRESS), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.MAINTENANCE_RENTAL_ADDRESS), ErrorMessageResourceType = typeof(vrm))]
        public string RentalAddress { get; set; }

        [Display(Name = nameof(rm.MAINTENANCE_FIRSTNAME), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.MAINTENANCE_FIRSTNAME), ErrorMessageResourceType = typeof(vrm))]
        public string FirstName { get; set; }

        [Display(Name = nameof(rm.MAINTENANCE_LASTNAME), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.MAINTENANCE_LASTNAME), ErrorMessageResourceType = typeof(vrm))]
        public string LastName { get; set; }

        [Display(Name = nameof(rm.MAINTENANCE_EMAIL), ResourceType = typeof(rm))]
        public string Email { get; set; }

        [Display(Name = nameof(rm.MAINTENANCE_PHONE), ResourceType = typeof(rm))]
        public string Phone { get; set; }

        [Display(Name = nameof(rm.MAINTENANCE_DESCRIPTION), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.MAINTENANCE_DESCRIPTION), ErrorMessageResourceType = typeof(vrm))]
        public string Description { get; set; }

        #endregion
    }
}
