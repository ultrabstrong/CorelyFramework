using System.Collections.Generic;
using BusinessLayer.Enums;
using rm = Resources.BusinessLayer.Application;
using vrm = Resources.BusinessLayer.ApplicationValidation;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Validation;

namespace BusinessLayer
{
    public class Application
    {
        #region Constructor

        public Application()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Rental Address
        /// </summary>
        [Display(Name = nameof(rm.APP_RENTAL_ADDRESS), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.APP_RENTAL_ADDRESS), ErrorMessageResourceType = typeof(vrm))]
        public string RentalAddress { get; set; }

        /// <summary>
        /// Other Applicants
        /// </summary>
        [Display(Name = nameof(rm.APP_OTHER_APPLICANTS), ResourceType = typeof(rm))]
        public string OtherApplicants { get; set; }

        /// <summary>
        /// Personal Information
        /// </summary>
        [Display(Name = nameof(rm.APP_PERSONAL_INFO), ResourceType = typeof(rm))]
        public PersonalInfo PersonalInfo { get; set; } = new PersonalInfo();

        /// <summary>
        /// Primary Employment
        /// </summary>
        public EmploymentInfo PrimaryEmployment { get; set; } = new EmploymentInfo()
        {
            DisplayName = rm.APP_PRIMARY_EMPLOYMENT,
            AllowElectiveRequire = true,
            ElectiveRequireDisplay = rm.APP_HAS_JOB
        };
        
        /// <summary>
        /// Secondary Employment (if selected)
        /// </summary>
        public EmploymentInfo SecondaryEmployment { get; set; } = new EmploymentInfo()
        {
            DisplayName = rm.APP_SECOND_EMPLOYMENT,
            AllowElectiveRequire = true,
            ElectiveRequireDisplay = rm.APP_HAS_SECOND_JOB
        };

        /// <summary>
        /// Parent information (if parents pay rent)
        /// </summary>
        public ParentInfo ParentInfo { get; set; } = new ParentInfo()
        {
            DisplayName = rm.APP_PARENT_INFO
        };

