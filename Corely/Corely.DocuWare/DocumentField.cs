using DocuWare.Platform.ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Corely.DocuWare
{

    [Serializable]
    public class DocumentField
    {
        #region Constructors

        /// <summary>
        /// Default for serialization
        /// </summary>
        public DocumentField() { }

        /// <summary>
        /// Easy constructor
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="itemType"></param>
        /// <param name="item"></param>
        public DocumentField(string fieldName, int itemType)
        {
            FieldName = fieldName;
            ItemType = itemType;
        }

        /// <summary>
        /// Easy constructor for string or memo
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="itemType"></param>
        /// <param name="item"></param>
        public DocumentField(string fieldName, int itemType, string item) : this(fieldName, itemType)
        {
            StringItem = item;
        }

        /// <summary>
        /// Easy constructor for int
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="itemType"></param>
        /// <param name="item"></param>
        public DocumentField(string fieldName, int itemType, int item) : this(fieldName, itemType)
        {
            IntItem = item;
        }

        /// <summary>
        /// Easy constructor for decimal
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="itemType"></param>
        /// <param name="item"></param>
        public DocumentField(string fieldName, int itemType, decimal item) : this(fieldName, itemType)
        {
            DecimalItem = item;
        }

        /// <summary>
        /// Easy constructor for date or datetime
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="itemType"></param>
        /// <param name="item"></param>
        public DocumentField(string fieldName, int itemType, DateTime item) : this(fieldName, itemType)
        {
            DateTimeItem = item;
        }

        /// <summary>
        /// Easy constructor for keyword
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="itemType"></param>
        /// <param name="item"></param>
        public DocumentField(string fieldName, int itemType, List<string> item) : this(fieldName, itemType)
        {
            KeywordItem = item;
        }

        /// <summary>
        /// Easy constructor for table
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="itemType"></param>
        /// <param name="item"></param>
        public DocumentField(string fieldName, int itemType, List<List<DocumentField>> item) : this(fieldName, itemType)
        {
            TableItem = item;
        }

        #endregion

        #region Properties

        /// <summary>
        /// DocuWare field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// DocuWare ItemChoiceType
        /// </summary>
        public int ItemType { get; set; }

        /// <summary>
        /// String or memo item
        /// </summary>
        public string StringItem { get; set; }

        /// <summary>
        /// Should string item be XML serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeStringItem() => StringItem != null;

        /// <summary>
        /// Integer item
        /// </summary>
        public int? IntItem { get; set; }

        /// <summary>
        /// Should int item be XML serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeIntItem() => IntItem.HasValue;

        /// <summary>
        /// Date or DateTime item
        /// </summary>
        public DateTime? DateTimeItem { get; set; }

        /// <summary>
        /// Should datetime item be XML serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeDateTimeItem() => DateTimeItem.HasValue;

        /// <summary>
        /// Decimal item
        /// </summary>
        public decimal? DecimalItem { get; set; }

        /// <summary>
        /// Should decimal item be XML serialized
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeDecimalItem() => DecimalItem.HasValue;

        /// <summary>
        /// Keyword item
        /// </summary>
        public List<string> KeywordItem { get; set; }

        /// <summary>
        /// Table item
        /// </summary>
        public List<List<DocumentField>> TableItem { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get basic docuware field for field
        /// </summary>
        /// <param name="dwField"></param>
        /// <returns></returns>
        public DocumentIndexField BasicDWFieldForField()
        {
            switch ((ItemChoiceType)ItemType)
            {
                case ItemChoiceType.Date:
                case ItemChoiceType.DateTime:
                    return new DocumentIndexField() { FieldName = FieldName, ItemElementName = (ItemChoiceType)ItemType, Item = DateTimeItem };
                case ItemChoiceType.Decimal:
                    return new DocumentIndexField() { FieldName = FieldName, ItemElementName = (ItemChoiceType)ItemType, Item = DecimalItem };
                case ItemChoiceType.Int:
                    return new DocumentIndexField() { FieldName = FieldName, ItemElementName = (ItemChoiceType)ItemType, Item = IntItem };
                case ItemChoiceType.Keywords:
                    return new DocumentIndexField() { FieldName = FieldName, ItemElementName = (ItemChoiceType)ItemType, Item = new DocumentIndexFieldKeywords() { Keyword = KeywordItem } };
                default:
                    return new DocumentIndexField() { FieldName = FieldName, ItemElementName = (ItemChoiceType)ItemType, Item = StringItem };
            }
        }

        /// <summary>
        /// Get standard field for basic docuware field
        /// </summary>
        /// <param name="dwField"></param>
        /// <returns></returns>
        public static DocumentField ForBasicDWField(DocumentIndexField dwField)
        {
            switch (dwField.ItemElementName)
            {
                case ItemChoiceType.Date:
                case ItemChoiceType.DateTime:
                    return new DocumentField(dwField.FieldName, (int)dwField.ItemElementName, (DateTime)dwField.Item);
                case ItemChoiceType.Decimal:
                    return new DocumentField(dwField.FieldName, (int)dwField.ItemElementName, Convert.ToDecimal(dwField.Item));
                case ItemChoiceType.Int:
                    return new DocumentField(dwField.FieldName, (int)dwField.ItemElementName, Convert.ToInt32(dwField.Item));
                case ItemChoiceType.Keywords:
                    return new DocumentField(dwField.FieldName, (int)dwField.ItemElementName, ((DocumentIndexFieldKeywords)dwField.Item).Keyword);
                default:
                    return new DocumentField(dwField.FieldName, (int)dwField.ItemElementName, (string)dwField.Item);
            }
        }

        /// <summary>
        /// Display field properties
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{FieldName} ({(ItemChoiceType)ItemType}) : {StringItem}{IntItem}{DateTimeItem}{DecimalItem}{string.Join(",", KeywordItem ?? new List<string>())}{TableItem?.Count}";
        }

        #endregion
    }

}
