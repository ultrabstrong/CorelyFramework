using BusinessLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BusinessLayer.Validation
{
    public class RangeIfEnumAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Instance name
        /// </summary>
        public string InstanceName { get; set; }
        /// <summary>
        /// Dependant Property Name
        /// </summary>
        private string CheckIfName { get; set; }
        /// <summary>
        /// Type of enum for int conversion
        /// </summary>
        private Type EnumType { get; set; }
        /// <summary>
        /// Dependant Property Expected Value
        /// </summary>
        private int CheckIfValue { get; set; }
        /// <summary>
        /// Minimum value
        /// </summary>
        private decimal? MinValue { get; set; } = null;
        /// <summary>
        /// Maximum value
        /// </summary>
        private decimal? MaxValue { get; set; } = null;
        /// <summary>
        /// Decimal place accuracy. 0 means whole numbers
        /// </summary>
        private short Accuracy { get; set; }
        
        /// <summary>
        /// Constructor with error
        /// </summary>
        /// <param name="minval"></param>
        /// <param name="maxval"></param>
        /// <param name="accuracy"></param>
        /// <param name="checkIfName"></param>
        /// <param name="checkIfValue"></param>
        /// <param name="errorName"></param>
        /// <param name="errorType"></param>
        public RangeIfEnumAttribute(string minval, short accuracy, string maxval, string checkIfName, object checkIfValue, string errorName, Type errorType)
        {
            MinValue = decimal.Round(Convert.ToDecimal(minval), accuracy);
            Accuracy = accuracy; 
            MaxValue = decimal.Round(Convert.ToDecimal(maxval), accuracy);
            CheckIfName = checkIfName;
            EnumType = checkIfValue.GetType();
            CheckIfValue = (int)checkIfValue;
            ErrorMessageResourceName = errorName;
            ErrorMessageResourceType = errorType;
        }

        /// <summary>
        /// Constructor with error
        /// </summary>
        /// <param name="minval"></param>
        /// <param name="maxval"></param>
        /// <param name="accuracy"></param>
        /// <param name="checkIfName"></param>
        /// <param name="checkIfValue"></param>
        /// <param name="errorName"></param>
        /// <param name="errorType"></param>
        public RangeIfEnumAttribute(string minval, short accuracy, string checkIfName, object checkIfValue, string errorName, Type errorType)
        {
            MinValue = decimal.Round(Convert.ToDecimal(minval), accuracy);
            Accuracy = accuracy;
            MaxValue = null;
            CheckIfName = checkIfName;
            EnumType = checkIfValue.GetType();
            CheckIfValue = (int)checkIfValue;
            ErrorMessageResourceName = errorName;
            ErrorMessageResourceType = errorType;
        }

        /// <summary>
        /// Constructor with error
        /// </summary>
        /// <param name="minval"></param>
        /// <param name="maxval"></param>
        /// <param name="accuracy"></param>
        /// <param name="checkIfName"></param>
        /// <param name="checkIfValue"></param>
        /// <param name="errorName"></param>
        /// <param name="errorType"></param>
        public RangeIfEnumAttribute(short accuracy, string maxval, string checkIfName, object checkIfValue, string errorName, Type errorType)
        {
            MinValue = null;
            Accuracy = accuracy;
            MaxValue = decimal.Round(Convert.ToDecimal(maxval), accuracy);
            CheckIfName = checkIfName;
            EnumType = checkIfValue.GetType();
            CheckIfValue = (int)checkIfValue;
            ErrorMessageResourceName = errorName;
            ErrorMessageResourceType = errorType;
        }

        /// <summary>
        /// Check validity
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var dependentValue = context.ObjectInstance.GetType().GetProperty(CheckIfName).GetValue(context.ObjectInstance, null);

            if (dependentValue != null && dependentValue.ToString() == ((Enum)Enum.ToObject(EnumType, CheckIfValue)).ToString())
            {
                decimal decimalvalue = Decimal.Round(Convert.ToDecimal(value), Accuracy);
                if (MinValue != null && decimalvalue < MinValue ||
                    MaxValue != null && decimalvalue > MaxValue)
                {
                    return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
                }
            }
            return ValidationResult.Success;
        }

        /// <summary>
        /// Define client validation settings
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessageString,
                ValidationType = nameof(RangeIfEnumAttribute).Replace(nameof(Attribute), "").ToLower()
            };
            rule.ValidationParameters[nameof(MinValue).ToLower()] = MinValue;
            rule.ValidationParameters[nameof(MaxValue).ToLower()] = MaxValue;
            rule.ValidationParameters[nameof(Accuracy).ToLower()] = Accuracy;
            rule.ValidationParameters[nameof(CheckIfName).ToLower()] = CheckIfName;
            rule.ValidationParameters[nameof(CheckIfValue).ToLower()] = CheckIfValue;

            yield return rule;
        }
    }
}
