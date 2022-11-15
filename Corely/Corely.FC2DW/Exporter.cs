using Corely.Logging;
using System;
using System.Collections.Generic;
using ABBYY.FlexiCapture;
using DocuWare.Platform.ServerClient;
using System.IO;
using System.Threading;
using System.Linq;
using Corely.Services;
using Newtonsoft.Json;
using Corely.Data.Serialization;
using Corely.DocuWare;

namespace Corely.FC2DW
{
    public class Exporter
    {
        #region Constructor

        /// <summary>
        /// Initialize with all complex object defaulted
        /// </summary>
        public Exporter() { }

        #endregion

        #region Properties

        /// <summary>
        /// DW Server URL
        /// </summary>
        public string DWServer { get; set; }

        /// <summary>
        /// DW Username
        /// </summary>
        public string DWUsername { get; set; }

        /// <summary>
        /// DW Password
        /// </summary>
        public string DWPassword { get; set; }

        /// <summary>
        /// DW Token
        /// </summary>
        public string DWToken { get; set; }

        /// <summary>
        /// DW File cabinet ID
        /// </summary>
        public string DWFCID { get; set; }

        /// <summary>
        /// Connection host ID
        /// </summary>
        public string HostId { get; set; }

        /// <summary>
        /// Mapped header fields
        /// </summary>
        public Dictionary<string, string> HeaderFieldMapping { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Fixed header fields
        /// </summary>
        public Dictionary<string, string> HeaderFixedFields { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Table field mappings
        /// </summary>
        public TableMapping TableMapping { get; set; } = new TableMapping();

        /// <summary>
        /// Fixed table mapping values
        /// </summary>
        public FixedTableMapping FixedTableMapping { get; set; } = new FixedTableMapping();

        /// <summary>
        /// FC table column to DW keyword mapping
        /// </summary>
        public List<ColumnToKeywordMapping> ColumnToKeywordMapping { get; set; } = new List<ColumnToKeywordMapping>();

        /// <summary>
        /// Fixed keyword mappings
        /// </summary>
        public List<FixedKeywordMapping> FixedKeywordMappings { get; set; } = new List<FixedKeywordMapping>();

        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Upload document override event for custom upload implementation
        /// </summary>
        /// <param name="docbytes"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public delegate Document UploadDocumentOverrideDelegate(byte[] docbytes, string mimetype, List<DocumentIndexField> fields);
        public event UploadDocumentOverrideDelegate UploadDocumentOverride;

        /// <summary>
        /// Upload doucment to service override event for custom upload implementation
        /// </summary>
        /// <param name="docbytes"></param>
        /// <param name="mimetype"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public delegate ServiceResponseBase UploadDocumentToServiceOverrideDelegate(string postBody);
        public event UploadDocumentToServiceOverrideDelegate UploadDocumentToServiceOverride;

        #endregion

        #region Methods

        /// <summary>
        /// Upload FlexiCapture document to DocuWare
        /// </summary>
        /// <param name="fcDoc"></param>
        /// <param name="proc"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public int ExportDocument(IDocument fcDoc, IExportImageSavingOptions options)
        {
            try
            {
                // Get first section of Flexi document
                IField section = fcDoc.Sections[0];
                // Create new DocuWare document
                List<DocumentIndexField> newFields = new List<DocumentIndexField>();
                Logger?.WriteLog("Mapping header fields", "", LogLevel.NOTICE);
                // Create DW document fields from FC document fields
                if (HeaderFieldMapping != null)
                {
                    foreach (KeyValuePair<string, string> kvp in HeaderFieldMapping)
                    {
                        if (fcDoc.HasField(kvp.Value))
                        {
                            // Map field from FC document
                            string value = fcDoc.Field(kvp.Value).Text;
                            newFields.Add(DocumentIndexField.Create(kvp.Key, value));
                            Logger?.WriteLog($"Retrieved FlexiCapture header field [{kvp.Value}] from document", value, LogLevel.DEBUG);
                        }
                        else if (section.HasField(kvp.Value))
                        {
                            // Map field from first FC document section 
                            string value = section.Field(kvp.Value).Text;
                            newFields.Add(DocumentIndexField.Create(kvp.Key, value));
                            Logger?.WriteLog($"Retrieved FlexiCapture header field [{kvp.Value}] from section", value, LogLevel.DEBUG);
                        }
                    }
                }

                Logger?.WriteLog("Mapping fixed fields", "", LogLevel.NOTICE);
                // Create DW document fields from fixed fields
                if (HeaderFixedFields != null)
                {
                    foreach (KeyValuePair<string, string> kvp in HeaderFixedFields)
                    {
                        newFields.Add(DocumentIndexField.Create(kvp.Key, kvp.Value));
                    }
                }

                Logger?.WriteLog("Mapping table fields", "", LogLevel.NOTICE);
                // Create DW table field from FC table field
                if (TableMapping != null && TableMapping.IsValid() &&
                    (fcDoc.HasField(TableMapping.FlexiTableName) || section.HasField(TableMapping.FlexiTableName)))
                {
                    Logger?.WriteLog("Getting FlexiCapture table", "", LogLevel.DEBUG);
                    // Initialize DW table rows
                    List<DocumentIndexFieldTableRow> rows = new List<DocumentIndexFieldTableRow>();
                    // Get FC table from document or first document section
                    IField fcTable;
                    if (fcDoc.HasField(TableMapping.FlexiTableName))
                    {
                        fcTable = fcDoc.Field(TableMapping.FlexiTableName);
                        Logger?.WriteLog("Retrieved FlexiCapture table from document", "", LogLevel.DEBUG);
                    }
                    else
                    {
                        fcTable = section.Field(TableMapping.FlexiTableName);
                        Logger?.WriteLog("Retrieved FlexiCapture table from first section", "", LogLevel.DEBUG);
                    }
                    // Iterate FC rows and map table
                    foreach (IField thisRow in fcTable.Items)
                    {
                        // Create new DW row
                        DocumentIndexFieldTableRow newRow = new DocumentIndexFieldTableRow()
                        {
                            ColumnValue = new List<DocumentIndexField>()
                        };
                        // Iterate columns of this FC table row if they exist
                        if (thisRow.Children != null)
                        {
                            // Iterate mapped field names and add FC table values to DW table values if they exist
                            foreach (KeyValuePair<string, string> kvp in TableMapping.MappedColumns)
                            {
                                if (thisRow.HasField(kvp.Value))
                                {
                                    string value = thisRow.Field(kvp.Value).Text;
                                    Logger?.WriteLog($"Creating DW table column for FC field [{kvp.Value}]", value, LogLevel.DEBUG);
                                    newRow.ColumnValue.Add(DocumentIndexField.Create(kvp.Key, value));
                                }
                            }
                        }
                        rows.Add(newRow);
                    }
                    // Create DW table index field with mapped table rows
                    DocumentIndexField dwTable = new DocumentIndexField()
                    {
                        FieldName = TableMapping.DWTableName,
                        ItemElementName = ItemChoiceType.Table,
                        Item = new DocumentIndexFieldTable
                        {
                            Row = rows
                        }
                    };
                    // Add DW table to new DW Document
                    newFields.Add(dwTable);
                }
                else
                {
                    Logger?.WriteLog("No table mapping found or table defined", "", LogLevel.NOTICE);
                }

                Logger?.WriteLog("Mapping fixed table fields", "", LogLevel.NOTICE);
                // Create DW table field from fixed
                if (FixedTableMapping != null && FixedTableMapping.IsValid())
                {
                    Logger?.WriteLog("Checking if DW table was already created", "", LogLevel.DEBUG);
                    // Create DW table index field with mapped table rows
                    DocumentIndexField dwTable = null;
                    int tableIdx = newFields.FindIndex(m => m.FieldName == FixedTableMapping.DWTableName);
                    if (tableIdx > -1)
                    {
                        // Table was created by table field mapping
                        dwTable = newFields.First(m => m.FieldName == FixedTableMapping.DWTableName);
                        Logger?.WriteLog("DW table found. Appending to existing table rows", "", LogLevel.DEBUG);
                    }
                    else
                    {
                        // Table doesn't exist yet. Create new
                        dwTable = new DocumentIndexField()
                        {
                            FieldName = FixedTableMapping.DWTableName,
                            ItemElementName = ItemChoiceType.Table,
                            Item = new DocumentIndexFieldTable
                            {
                                Row = new List<DocumentIndexFieldTableRow>()
                            }
                        };
                        Logger?.WriteLog("DW table not found. Adding to new table", "", LogLevel.DEBUG);
                    }
                    // Iterate fixed rows and map table
                    foreach (Dictionary<string, string> row in FixedTableMapping.FixedRowColumnMapping)
                    {
                        // Create new DW row
                        DocumentIndexFieldTableRow newRow = new DocumentIndexFieldTableRow()
                        {
                            ColumnValue = new List<DocumentIndexField>()
                        };
                        // Iterate mapped columns and add to column value
                        foreach (KeyValuePair<string, string> kvp in row)
                        {
                            Logger?.WriteLog("Creating fixed DW table column", kvp.Key, LogLevel.DEBUG);
                            newRow.ColumnValue.Add(DocumentIndexField.Create(kvp.Key, kvp.Value));
                        }
                        // Add new row to table field
                        ((DocumentIndexFieldTable)dwTable.Item).Row.Add(newRow);
                    }
                    if (tableIdx > -1)
                    {
                        // Update existing DW table field with newly added rows
                        newFields[tableIdx] = dwTable;
                    }
                    else
                    {
                        // Add DW table to new DW Document
                        newFields.Add(dwTable);
                    }
                }
                else
                {
                    Logger?.WriteLog("No fixed table mapping found", "", LogLevel.NOTICE);
                }

                Logger?.WriteLog("Mapping keyword fields", "", LogLevel.NOTICE);
                // Create DW keyword field from FC table column
                if (ColumnToKeywordMapping != null && ColumnToKeywordMapping.Count > 0)
                {
                    foreach (ColumnToKeywordMapping keywrdMapping in ColumnToKeywordMapping)
                    {
                        if (keywrdMapping != null && keywrdMapping.IsValid() &&
                            (fcDoc.HasField(keywrdMapping.FlexiTableName) || section.HasField(keywrdMapping.FlexiTableName)))
                        {
                            Logger?.WriteLog("Getting FlexiCapture table", "", LogLevel.DEBUG);
                            // Initialize DW keywords list object
                            DocumentIndexFieldKeywords keywords = new DocumentIndexFieldKeywords() { Keyword = new List<string>() };
                            // Get FC table from document or first document section
                            IField fcTable;
                            if (fcDoc.HasField(keywrdMapping.FlexiTableName))
                            {
                                fcTable = fcDoc.Field(keywrdMapping.FlexiTableName);
                                Logger?.WriteLog("Retrieved FlexiCapture table from document", "", LogLevel.DEBUG);
                            }
                            else
                            {
                                fcTable = section.Field(keywrdMapping.FlexiTableName);
                                Logger?.WriteLog("Retrieved FlexiCapture table from first section", "", LogLevel.DEBUG);
                            }
                            // Iterate FC rows and map table
                            foreach (IField thisRow in fcTable.Items)
                            {
                                // If this row has the column for the keyword then add it to the keyword value
                                if (thisRow.HasField(keywrdMapping.FlexiTableColumn))
                                {
                                    Logger?.WriteLog("Adding keyword value for FC field", "", LogLevel.DEBUG);
                                    keywords.Keyword.Add(thisRow.Field(keywrdMapping.FlexiTableColumn).Text);
                                }
                            }
                            // Create new DW keyword field
                            DocumentIndexField keywordField = DocumentIndexField.Create(keywrdMapping.KeywordFieldName, keywords);
                            // Add DW keyword to new DW Document
                            newFields.Add(keywordField);
                        }
                        else
                        {
                            Logger?.WriteLog("Keyword mapping found or table defined", "", LogLevel.NOTICE);
                        }
                    }
                }
                else
                {
                    Logger?.WriteLog("No keyword mappings found", "", LogLevel.NOTICE);
                }


                Logger?.WriteLog("Mapping fixed keyword fields", "", LogLevel.NOTICE);
                // Create fixed DW keyword field
                if (FixedKeywordMappings != null && FixedKeywordMappings.Count > 0)
                {
                    foreach (FixedKeywordMapping keywrdMapping in FixedKeywordMappings)
                    {
                        if (keywrdMapping != null && keywrdMapping.IsValid())
                        {
                            // Create new DW keyword field
                            DocumentIndexField keywordField = DocumentIndexField.Create(keywrdMapping.KeywordFieldName, new DocumentIndexFieldKeywords() { Keyword = keywrdMapping.Keywords });
                            // Add DW keyword to new DW Document
                            newFields.Add(keywordField);
                        }
                        else
                        {
                            Logger?.WriteLog("Fixed keyword mapping not valid", "", LogLevel.NOTICE);
                        }
                    }
                }
                else
                {

                    Logger?.WriteLog("No fixed keyword mappings found", "", LogLevel.NOTICE);
                }
                Logger?.WriteLog("Getting document bytes", "", LogLevel.NOTICE);
                byte[] docBytes = fcDoc.SaveAsStream(options);
                string mimeType = FormatToMimeType(options.Format);
                // Upload new DW document
                Document newDoc = null;
                if(UploadDocumentToServiceOverride != null)
                {
                    // Upload to service endpoint so service can post to DW
                    Logger?.WriteLog("Uploading new document to DocuWare with upload to service override", "", LogLevel.NOTICE);
                    // Get XML or JSON body
                    DocumentData data = new DocumentData(docBytes, mimeType, newFields);
                    ServiceResponseBase response = UploadDocumentToServiceOverride(data.ToJsonString());
                    if(response.Status == 200)
                    {
                        Logger?.WriteLog("Upload successful", "", LogLevel.NOTICE);
                        int.TryParse(response.Data, out int docid);
                        return docid;
                    }
                    else
                    {
                        throw new Exception($"{response.Status} : {response.Message}{Environment.NewLine}{response.Data}");
                    }
                }
                else if (UploadDocumentOverride != null)
                {
                    // Upload with override
                    Logger?.WriteLog("Uploading new document to DocuWare with upload override", "", LogLevel.NOTICE);
                    newDoc = UploadDocumentOverride(docBytes, mimeType, newFields);
                }
                else
                {
                    // Upload with default logic
                    Logger?.WriteLog("Uploading new document to DocuWare with default upload", "", LogLevel.NOTICE);
                    newDoc = UploadToDW(docBytes, mimeType, newFields);
                }
                Logger?.WriteLog("Upload successful", "", LogLevel.NOTICE);
                // Return new DW document ID
                return newDoc.Id;
            }
            catch (Exception ex)
            {
                // Log error and throw
                Logger?.WriteLog("Failed to map fields and upload document", ex, LogLevel.ERROR);
                throw;
            }
        }

        /// <summary>
        /// Upload new DW document to DocuWare
        /// </summary>
        /// <param name="newFields"></param>
        /// <param name="fcDoc"></param>
        /// <param name="dwServer"></param>
        /// <param name="fcid"></param>
        /// <param name="dwUser"></param>
        /// <param name="dwPass"></param>
        /// <returns></returns>
        private Document UploadToDW(byte[] docbytes, string mimetype, List<DocumentIndexField> newFields)
        {
            Logger?.WriteLog("Connecting to DocuWare", "", LogLevel.NOTICE);
            // Create connection to DW for platform license
            ServiceConnection dw = null;
            if (!string.IsNullOrWhiteSpace(HostId))
            {
                ServiceConnection.SetHostId(HostId);
            }
            try
            {
                if (string.IsNullOrWhiteSpace(DWToken))
                {
                    dw = ServiceConnection.Create(new Uri($"{DWServer}/DocuWare/Platform"), DWUsername, DWPassword, licenseType: DWProductTypes.PlatformService);
                }
                else
                {
                    dw = ServiceConnection.Create(new Uri($"{DWServer}/DocuWare/Platform"), DWToken, licenseType: DWProductTypes.PlatformService);
                }
                // Get file cabinet
                FileCabinet fc = dw.GetFileCabinet(DWFCID);
                // Upload document stream
                Logger?.WriteLog("Uploading document", "", LogLevel.NOTICE);
                Document newDoc = null;
                using (MemoryStream ms = new MemoryStream(docbytes))
                {
                    newDoc = fc.PostToAdvancedDocumentUploadRelationForDocument(mimetype, ms);
                }
                // Set fields on new document
                Logger?.WriteLog("Setting index data", "", LogLevel.NOTICE);
                newDoc = newDoc.GetDocumentFromSelfRelation();
                newDoc.PutToFieldsRelationForDocumentIndexFields(new DocumentIndexFields() { Field = newFields });
                // Return new document
                return newDoc;
            }
            finally
            {
                try { dw?.Disconnect(); } catch { };
            }
        }

        /// <summary>
        /// Convert FC format to content mime type
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string FormatToMimeType(string format)
        {
            switch (format.Replace(" ", "").ToLower())
            {
                case "tif":
                    return "image/tiff";
                case "jpg":
                case "jpg2000":
                    return "image/jpeg";
                case "png":
                    return "image/png";
                case "pcx":
                case "bmp":
                    return "image/bmp";
                case "pdf":
                case "pdf-a":
                case "pdf-s":
                case "pdf-s-a":
                case "pdf-a-s":
                default:
                    return "application/pdf";
            }
        }

        #endregion
    }
}
