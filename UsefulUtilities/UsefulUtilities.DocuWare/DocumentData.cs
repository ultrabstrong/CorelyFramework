using DocuWare.Platform.ServerClient;
using UsefulUtilities.Data.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.DocuWare
{

    [Serializable]
    public class DocumentData
    {
        #region Constructors

        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public DocumentData() { }

        /// <summary>
        /// Easy constructor
        /// </summary>
        /// <param name="docBytes"></param>
        /// <param name="mimeType"></param>
        /// <param name="dwFields"></param>
        public DocumentData(byte[] docBytes, string mimeType, List<DocumentIndexField> dwFields)
        {
            DocBytes = docBytes;
            MimeType = mimeType;
            SetFields(dwFields);
        }

        #region Properties

        #endregion

        /// <summary>
        /// Document image bytes
        /// </summary>
        public byte[] DocBytes { get; set; }

        /// <summary>
        /// Mime type for document extension
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Document index fields
        /// </summary>
        public List<DocumentField> Fields { get; set; } = new List<DocumentField>();

        #endregion

        #region Methods

        /// <summary>
        /// Set fields from DW fields
        /// </summary>
        /// <param name="dwFields"></param>
        public void SetFields(List<DocumentIndexField> dwFields)
        {
            if (dwFields != null)
            {
                // Iterate docuware fields
                foreach (DocumentIndexField dwField in dwFields)
                {
                    if (dwField.ItemElementName == ItemChoiceType.Table)
                    {
                        // Add table field
                        DocumentIndexFieldTable table = (DocumentIndexFieldTable)dwField.Item;
                        // Copy all table rows to standardized rows
                        List<List<DocumentField>> rows = new List<List<DocumentField>>();
                        foreach (DocumentIndexFieldTableRow dwRow in table.Row)
                        {
                            // Copy all table row columns to standardized columns
                            List<DocumentField> row = new List<DocumentField>();
                            foreach (DocumentIndexField dwCol in dwRow.ColumnValue)
                            {
                                row.Add(DocumentField.ForBasicDWField(dwCol));
                            }
                            rows.Add(row);
                        }
                        Fields.Add(new DocumentField(dwField.FieldName, (int)dwField.ItemElementName, rows));
                    }
                    else
                    {
                        // Add basic field
                        Fields.Add(DocumentField.ForBasicDWField(dwField));
                    }
                }
            }
        }

        /// <summary>
        /// Get dw fields from fields
        /// </summary>
        /// <returns></returns>
        public List<DocumentIndexField> GetDWFields()
        {
            List<DocumentIndexField> dwFields = new List<DocumentIndexField>();
            if (Fields != null)
            {
                // Iterate fields
                foreach (DocumentField field in Fields)
                {
                    if ((ItemChoiceType)field.ItemType == ItemChoiceType.Table)
                    {
                        // Add table field
                        DocumentIndexFieldTable dwTable = new DocumentIndexFieldTable() { Row = new List<DocumentIndexFieldTableRow>() };
                        // Copy all standardized rows to table rows
                        foreach (List<DocumentField> row in field.TableItem)
                        {
                            // Copy all standardized row columns to table columns
                            DocumentIndexFieldTableRow dwRow = new DocumentIndexFieldTableRow() { ColumnValue = new List<DocumentIndexField>() };
                            foreach (DocumentField column in row)
                            {
                                dwRow.ColumnValue.Add(column.BasicDWFieldForField());
                            }
                            dwTable.Row.Add(dwRow);
                        }
                        dwFields.Add(new DocumentIndexField()
                        {
                            FieldName = field.FieldName,
                            ItemElementName = (ItemChoiceType)field.ItemType,
                            Item = dwTable
                        });
                    }
                    else
                    {
                        // Add basic field
                        dwFields.Add(field.BasicDWFieldForField());
                    }
                }
            }
            return dwFields;
        }

        /// <summary>
        /// Serialize JSON string
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        /// <summary>
        /// Deserialize to data object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DocumentData FromJsonString(string json)
        {
            return JsonConvert.DeserializeObject<DocumentData>(json);
        }

        /// <summary>
        /// Serialize XML string
        /// </summary>
        /// <returns></returns>
        public string ToXmlString()
        {
            return XmlSerializer.Serialize(this);
        }

        /// <summary>
        /// Deserialize to data object
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static DocumentData FromXmlString(string xml)
        {
            return XmlSerializer.DeSerialize<DocumentData>(xml);
        }

        /// <summary>
        /// Post document for document
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="fileCabinetId"></param>
        /// <returns></returns>
        public Document PostDocument(ServiceConnection conn, string fileCabinetId)
        {
            // Get file cabinet
            FileCabinet fc = conn.GetFileCabinet(fileCabinetId);
            // Upload document stream
            Document newDoc = null;
            using (MemoryStream ms = new MemoryStream(DocBytes))
            {
                newDoc = fc.PostToAdvancedDocumentUploadRelationForDocument(MimeType, ms);
            }
            // Set fields on new document
            newDoc = newDoc.GetDocumentFromSelfRelation();
            newDoc.PutToFieldsRelationForDocumentIndexFields(new DocumentIndexFields() { Field = GetDWFields() });
            // Return new document
            return newDoc;
        }

        /// <summary>
        /// Post document for doc id
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="fileCabinetId"></param>
        /// <returns></returns>
        public int PostDocumentForDocid(ServiceConnection conn, string fileCabinetId)
        {
            return PostDocument(conn, fileCabinetId).Id;
        }

        #endregion
    }

}
