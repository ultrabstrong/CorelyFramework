using Corely.Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Corely.UI
{
    #region Generic Converters

    /// <summary>
    /// Boolean to visibility converter
    /// </summary>
    public partial class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get boolean and make sure it is not null
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }

            // Return inverted visibility
            return ((bool)value ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return opposite of visibility or false
            if (value is Visibility)
            {
                return ((Visibility)value == Visibility.Visible);
            }
            else return true;
        }
    }

    /// <summary>
    /// Inverted boolean to visibility converter
    /// </summary>
    public partial class InvertedBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get boolean and make sure it is not null
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }

            // Return inverted visibility
            return ((bool)value ? Visibility.Collapsed : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return opposite of visibility or false
            if (value is Visibility)
            {
                return ((Visibility)value == Visibility.Collapsed);
            }
            else return true;
        }
    }

    /// <summary>
    /// Enum to text converter
    /// </summary>
    public class EnumConverter : IValueConverter
    {
        /// <summary>
        /// Get enumeration text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) { return ""; }
            if (parameter != null)
            {
                return ReflectionHelper.GetResource((Type)parameter, value.ToString());
            }
            return "";
        }
        /// <summary>
        /// Convert enumeration text back
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// Decimal to text converter
    /// </summary>
    public class DecimalConverter : IValueConverter
    {
        /// <summary>
        /// Get decimal text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }
        /// <summary>
        /// Convert decimal text back
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string stringval = value.ToString();
            if (stringval.EndsWith(".")) { stringval += "0"; }
            decimal.TryParse(stringval, out decimal d);
            return d;
        }
    }

    /// <summary>
    /// Convert decimal or int
    /// </summary>
    public class DecimalIntConverter : IValueConverter
    {
        /// <summary>
        /// Get decimal or int text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert to int if param = true and value <> null
            if (bool.TryParse(parameter?.ToString(), out bool isint) &&
                isint &&
                decimal.TryParse(value?.ToString(), out decimal d))
            {
                value = decimal.Truncate(d);
            }

            return value.ToString();
        }

        /// <summary>
        /// Convert decimal text back
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get value and format with 0 if . is the end of the string
            string stringval = value?.ToString() ?? "";
            if (stringval.EndsWith(".")) { stringval += "0"; }
            // Convert to int if param = true and value <> null
            if (decimal.TryParse(stringval, out decimal d) &&
                bool.TryParse(parameter?.ToString(), out bool isint) &&
                isint)
            {
                d = decimal.Truncate(d);
            }

            return d;
        }
    }

    #endregion

}
