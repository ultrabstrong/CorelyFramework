using DocuWare.Platform.ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rm = UsefulUtilities.DocuWare.Resources;

namespace UsefulUtilities.DocuWare
{
    public static class Helpers
    {

        #region Get document methods

        // GET

        public static string GetField(this Document doc, string fieldName) => GetField(doc.Fields, fieldName);
        public static int GetIntField(this Document doc, string fieldName) => GetIntField(doc.Fields, fieldName);
        public static string GetIntField(this Document doc, string fieldName, string format = "") => GetIntField(doc.Fields, fieldName, format);
        public static decimal GetDecimalField(this Document doc, string fieldName) => GetDecimalField(doc.Fields, fieldName);
        public static string GetDecimalField(this Document doc, string fieldName, string format = "") => GetDecimalField(doc.Fields, fieldName, format);
        public static DateTime GetDateField(this Document doc, string fieldName) => GetDateField(doc.Fields, fieldName);
        public static string GetDateField(this Document doc, string fieldName, string format = "") => GetDateField(doc.Fields, fieldName, format);
        public static List<string> GetKeywordField(this Document doc, string fieldName) => GetKeywordField(doc.Fields, fieldName);

        // TRY GET

        public static string TryGetField(this Document doc, string fieldName) => TryGetField(doc.Fields, fieldName);
        public static bool TryGetField(this Document doc, string fieldName, out string fieldVal) => TryGetField(doc.Fields, fieldName, out fieldVal);
        public static int? TryGetIntField(this Document doc, string fieldName) => TryGetIntField(doc.Fields, fieldName);
        public static bool TryGetIntField(this Document doc, string fieldName, out int? fieldVal) => TryGetIntField(doc.Fields, fieldName, out fieldVal);
        public static string TryGetIntField(this Document doc, string fieldName, string format = "") => TryGetIntField(doc.Fields, fieldName, format);
        public static bool TryGetIntField(this Document doc, string fieldName, out string fieldVal, string format = "") => TryGetIntField(doc.Fields, fieldName, out fieldVal, format);
        public static decimal? TryGetDecimalField(this Document doc, string fieldName) => TryGetDecimalField(doc.Fields, fieldName);
        public static bool TryGetDecimalField(this Document doc, string fieldName, out decimal? fieldVal) => TryGetDecimalField(doc.Fields, fieldName, out fieldVal);
        public static string TryGetDecimalField(this Document doc, string fieldName, string format = "F") => TryGetDecimalField(doc.Fields, fieldName, format);
        public static bool TryGetDecimalField(this Document doc, string fieldName, out string fieldVal, string format = "F") => TryGetDecimalField(doc.Fields, fieldName, out fieldVal, format);
        public static DateTime? TryGetDateField(this Document doc, string fieldName) => TryGetDateField(doc.Fields, fieldName);
        public static bool TryGetDateField(this Document doc, string fieldName, out DateTime? fieldVal) => TryGetDateField(doc.Fields, fieldName, out fieldVal);
        public static string TryGetDateField(this Document doc, string fieldName, string format = "MM/dd/yyyy") => TryGetDateField(doc.Fields, fieldName, format);
        public static bool TryGetDateField(this Document doc, string fieldName, out string fieldVal, string format = "") => TryGetDateField(doc.Fields, fieldName, out fieldVal, format);
        public static List<string> TryGetKeywordField(this Document doc, string fieldName) => TryGetKeywordField(doc.Fields, fieldName);
        public static bool TryGetKeywordField(this Document doc, string fieldName, out List<string> fieldVal) => TryGetKeywordField(doc.Fields, fieldName, out fieldVal);

        #endregion

        #region Get row methods

        // GET

