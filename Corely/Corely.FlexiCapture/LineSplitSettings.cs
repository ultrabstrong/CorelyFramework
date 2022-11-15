using ABBYY.FlexiCapture;
using Corely.Core;
using Corely.Data.Serialization;
using Corely.Distribution;
using Corely.Logging;
using Corely.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using rm = Corely.FlexiCapture.Resources.LineSplitSettings;

namespace Corely.FlexiCapture
{
    public class LineSplitSettings
    {
        #region Contstructor

        /// <summary>
        /// Construct this object
        /// </summary>
        public LineSplitSettings() { }

        #endregion

        #region Properties

        /// <summary>
        /// Location to insert split rows
        /// </summary>
        public DistributionInsertAt DistributionInsertAt { get; set; } = DistributionInsertAt.Bottom;

        /// <summary>
        /// Names of line item fields to copy
        /// </summary>
        public CopyFields CopyFields { get; set; }

        /// <summary>
        /// Additional fields to distribute (must be numeric fields)
        /// </summary>
        public AdditionalDistributeFields AdditionalDistributeFields
        {
            get => _additionalDistributeFields;
            set
            {
                if(value != null) { _additionalDistributeFields = value; }
            }
        }
        private AdditionalDistributeFields _additionalDistributeFields = new AdditionalDistributeFields();

        /// <summary>
        /// Field to check for splitting
        /// </summary>
        public string SplitCheckField { get; set; }

        /// <summary>
        /// Name of line item total price field
        /// </summary>
        public string TotalPriceField { get; set; }

        /// <summary>
        /// Name of line item unit price field
        /// </summary>
        public string UnitPriceField { get; set; }

        /// <summary>
        /// Name of line item quantity field
        /// </summary>
        public string QuantityField { get; set; }

        /// <summary>
        /// Field to save distribution XML to
        /// </summary>
        public string DistributionXmlField { get; set; }

        /// <summary>
        /// Split using total
        /// </summary>
        public bool SplitOnTotal { get; set; }

        /// <summary>
        /// Split distribtuion settings for calcuating quantity distribution
        /// </summary>
        public DistributionSettings DistributionSettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Show split distributions window and run selected settings
        /// </summary>
        /// <param name="settingsName"></param>
        /// <param name="doc"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        /// <param name="splitRowAction"></param>
        /// <returns></returns>
        public ResultBase ShowDistributionSettingsFromProjectAndSplit(string settingsName, IDocument doc, ILogger logger = null, Action<int, int> splitRowAction = null)
        {
            ResultBase result = new ResultBase();
            try
            {
                // Get settings from UI
                DistributionSettings distributionSettings = ShowDistributionSettingsFromProject(doc.Batch.Project, settingsName);
                // Set distribution settings
                DistributionSettings = distributionSettings;
                // Return for selected settings
                return SplitSelectedLines(doc, logger, splitRowAction);
            }
            catch (OperationCanceledException)
            {
                result.Succeeded = true;
            }
            catch (InvalidCastException)
            {
                // Set result for empty settings
                result.Message = $"{rm.noSettingsSelected}. {rm.splittingCancelled}";
            }
            catch (Exception ex)
            {
                // Set error result
                logger?.WriteLog(result.Message, ex, LogLevel.ERROR);
                result.Message = $"{rm.settingsSelectionFailed}. {ex.Message}";
                result.Exception = ex;
            }
            return result;
        }

        /// <summary>
        /// Show distribution settings from file
        /// </summary>
        /// <param name="settingsFilePath"></param>
        /// <returns></returns>
        public DistributionSettings ShowDistributionSettingsFromProject(IProject project, string settingsName)
        {
            // Get settings from project
            string settingsXml = project.EnvironmentVariables.Get(settingsName);
            List<DistributionSettings> settings = XmlSerializer.DeSerialize<List<DistributionSettings>>(settingsXml);
            // Create settings load function
            List<DistributionSettings> loadSettings() => settings;
            // Show settings and return result
            return ShowDistributionSettings(loadSettings, null);
        }

