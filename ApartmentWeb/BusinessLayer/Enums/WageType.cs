using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rm = Resources.BusinessLayer.Application;

namespace BusinessLayer.Enums
{
    public enum WageType
    {
        [Display(Name = nameof(rm.ENUM_WAGE_HOURLY), ResourceType = typeof(rm))]
        Hourly = 1,
        [Display(Name = nameof(rm.ENUM_WAGE_SALARY), ResourceType = typeof(rm))]
        Salary = 2
    }
}