        public static string GetField(this DocumentIndexFieldTableRow row, string fieldName) => GetField(row.ColumnValue, fieldName);
        public static int GetIntField(this DocumentIndexFieldTableRow row, string fieldName) => GetIntField(row.ColumnValue, fieldName);
        public static string GetIntField(this DocumentIndexFieldTableRow row, string fieldName, string format = "") => GetIntField(row.ColumnValue, fieldName, format);
        public static decimal GetDecimalField(this DocumentIndexFieldTableRow row, string fieldName) => GetDecimalField(row.ColumnValue, fieldName);
        public static string GetDecimalField(this DocumentIndexFieldTableRow row, string fieldName, string format = "") => GetDecimalField(row.ColumnValue, fieldName, format);
        public static DateTime GetDateField(this DocumentIndexFieldTableRow row, string fieldName) => GetDateField(row.ColumnValue, fieldName);
        public static string GetDateField(this DocumentIndexFieldTableRow row, string fieldName, string format = "") => GetDateField(row.ColumnValue, fieldName, format);
        public static List<string> GetKeywordField(this DocumentIndexFieldTableRow row, string fieldName) => GetKeywordField(row.ColumnValue, fieldName);

        // TRY GET

        public static string TryGetField(this DocumentIndexFieldTableRow row, string fieldName) => TryGetField(row.ColumnValue, fieldName);
        public static bool TryGetField(this DocumentIndexFieldTableRow row, string fieldName, out string fieldVal) => TryGetField(row.ColumnValue, fieldName, out fieldVal);
        public static int? TryGetIntField(this DocumentIndexFieldTableRow row, string fieldName) => TryGetIntField(row.ColumnValue, fieldName);
        public static bool TryGetIntField(this DocumentIndexFieldTableRow row, string fieldName, out int? fieldVal) => TryGetIntField(row.ColumnValue, fieldName, out fieldVal);
        public static string TryGetIntField(this DocumentIndexFieldTableRow row, string fieldName, string format = "") => TryGetIntField(row.ColumnValue, fieldName, format);
        public static bool TryGetIntField(this DocumentIndexFieldTableRow row, string fieldName, out string fieldVal, string format = "") => TryGetIntField(row.ColumnValue, fieldName, out fieldVal, format);
        public static decimal? TryGetDecimalField(this DocumentIndexFieldTableRow row, string fieldName) => TryGetDecimalField(row.ColumnValue, fieldName);
        public static bool TryGetDecimalField(this DocumentIndexFieldTableRow row, string fieldName, out decimal? fieldVal) => TryGetDecimalField(row.ColumnValue, fieldName, out fieldVal);
        public static string TryGetDecimalField(this DocumentIndexFieldTableRow row, string fieldName, string format = "F") => TryGetDecimalField(row.ColumnValue, fieldName, format);
        public static bool TryGetDecimalField(this DocumentIndexFieldTableRow row, string fieldName, out string fieldVal, string format = "F") => TryGetDecimalField(row.ColumnValue, fieldName, out fieldVal, format);
        public static DateTime? TryGetDateField(this DocumentIndexFieldTableRow row, string fieldName) => TryGetDateField(row.ColumnValue, fieldName);
        public static bool TryGetDateField(this DocumentIndexFieldTableRow row, string fieldName, out DateTime? fieldVal) => TryGetDateField(row.ColumnValue, fieldName, out fieldVal);
        public static string TryGetDateField(this DocumentIndexFieldTableRow row, string fieldName, string format = "MM/dd/yyyy") => TryGetDateField(row.ColumnValue, fieldName, format);
        public static bool TryGetDateField(this DocumentIndexFieldTableRow row, string fieldName, out string fieldVal, string format = "") => TryGetDateField(row.ColumnValue, fieldName, out fieldVal, format);
        public static List<string> TryGetKeywordField(this DocumentIndexFieldTableRow row, string fieldName) => TryGetKeywordField(row.ColumnValue, fieldName);
        public static bool TryGetKeywordField(this DocumentIndexFieldTableRow row, string fieldName, out List<string> fieldVal) => TryGetKeywordField(row.ColumnValue, fieldName, out fieldVal);
        
        #endregion

        #region Get base methods

        // STRING

        public static string GetField(this List<DocumentIndexField> fields, string fieldName)
        {
            string fieldVal = fields.FirstOrDefault(m => m.FieldName == fieldName)?.Item?.ToString();
            if (string.IsNullOrWhiteSpace(fieldVal)) { throw new Exception($"{rm.fieldNotEmpty} {fieldName}"); }
            return fieldVal;
        }