        /// <summary>
        /// Show distribution
        /// </summary>
        /// <param name="loadSettings"></param>
        /// <param name="saveSettings"></param>
        /// <returns></returns>
        public DistributionSettings ShowDistributionSettings(Func<List<DistributionSettings>> loadSettings, Action<List<DistributionSettings>> saveSettings)
        {
            // Show distributions window
            Distributions distributionsWindow = new Distributions(loadSettings, saveSettings);
            distributionsWindow.ShowDialog();
            // Throw cancelled exception if user cancelled
            if (distributionsWindow.Cancelled)
            {
                throw new OperationCanceledException();
            }
            DistributionSettings settings = distributionsWindow.GetSelectedSettings();
            // Throw exception if nothing selected
            if (settings == null)
            {
                throw new InvalidDataException();
            }
            // Return settings
            return settings;
        }

        /// <summary>
        /// Split selected lines for setting
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        /// <param name="splitRowAction"></param>
        /// <returns></returns>
        public ResultBase SplitSelectedLines(IDocument doc, ILogger logger = null, Action<int, int> splitRowAction = null)
        {
            // Initialize return object
            ResultBase result = new ResultBase();
            ProgressDisplay progress = null;
            List<IField> deleterows = new List<IField>();
            try
            {
                logger?.WriteLog("Validating input", "", LogLevel.DEBUG);
                // Check for valid input
                result.Succeeded = true;
                if (doc == null)
                {
                    // Document is invalid
                    result.AddDataError($"{rm.paramNoSet}: {nameof(doc)}");
                }
                // Check if properties are valid
                logger?.WriteSerializedLog("Settings param", this, LogLevel.DEBUG);
                IsValid(result);
                if (result.Succeeded == false)
                {
                    // Return early if result is invalid
                    logger?.WriteLog("Parameters invalid", result.Message, LogLevel.DEBUG);
                    result.AddDataError(rm.splittingCancelled);
                    return result;
                }
                else
                {
                    // Reset result and continue processing
                    logger?.WriteLog("Parameters valid", result.Message, LogLevel.DEBUG);
                    result = new ResultBase();
                }
                // Find the number of the items marked for splitting
                logger?.WriteLog("Getting split count", "", LogLevel.DEBUG);
                List<int> allSplitLineNums = new List<int>();
                // Get number of rows marked for splitting
                for (int i = 0; i < doc.Field("Invoice Layout\\LineItems").Items.Count; i++)
                {
                    if (doc.Field("Invoice Layout\\LineItems").Items[i].Field(SplitCheckField).Text == "Yes")
                    {
                        allSplitLineNums.Add(i + 1);
                    }
                }
                // Set error message if no line item found
                if (allSplitLineNums.Count == 0)
                {
                    result.AddDataError($"{rm.noInvoicesMarked}. {rm.splittingCancelled}.");
                    logger?.WriteLog(result.Message, "", LogLevel.NOTICE);
                }
                else
                {
                    // Serialize distribution XML so it doesn't need to be on every split
                    string distXml = XmlSerializer.Serialize(DistributionSettings);
                    // Initialize progress windows and splits handled
                    progress = AsyncWindow.ShowAsync<ProgressDisplay>();
                    int splitshandled = -1;
                startsplit:
                    int curSplitIdx = -1;
                    // Find the index of the next item marked for splitting
                    logger?.WriteLog("Getting next split index", "", LogLevel.DEBUG);
                    for (int i = 0; i < doc.Field("Invoice Layout\\LineItems").Items.Count; i++)
                    {
                        if (doc.Field("Invoice Layout\\LineItems").Items[i].Field(SplitCheckField).Text == "Yes")
                        {
                            curSplitIdx = i;
                            splitshandled++;
                            break;
                        }
                    }
                    // Make sure there is a current split to process
                    if (curSplitIdx > -1)
                    {
                        // Update progress display text
                        progress?.SetDisplayText($"{rm.prog_splittingRow} {allSplitLineNums[splitshandled]} / {allSplitLineNums.Count}");
                        // Cancel split if cancel button pressed
                        progress?.ThrowIfCancellationRequested();
                        // Validate value               
                        logger?.WriteLog($"Getting value for line {allSplitLineNums[splitshandled]}", "", LogLevel.DEBUG);
                        decimal value = (decimal)doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(SplitOnTotal ? TotalPriceField : QuantityField).Value;
                        if (DistributionSettings.DistributionType != DistributionType.Fixed &&
                            !DistributionSettings.SetValueToOne &&
                            (value < DistributionSettings.MinValue ||
                            value > DistributionSettings.MaxValue))
                        {
                            // Value out of range error
                            result.AddDataError($"{rm.line} {allSplitLineNums[splitshandled]} {rm.valueInRange} [{DistributionSettings.MinValue}, {DistributionSettings.MaxValue}]. {rm.splittingCancelled}.");
                            logger?.WriteLog(result.Message, "", LogLevel.NOTICE);
                            // Remove check mark from invalid split line
                            doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(SplitCheckField).Value = false;
                        }
                        else
                        {
                            logger?.WriteLog($"Calculating new split price for line {allSplitLineNums[splitshandled]}", "", LogLevel.DEBUG);
                            // Get unit price
                            decimal unitprice = 0.0M;
                            if (!string.IsNullOrWhiteSpace(doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(UnitPriceField).Text))
                            {
                                unitprice = (decimal)doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(UnitPriceField).Value;
                            }
                            // Get list of field values to copy
                            logger?.WriteLog($"Getting copy values for line {allSplitLineNums[splitshandled]}", "", LogLevel.DEBUG);
                            foreach (CopyField field in CopyFields)
                            {
                                if (!string.IsNullOrWhiteSpace(field.CopyFrom))
                                {
                                    field.CopyValue = doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(field.CopyFrom).Text;
                                }
                            }
                            // Get insert index based on insert settings
                            int insertat = doc.Field("Invoice Layout\\LineItems").Items.Count;
                            switch (DistributionInsertAt)
                            {
                                case DistributionInsertAt.Top:
                                    insertat = 0;
                                    break;
                                case DistributionInsertAt.Current:
                                    insertat = curSplitIdx;
                                    break;
                                case DistributionInsertAt.Bottom:
                                default:
                                    break;
                            }
                            // Get row to delete before re-arranging the table indicies
                            IField deleterow = doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx];
                            // Get distribution for value
                            List<decimal> distributions = DistributionSettings.GetDistribtuion(value);
                            // Get additional value distributions
                            foreach (AdditionalDistributeField field in AdditionalDistributeFields)
                            {
                                decimal.TryParse(doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(field.FieldName).Text, out decimal additionalValue);
                                if(additionalValue != 0.0m) 
                                { 
                                    field.Distributions = DistributionSettings.GetDistribtuion(additionalValue); 
                                }
                            }
                            // Create new rows
                            for (int i = 0; i < distributions.Count; i++)
                            {
                                logger?.WriteLog($"Inserting line {i + 1} of {distributions.Count} for line {allSplitLineNums[splitshandled]}", "", LogLevel.DEBUG);
                                progress?.SetDisplayText($"{rm.prog_splittingRow} {splitshandled + 1} / {allSplitLineNums.Count} [{i + 1}/{distributions.Count}]");
                                // Insert new item
                                doc.Field("Invoice Layout\\LineItems").Items.AddNew(insertat);
                                // Set fields from copied fields
                                foreach(CopyField field in CopyFields)
                                {
                                    doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(field.CopyTo).Text = field.CopyValue;
                                }
                                // Get additional values to use in row calculation
                                decimal additionalValue = AdditionalDistributeFields.Sum(m => m.GetCalcDist(i));
                                // Set default / calculated fields
                                if (SplitOnTotal)
                                {
                                    // Distribute total and calculate quantity
                                    doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(TotalPriceField).Text = (distributions[i]).ToString("F");
                                    if(unitprice != 0.0M)
                                    {
                                        decimal newqty = distributions[i] - additionalValue / unitprice;
                                        doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(QuantityField).Text = newqty.ToString("F");
                                    }
                                }
                                else
                                {
                                    // Distribute quantity and calculate total
                                    doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(QuantityField).Text = distributions[i].ToString();
                                    doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(TotalPriceField).Text = (distributions[i] * unitprice + additionalValue).ToString("F");
                                }
                                // Set additional distribution fields
                                foreach(AdditionalDistributeField field in AdditionalDistributeFields)
                                {
                                    if(field.Distributions != null)
                                    {
                                        doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(field.FieldName).Text = field.GetDist(i).ToString("F");
                                    }
                                }
                                // Save distribution XML to field
                                if(!string.IsNullOrWhiteSpace(DistributionXmlField))
                                {
                                    doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(DistributionXmlField).Text = distXml;
                                }
                                // Invoke action if it was provided
                                if (splitRowAction != null)
                                {
                                    logger?.WriteLog($"Invoking custom action for split line {i + 1}", "", LogLevel.DEBUG);
                                    splitRowAction.Invoke(curSplitIdx, insertat);
                                }
                            }
                            // Delete the current row
                            logger?.WriteLog($"Deleting line {allSplitLineNums[splitshandled]}", "", LogLevel.DEBUG);
                            doc.Field("Invoice Layout\\LineItems").Items.Delete(deleterow);
                            // Cancel split if cancel button pressed
                            progress?.ThrowIfCancellationRequested();
                        }
                        // Process next split
                        goto startsplit;
                    }
                    else
                    {
                        // Set result success
                        result.Succeeded = true;
                        result.Message = rm.splitSucceeded;
                        logger?.WriteLog(result.Message, result.GetResultString(), LogLevel.NOTICE);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                logger?.WriteLog(rm.splitCancelledByUser, "", LogLevel.DEBUG);
                result.Message = rm.splitCancelledByUser;
            }
            catch (Exception ex)
            {
                logger?.WriteLog(result.Message, ex, LogLevel.ERROR);
                result.Message = $"{rm.splitFailed}. {ex.Message}";
                result.Exception = ex;
            }
            finally
            {
                AsyncWindow.CloseAsync(progress);
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
            // Validate copy fields
            if (CopyFields == null || CopyFields.Count == 0)
            {
                result.AddDataError($"{nameof(LineSplitSettings)}.{nameof(CopyFields)} {rm.propertyError} : {rm.mustHaveOne}");
            }
            // Validate split check
            if (string.IsNullOrWhiteSpace(SplitCheckField))
            {
                result.AddDataError($"{nameof(LineSplitSettings)}.{nameof(SplitCheckField)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            // Validate total price field
            if (string.IsNullOrWhiteSpace(TotalPriceField))
            {
                result.AddDataError($"{nameof(LineSplitSettings)}.{nameof(TotalPriceField)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            // Validate unit price field
            if (string.IsNullOrWhiteSpace(UnitPriceField))
            {
                result.AddDataError($"{nameof(LineSplitSettings)}.{nameof(UnitPriceField)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            // Validate quantity field
            if (string.IsNullOrWhiteSpace(QuantityField))
            {
                result.AddDataError($"{nameof(LineSplitSettings)}.{nameof(QuantityField)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            // Validate split distribution settings
            if (DistributionSettings == null)
            {
                result.AddDataError($"{nameof(LineSplitSettings)}.{nameof(DistributionSettings)} {rm.propertyError} : {rm.cannotBeEmpy}");
            }
            else
            {
                // Validate the split dist settings
                DistributionSettings.IsValid(result);
            }
        }

        #endregion
    }
}