        /// <summary>
        /// ? Consider other income
        /// </summary>
        [Display(Name = nameof(rm.APP_CONSIDER_OTHER_INCOME), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_CONSIDER_OTHER_INCOME), ErrorMessageResourceType = typeof(vrm))]
        public YesNo ConsiderOtherIncome { get; set; }

        /// <summary>
        /// Other income explain (if consider other income)
        /// </summary>
        [Display(Name = nameof(rm.APP_OTHER_INCOME), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        [RequireIfEnum(nameof(ConsiderOtherIncome), YesNo.Yes, nameof(vrm.APP_OTHER_INCOME), typeof(vrm))]
        public string OtherIncomeExplain { get; set; }

        /// <summary>
        /// Automobile info (if selected)
        /// </summary>
        public Automobile Automobile { get; set; } = new Automobile()
        {
            DisplayName = rm.APP_AUTOMOBILE,
            AllowElectiveRequire = true
        };

        /// <summary>
        /// Current rental / residency
        /// </summary>
        public RentalReference CurrentRental { get; set; } = new RentalReference()
        {
            DisplayName = rm.APP_CURRENT_RESIDENCE,
            ElectiveRequireValue = YesNo.Yes
        };

        /// <summary>
        /// Second rental ref (if selected)
        /// </summary>
        public RentalReference PriorRentRef1 { get; set; } = new RentalReference()
        {
            DisplayName = rm.APP_RENT_REF_1,
            AllowElectiveRequire = true,
            ElectiveRequireValue = YesNo.No,
            ElectiveRequireDisplay = rm.APP_ADD_RENT_REF
        };

        /// <summary>
        /// Third rental ref (if selected)
        /// </summary>
        public RentalReference PriorRentRef2 { get; set; } = new RentalReference()
        {
            DisplayName = rm.APP_RENT_REF_2,
            AllowElectiveRequire = true,
            ElectiveRequireValue = YesNo.No,
            ElectiveRequireDisplay = rm.APP_ADD_RENT_REF
        };

        /// <summary>
        /// First Personal References (if selected)
        /// </summary>
        public PersonalReference PersonalReference1 = new PersonalReference()
        {
            DisplayName = rm.APP_PERSONAL_REF_1,
            AllowElectiveRequire = true,
            ElectiveRequireValue = YesNo.No,
            ElectiveRequireDisplay = rm.APP_ADD_PERSONAL_REF
        };

        /// <summary>
        /// Second Personal References (if selected)
        /// </summary>
        public PersonalReference PersonalReference2 = new PersonalReference()
        {
            DisplayName = rm.APP_PERSONAL_REF_2,
            AllowElectiveRequire = true,
            ElectiveRequireValue = YesNo.No,
            ElectiveRequireDisplay = rm.APP_ADD_PERSONAL_REF 
        };
        /// <summary>
        /// Anticipated duration of stay
        /// </summary>
        [Display(Name = nameof(rm.APP_ANTICIPATED_DURATION), ResourceType = typeof(rm))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = nameof(vrm.APP_ANTICIPATED_DURATION), ErrorMessageResourceType = typeof(vrm))]
        public string AnticipatedDuration { get; set; }

        /// <summary>
        /// ? Has criminal record
        /// </summary>
        [Display(Name = nameof(rm.APP_HAS_CRIMINAL_RECORD), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_HAS_CRIMINAL_RECORD), ErrorMessageResourceType = typeof(vrm))]
        public YesNo HasCriminalRecord { get; set; }

        /// <summary>
        /// Criminal record explain (if has criminal record)
        /// </summary>
        [Display(Name = nameof(rm.APP_CRIMINAL_RECORD), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        [RequireIfEnum(nameof(HasCriminalRecord), YesNo.Yes, nameof(vrm.APP_CRIMINAL_RECORD), typeof(vrm))]
        public string ExplainCriminalRecord { get; set; }

        /// <summary>
        /// ? Has been evicted
        /// </summary>
        [Display(Name = nameof(rm.APP_HAS_BEEN_EVICTED), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_HAS_BEEN_EVICTED), ErrorMessageResourceType = typeof(vrm))]
        public YesNo HasBeenEvicted { get; set; }

        /// <summary>
        /// Evicted explain (if has been evicted)
        /// </summary>
        [Display(Name = nameof(rm.APP_EVICTED_EXPLAIN), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        [RequireIfEnum(nameof(HasBeenEvicted), YesNo.Yes, nameof(vrm.APP_EVICTED_EXPLAIN), typeof(vrm))]
        public string ExplainBeenEvicted { get; set; }

        /// <summary>
        /// ? Has marijuana card
        /// </summary>
        [Display(Name = nameof(rm.APP_MARIJUANA), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_MARIJUANA), ErrorMessageResourceType = typeof(vrm))]
        public YesNo MarijuanaCard { get; set; }

        /// <summary>
        /// ? Has smokers
        /// </summary>
        [Display(Name = nameof(rm.APP_SMOKERS), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_SMOKERS), ErrorMessageResourceType = typeof(vrm))]
        public YesNo Smokers { get; set; }

        /// <summary>
        /// Number of smokers (if has smokers)
        /// </summary>
        [Display(Name = nameof(rm.APP_SMOKERS_COUNT), ResourceType = typeof(rm))]
        [RangeIfEnum("1", 0, "5", nameof(Smokers), YesNo.Yes, nameof(vrm.APP_SMOKERS_COUNT), typeof(vrm))]
        public int SmokersCount { get; set; }

        /// <summary>
        /// ? Has drinkers
        /// </summary>
        [Display(Name = nameof(rm.APP_DRINKERS), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_DRINKERS), ErrorMessageResourceType = typeof(vrm))]
        public YesNo Drinkers { get; set; }

        /// <summary>
        /// Drinks how often (if has drinkers)
        /// </summary>
        [Display(Name = nameof(rm.APP_DRINKERS_HOWOFTEN), ResourceType = typeof(rm))]
        [RangeIfEnum("1", 0, "3", nameof(Drinkers), YesNo.Yes, nameof(vrm.APP_DRINKERS_HOWOFTEN), typeof(vrm))]
        public HowOften HowOftenDrink { get; set; }

        /// <summary>
        /// ? Has pets
        /// </summary>
        [Display(Name = nameof(rm.APP_PETS), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_PETS), ErrorMessageResourceType = typeof(vrm))]
        public YesNo AnyPets { get; set; }

        /// <summary>
        /// Description of pets (if has pets)
        /// </summary>
        [Display(Name = nameof(rm.APP_PETS_DESCRIBE), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        [RequireIfEnum(nameof(AnyPets), YesNo.Yes, nameof(vrm.APP_PETS_DESCRIBE), typeof(vrm))]
        public string DescribePets { get; set; }

        /// <summary>
        /// ? Has non human
        /// </summary>
        [Display(Name = nameof(rm.APP_NON_HUMAN), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_NON_HUMAN), ErrorMessageResourceType = typeof(vrm))]
        public YesNo AnyNonHuman { get; set; }

        /// <summary>
        /// Descrition of non human (if has non human)
        /// </summary>
        [Display(Name = nameof(rm.APP_NON_HUMAN_DESCRIBE), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        [RequireIfEnum(nameof(AnyNonHuman), YesNo.Yes, nameof(vrm.APP_NON_HUMAN_DESCRIBE), typeof(vrm))]
        public string DescribeNonHuman { get; set; }

        /// <summary>
        /// ? Has attended college
        /// </summary>
        [Display(Name = nameof(rm.APP_ATTEND_COLLEGE), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_ATTEND_COLLEGE), ErrorMessageResourceType = typeof(vrm))]
        public YesNo AttendCollege { get; set; }

        /// <summary>
        /// College years attended (if has attended college)
        /// </summary>
        [Display(Name = nameof(rm.APP_COLLEGE_YEARS), ResourceType = typeof(rm))]
        [RangeIfEnum("0.1", 1, "20.0", nameof(AttendCollege), YesNo.Yes, nameof(vrm.APP_COLLEGE_YEARS), typeof(vrm))]
        public int CollegeYearsAttended { get; set; }

        /// <summary>
        /// Plan to graduate (if has attended college)
        /// </summary>
        [Display(Name = nameof(rm.APP_COLLEGE_GRADUATE), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        [RequireIfEnum(nameof(AttendCollege), YesNo.Yes, nameof(vrm.APP_COLLEGE_GRADUATE), typeof(vrm))]
        public string PlanToGraduate { get; set;}

        /// <summary>
        /// ? Need reasonable accommodation
        /// </summary>
        [Display(Name = nameof(rm.APP_ACCOMMODATION), ResourceType = typeof(rm))]
        [EnumDataType(typeof(YesNo))]
        [Range(1, 2, ErrorMessageResourceName = nameof(vrm.APP_ACCOMMODATION), ErrorMessageResourceType = typeof(vrm))]
        public YesNo NeedReasonableAccommodation { get; set; }

        /// <summary>
        /// Reasonable accommodation description (if need reasonable accommodation)
        /// </summary>
        [Display(Name = nameof(rm.APP_ACCOMMODATION_DESCRIBE), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        [RequireIfEnum(nameof(NeedReasonableAccommodation), YesNo.Yes, nameof(vrm.APP_ACCOMMODATION_DESCRIBE), typeof(vrm))]
        public string DescribeReasonableAccommodation { get; set; }

        /// <summary>
        /// Certification and authorization
        /// </summary>
        [Display(Name = nameof(rm.APP_CERT_AND_AUTH), ResourceType = typeof(rm))]
        [EnumDataType(typeof(Yes))]
        [Range(1, 1, ErrorMessageResourceName = nameof(vrm.APP_CERT_AND_AUTH), ErrorMessageResourceType = typeof(vrm))]
        public Yes CertificationAndAuthorization { get; set; }

        /// <summary>
        /// Additional Comments
        /// </summary>
        [Display(Name = nameof(rm.APP_ADDITIONAL_COMMENTS), ResourceType = typeof(rm))]
        [DataType(DataType.MultilineText)]
        public string AdditionalComments { get; set; }

        #endregion
    }
}