        // INT

        public static int GetIntField(this List<DocumentIndexField> fields, string fieldName)
        {
            string fieldVal = fields.GetField(fieldName);
            if (!int.TryParse(fieldVal, out int parsedField)) { throw new Exception($"{rm.fieldMustBeInt} {fieldName}"); }
            return parsedField;
        }

        public static string GetIntField(this List<DocumentIndexField> fields, string fieldName, string format = "")
        {
            string fieldVal = fields.GetField(fieldName);
            if (!int.TryParse(fieldVal, out int parsedField)) { throw new Exception($"{rm.fieldMustBeInt} {fieldName}"); }
            return parsedField.ToString();
        }

        // DECIMAL

        public static decimal GetDecimalField(this List<DocumentIndexField> fields, string fieldName)
        {
            string fieldVal = fields.GetField(fieldName);
            if (!decimal.TryParse(fieldVal, out decimal parsedField)) { throw new Exception($"{rm.fieldMustBeDecimal} {fieldName}"); }
            return parsedField;
        }

        public static string GetDecimalField(this List<DocumentIndexField> fields, string fieldName, string format = "F")
        {
            string fieldVal = fields.GetField(fieldName);
            if (!decimal.TryParse(fieldVal, out decimal parsedField)) { throw new Exception($"{rm.fieldMustBeDecimal} {fieldName}"); }
            return parsedField.ToString(format);
        }

        // DATE

        public static DateTime GetDateField(this List<DocumentIndexField> fields, string fieldName)
        {
            string fieldVal = fields.GetField(fieldName);
            if (!DateTime.TryParse(fieldVal, out DateTime parsedField)) { throw new Exception($"{rm.fieldMustBeDate} {fieldName}"); }
            return parsedField;
        }

        public static string GetDateField(this List<DocumentIndexField> fields, string fieldName, string format = "MM/dd/yyyy")
        {
            string fieldVal = fields.GetField(fieldName);
            if (!DateTime.TryParse(fieldVal, out DateTime parsedField)) { throw new Exception($"{rm.fieldMustBeDate} {fieldName}"); }
            return parsedField.ToString(format);
        }

        // KEYWORD

        public static List<string> GetKeywordField(this List<DocumentIndexField> fields, string fieldName)
        {
            DocumentIndexFieldKeywords fieldVal = fields.FirstOrDefault(m => m.FieldName == fieldName)?.Item as DocumentIndexFieldKeywords;
            if (fieldVal == null) { throw new Exception($"{rm.fieldMustBeKeyword} {fieldName}"); }
            return fieldVal.Keyword;
        }

        #endregion

        #region Try get base methods

        // STRING

        public static string TryGetField(this List<DocumentIndexField> fields, string fieldName)
        {
            string fieldVal = fields.FirstOrDefault(m => m.FieldName == fieldName)?.Item?.ToString();
            if (string.IsNullOrWhiteSpace(fieldVal)) { fieldVal = null; }
            return fieldVal;
        }

        public static bool TryGetField(this List<DocumentIndexField> fields, string fieldName, out string fieldVal)
        {
            fieldVal = fields.FirstOrDefault(m => m.FieldName == fieldName)?.Item?.ToString();
            if (string.IsNullOrWhiteSpace(fieldVal)) { fieldVal = null; }
            return fieldVal != null;
        }

        // INT

        public static int? TryGetIntField(this List<DocumentIndexField> fields, string fieldName)
        {
            string fieldVal = fields.TryGetField(fieldName);
            if (!int.TryParse(fieldVal, out int parsedField)) { return null; }
            return parsedField;
        }

        public static bool TryGetIntField(this List<DocumentIndexField> fields, string fieldName, out int? fieldVal)
        {
            string fieldValStr = fields.TryGetField(fieldName);
            if (!int.TryParse(fieldValStr, out int tempFieldVal)) 
            {
                fieldVal = null;
                return false; 
            }
            fieldVal = tempFieldVal;
            return true;
        }

