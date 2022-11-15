using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rm = Resources.BusinessLayer.Application;

namespace BusinessLayer.Enums
{
    public enum HowOften
    {
        [Display(Name = nameof(rm.ENUM_HOWOFTEN_OCCASIONAL), ResourceType = typeof(rm))]
        Occasionally = 1,
        [Display(Name = nameof(rm.ENUM_HOWOFTEN_MODERATE), ResourceType = typeof(rm))]
        Moderately = 2,
        [Display(Name = nameof(rm.ENUM_HOWOFTEN_FREQUENT), ResourceType = typeof(rm))]
        Frequently = 3
    }
}
