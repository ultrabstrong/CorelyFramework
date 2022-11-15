using ABBYY.FlexiCapture;
using KelleyUtilities.Logging;
using KelleyUtilities.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rm = KelleyFCUtilities.Resources.GLSplitButton.GLSplit;

namespace KelleyFCUtilities.GLSplitButton
{
    public static class GLSplit
    {
        public static GLSplitResult SplitSelectedGLs(IDocument doc, GLSplitSettings settings, ILogger logger = null, Action<int, int> splitRowAction = null)
        {
            // Initialize return object
            GLSplitResult result = new GLSplitResult();
            //ProgressDisplay progress = null;
            try
            {
                logger?.WriteLog("Validating input", "", LogLevel.DEBUG);
                // Check for valid input
                result.Succeeded = true;
                if(doc == null)
                {
                    // Document is invalid
                    result.AddError($"{rm.paramNoSet}: {nameof(doc)}");
                }
                if(settings == null)
                {
                    // Settings is invalid
                    result.AddError($"{rm.paramNoSet}: {nameof(settings)}");
                }
                else
                {
                    // Settings proerpties are invalid
                    logger?.WriteSerializedLog("Settings param", settings, LogLevel.DEBUG);
                    settings.IsValid(result);
                }
                if(result.Succeeded == false)
                {
                    // Return early if result is invalid
                    logger?.WriteLog("Parameters invalid", result.Message, LogLevel.DEBUG);
                    result.AddError(rm.splittingCancelled);
                    return result;
                }
                else
                {
                    // Reset result and continue processing
                    logger?.WriteLog("Parameters valid", result.Message, LogLevel.DEBUG);
                    result = new GLSplitResult();
                }
                // Create progress display
                //progress = new ProgressDisplay(false);
                // Find the indicies of the items marked for splitting
                logger?.WriteLog("Getting split indicies", "", LogLevel.DEBUG);
                bool splitfound = false;
                startsplit:
                int curSplitIdx = -1;
                for (int i = 0; i < doc.Field("Invoice Layout\\LineItems").Items.Count; i++)
                {
                    if (doc.Field("Invoice Layout\\LineItems").Items[i].Field(settings.SplitCheckField).Text == "Yes")
                    {
                        curSplitIdx = i;
                        splitfound = true;
                        break;
                    }
                }
                //progress.Show();
                // Set error message if no line item found
                if (splitfound == false)
                {
                    result.AddError($"{rm.noInvoicesMarked}. {rm.splittingCancelled}.");
                    logger?.WriteLog(result.Message, "", LogLevel.NOTICE);
                }
                // A split line was previously found. Make sure there is a current split to process
                else if(curSplitIdx > -1)
                {
                    // Update progress display text
                    //progress.DisplayText = $"{rm.prog_splittingRow} {curSplitIdx}";
                    // Cancel split if cancel button pressed
                    //progress.cancelTokenSource.Token.ThrowIfCancellationRequested();
                    // Validate quantity               
                    logger?.WriteLog($"Getting quantity for line {curSplitIdx}", "", LogLevel.DEBUG);
                    int qty = (int)doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(settings.QuantityField).Value;
                    if (qty < 2 || qty > settings.MaxSplitQuantity)
                    {
                        // No quantity entered error
                        result.AddLineError($"{rm.line} {curSplitIdx} {rm.quantyInRange} [2, {settings.MaxSplitQuantity}]. {rm.splittingCancelled}.");
                        logger?.WriteLog(result.Message, "", LogLevel.NOTICE);
                    }
                    else
                    {
                        /*
                         * There are two different ways to calculate the new price
                         */
                        logger?.WriteLog($"Calculating new split price for line {curSplitIdx}", "", LogLevel.DEBUG);
                        // Calculate new total price from total price / qty
                        decimal totalprice = (decimal)doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(settings.TotalPriceField).Value;
                        decimal newprice = totalprice / qty;
                        // Calculate new total price from unit price * qty
                        decimal unitprice = (decimal)doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(settings.UnitPriceField).Value;
                        newprice = unitprice;
                        /*
                         * Need customer feedback to determine the best one
                         */
                        // Get list of field values to copy
                        logger?.WriteLog($"Getting copy values for line {curSplitIdx}", "", LogLevel.DEBUG);
                        List<string> copyvals = new List<string>();
                        foreach (string field in settings.CopyFields)
                        {
                            copyvals.Add(doc.Field("Invoice Layout\\LineItems").Items[curSplitIdx].Field(field).Text);
                        }
                        // Get insert index based on insert settings
                        int insertat = 0;
                        switch (settings.InsertSplitsAt)
                        {
                            case InsertSplitsAt.Bottom:
                                insertat = doc.Field("Invoice Layout\\LineItems").Items.Count;
                                break;
                            case InsertSplitsAt.Current:
                                insertat = curSplitIdx;
                                break;
                            case InsertSplitsAt.Top:
                            default:
                                break;
                        }
                        // Create [qty] new rows
                        for (int i = 0; i < qty; i++)
                        {
                            logger?.WriteLog($"Appending line {i} of {qty} for line {curSplitIdx}", "", LogLevel.DEBUG);
                            // Insert new item
                            doc.Field("Invoice Layout\\LineItems").Items.AddNew(insertat);
                            // Set fields from copied fields
                            for (int j = 0; j < settings.CopyFields.Count; j++)
                            {
                                doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(settings.CopyFields[j]).Text = copyvals[j];

                            }
                            // Set default / calculated fields
                            doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(settings.QuantityField).Text = "1";
                            doc.Field("Invoice Layout\\LineItems").Items[insertat].Field(settings.TotalPriceField).Text = newprice.ToString("F");
                            // Invoke action if it was provided
                            if(splitRowAction != null)
                            {
                                logger?.WriteLog("Invoking custom action", "", LogLevel.DEBUG);
                                splitRowAction.Invoke(i, insertat);
                            }
                        }
                        // Delete the current split item
                        logger?.WriteLog($"Deleting line {curSplitIdx}", "", LogLevel.DEBUG);
                        doc.Field("Invoice Layout\\LineItems").Items.Delete(curSplitIdx);
                        // Cancel split if cancel button pressed
                        //progress.cancelTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    // Process next split
                    goto startsplit;
                }
                else
                {
                    // Set result success
                    result.Succeeded = true;
                    result.Message = rm.splitSucceeded;
                    logger?.WriteLog(result.Message, result.LineErrors, LogLevel.NOTICE);
                }
            }
            catch(OperationCanceledException)
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
                //progress?.Close();
            }
            // Return final result
            return result;
        }
    }
}