        public static string TryGetIntField(this List<DocumentIndexField> fields, string fieldName, string format = "")
        {
            string fieldVal = fields.TryGetField(fieldName);
            if (!int.TryParse(fieldVal, out int parsedField)) { return null; }
            return parsedField.ToString();
        }

        public static bool TryGetIntField(this List<DocumentIndexField> fields, string fieldName, out string fieldVal, string format = "")
        {
            string fieldValStr = fields.TryGetField(fieldName);
            if (!int.TryParse(fieldValStr, out int parsedField)) { fieldVal = null; }
            else { fieldVal = parsedField.ToString(); }
            return fieldVal != null;
        }

        // DECIMAL

        public static decimal? TryGetDecimalField(this List<DocumentIndexField> fields, string fieldName)
        {
            string fieldVal = fields.TryGetField(fieldName);
            if (!decimal.TryParse(fieldVal, out decimal parsedField)) { return null; }
            return parsedField;
        }

        public static bool TryGetDecimalField(this List<DocumentIndexField> fields, string fieldName, out decimal? fieldVal)
        {
            string fieldValStr = fields.TryGetField(fieldName);
            if (!decimal.TryParse(fieldValStr, out decimal tempFieldVal)) 
            {
                fieldVal = null;
                return false; 
            }
            fieldVal = tempFieldVal;
            return true;
        }

        public static string TryGetDecimalField(this List<DocumentIndexField> fields, string fieldName, string format = "F")
        {
            string fieldVal = fields.TryGetField(fieldName);
            if (!decimal.TryParse(fieldVal, out decimal parsedField)) { return null; }
            return parsedField.ToString(format);
        }

        public static bool TryGetDecimalField(this List<DocumentIndexField> fields, string fieldName, out string fieldVal, string format = "")
        {
            string fieldValStr = fields.TryGetField(fieldName);
            if (!decimal.TryParse(fieldValStr, out decimal parsedField)) { fieldVal = null; }
            else { fieldVal = parsedField.ToString(); }
            return fieldVal != null;
        }

        // DATE

        public static DateTime? TryGetDateField(this List<DocumentIndexField> fields, string fieldName)
        {
            string fieldVal = fields.TryGetField(fieldName);
            if (!DateTime.TryParse(fieldVal, out DateTime parsedField)) { return null; }
            return parsedField;
        }

        public static bool TryGetDateField(this List<DocumentIndexField> fields, string fieldName, out DateTime? fieldVal)
        {
            string fieldValStr = fields.TryGetField(fieldName);
            if (!DateTime.TryParse(fieldValStr, out DateTime tempFieldVal)) 
            {
                fieldVal = null;
                return false;
            }
            fieldVal = tempFieldVal;
            return true;
        }

        public static string TryGetDateField(this List<DocumentIndexField> fields, string fieldName, string format = "MM/dd/yyyy")
        {
            string fieldVal = fields.TryGetField(fieldName);
            if (!DateTime.TryParse(fieldVal, out DateTime parsedField)) { return null; }
            return parsedField.ToString(format);
        }

        public static bool TryGetDateField(this List<DocumentIndexField> fields, string fieldName, out string fieldVal, string format = "")
        {
            string fieldValStr = fields.TryGetField(fieldName);
            if (!DateTime.TryParse(fieldValStr, out DateTime parsedField)) { fieldVal = null; }
            else { fieldVal = parsedField.ToString(); }
            return fieldVal != null;
        }

        // KEYWORD

        public static List<string> TryGetKeywordField(this List<DocumentIndexField> fields, string fieldName)
        {
            List<string> fieldVal = (fields.FirstOrDefault(m => m.FieldName == fieldName)?.Item as DocumentIndexFieldKeywords)?.Keyword;
            return fieldVal;
        }

        public static bool TryGetKeywordField(this List<DocumentIndexField> fields, string fieldName, out List<string> fieldVal)
        {
            fieldVal = (fields.FirstOrDefault(m => m.FieldName == fieldName)?.Item as DocumentIndexFieldKeywords)?.Keyword;
            return fieldVal != null;
        }

        #endregion
    }
}
