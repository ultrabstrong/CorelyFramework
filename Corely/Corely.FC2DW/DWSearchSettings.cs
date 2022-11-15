using DocuWare.Platform.ServerClient;
using Corely.Core;
using Corely.Logging;
using Corely.Security.Authentication;
using Corely.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using rm = Corely.FC2DW.Resources.DWSearchSettings;

namespace Corely.FC2DW
{
    public class DWSearchSettings
    {
        #region Constructor

        #endregion

        #region Properties

        /// <summary>
        /// Credentials for connecting to DW
        /// </summary>
        public GeneralCredentials Credentials { get; set; }

        /// <summary>
        /// File cabinet to search in
        /// </summary>
        public string FileCabinetGuid { get; set; }

        /// <summary>
        /// Fields and values to search for
        /// </summary>
        public NamedValues SearchValues { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Look up DocuWare document
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public ResultBase LookupDWDocument(ILogger logger = null)
        {
            // Initialize return object
            ResultBase result = new ResultBase();
            try
            {
                // Check for valid input
                logger?.WriteLog("Validating input", "", LogLevel.DEBUG);
                result.Succeeded = true;
                // Check if properties are valid
                logger?.WriteSerializedLog("Settings param", this, LogLevel.DEBUG);
                IsValid(result);
                if (result.Succeeded == false)
                {
                    // Return early if result is invalid
                    logger?.WriteLog("Parameters invalid", result.Message, LogLevel.DEBUG);
                    result.AddDataError(rm.searchCancelled);
                    return result;
                }
                else
                {
                    // Reset result and continue processing
                    logger?.WriteLog("Parameters valid", "", LogLevel.DEBUG);
                    result = new ResultBase();
                }
                // Connect to DW
                logger?.WriteLog("Connecting to DW", "", LogLevel.DEBUG);
                ServiceConnection conn = ServiceConnection.Create(
                    new Uri(Credentials.Host),
                    Credentials.Username,
                    Credentials.Password.DecryptedValue);
                // Get cabient and dialog
                logger?.WriteLog("Getting cabinet and dialog", "", LogLevel.DEBUG);
                FileCabinet cabinet = conn.GetFileCabinet(FileCabinetGuid);
                Dialog search = cabinet.GetDialogFromCustomSearchRelation();
                // Execute query
                logger?.WriteLog("Querying for document", "", LogLevel.DEBUG);
                DialogExpression dex = new DialogExpression()
                {
                    Condition = SearchValues.Select(m => DialogExpressionCondition.Create(m.Name, m.Value)).ToList(),
                    Operation = DialogExpressionOperation.And,
                    Count = int.MaxValue
                };
                DocumentsQueryResult dqr = search.GetDocumentsResult(dex);
                List<Document> docs = dqr.Items;
                // Check query results
                logger?.WriteLog($"Documents found: {dqr.Items}", "", LogLevel.DEBUG);
                if (docs.Count == 0)
                {
                    result.Message = rm.noDocsFound;
                }
                else
                {
                    // Get first document
                    Document doc = docs[0].GetDocumentFromSelfRelation();
                    // Start html div for displaying document page images
                    logger?.WriteLog($"Downloading image for document id {doc.Id}", "", LogLevel.DEBUG);
                    string docimagehtml = "<div>";
                    for (int i = 0; i < doc.Sections.Count; i++)
                    {
                        // Get section and pages in section
                        Section section = doc.Sections[i].GetSectionFromSelfRelation();
                        Pages pages = section.PostToPagesBlockRelationForPages(new PagesBlockQuery()
                        {
                            FirstPage = 0,
                            PageCount = section.PageCount
                        });
                        // Iterate pages
                        for (int j = 0; j < pages.Page.Count; j++)
                        {
                            // Get page from self relation
                            Page page = pages.Page[j].GetPageFromSelfRelation();
                            // Get low res page bytes
                            byte[] pagebytes = null;
                            using (Stream pagestream = page.GetStreamFromLowQualityImageWithAnnotationRelation())
                            using (MemoryStream pagememstream = new MemoryStream())
                            {
                                pagestream.CopyTo(pagememstream);
                                pagebytes = pagememstream.ToArray();
                            }
                            // Add page byte to document image bytes
                            string pageimg = Convert.ToBase64String(pagebytes);
                            docimagehtml += $"<img src=\"data:image/png;base64,{pageimg}\"/>";
                        }
                    }
                    // End html div for displaying document page images
                    docimagehtml += "</div>";

                    Thread thread = new Thread(() =>
                    {
                        // Create user control for displaying document
                        HtmlBase64ImageViewerUC viewer = new HtmlBase64ImageViewerUC(docimagehtml);
                        viewer.Show(true);
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    // Set result succes
                    result.Succeeded = true;
                    result.Message = rm.searchSucceeded;
                    logger?.WriteLog(result.Message, "", LogLevel.NOTICE);
                }
            }
            catch (Exception ex)
            {
                result.Message = $"{rm.searchFailed}. {ex.Message}";
                result.Exception = ex;
                logger?.WriteLog(result.Message, ex, LogLevel.ERROR);
            }
            // Return final result
            return result;
        }
        
        /// <summary>
        /// Check if this object's properties are valid
        /// </summary>
        /// <returns></returns>
        public void IsValid(ResultBase result)
        {
            // Validate connection host
            if (string.IsNullOrWhiteSpace(Credentials.Host))
            {
                result.AddDataError($"{nameof(DWSearchSettings)}.{nameof(Credentials)}.{nameof(Credentials.Host)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            // Validate connection username
            if (string.IsNullOrWhiteSpace(Credentials.Username))
            {
                result.AddDataError($"{nameof(DWSearchSettings)}.{nameof(Credentials)}.{nameof(Credentials.Username)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            // Validate connection password
            if (string.IsNullOrWhiteSpace(Credentials.Password.DecryptedValue))
            {
                result.AddDataError($"{nameof(DWSearchSettings)}.{nameof(Credentials)}.{nameof(Credentials.Password)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            // Validate file cabinet guid
            if (string.IsNullOrWhiteSpace(FileCabinetGuid))
            {
                result.AddDataError($"{nameof(DWSearchSettings)}.{nameof(FileCabinetGuid)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            // Validate search fields
            if (SearchValues == null || SearchValues.Count == 0)
            {
                result.AddDataError($"{nameof(DWSearchSettings)}.{nameof(SearchValues)} {rm.propertyError} : {rm.mustHaveOne}");
            }
        }

        #endregion
    }
}
