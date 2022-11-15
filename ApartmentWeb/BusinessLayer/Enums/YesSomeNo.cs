using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rm = Resources.BusinessLayer.Application;

namespace BusinessLayer.Enums
{
    public enum YesSomeNo
    {
        [Display(Name = nameof(rm.ENUM_YESNO_NO), ResourceType = typeof(rm))]
        No = 1,
        [Display(Name = nameof(rm.ENUM_YESNO_YES), ResourceType = typeof(rm))]
        Yes = 2,
        [Display(Name = nameof(rm.ENUM_YESNO_SOME), ResourceType = typeof(rm))]
        Some = 3
    }
}
