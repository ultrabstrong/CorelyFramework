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
    public class RequireIfEnumAttribute : ValidationAttribute, IClientValidatable
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
        /// Inner attribute
        /// </summary>
        private readonly RequiredAttribute _innerAttribute;

        /// <summary>
        /// Constructor with error
        /// </summary>
        /// <param name="checkIfName"></param>
        /// <param name="checkIfValue"></param>
        /// <param name="errorName"></param>
        /// <param name="errorType"></param>
        public RequireIfEnumAttribute(string checkIfName, object checkIfValue, string errorName, Type errorType) : this(checkIfName, checkIfValue)
        {
            ErrorMessageResourceName = errorName;
            ErrorMessageResourceType = errorType;
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="checkIfName"></param>
        /// <param name="checkIfValue"></param>
        public RequireIfEnumAttribute(string checkIfName, object checkIfValue)
        {
            CheckIfName = checkIfName;
            EnumType = checkIfValue.GetType();
            CheckIfValue = (int)checkIfValue;
            _innerAttribute = new RequiredAttribute();
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
                if (!_innerAttribute.IsValid(value))
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
                ValidationType = nameof(RequireIfEnumAttribute).Replace(nameof(Attribute), "").ToLower()
            };
            rule.ValidationParameters[nameof(CheckIfName).ToLower()] = CheckIfName;
            rule.ValidationParameters[nameof(CheckIfValue).ToLower()] = CheckIfValue;

            yield return rule;
        }
    }
}
