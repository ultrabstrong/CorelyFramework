using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UsefulUtilities.Data.Delimited
{
    public class DelimitedWriter
    {
        #region Constructors / Initialization


        /// <summary>
        /// Create as CSV reader
        /// </summary>
        public DelimitedWriter()
        {
            Setup(',', '"', Environment.NewLine);
        }

        /// <summary>
        /// Create delimited reader with pre-defined settings
        /// </summary>
        /// <param name="delim"></param>
        public DelimitedWriter(Delimiter delim)
        {
            switch (delim)
            {
                case Delimiter.Tab:
                    Setup('\t', '"', Environment.NewLine);
                    break;
                case Delimiter.CSV:
                default:
                    Setup(',', '"', Environment.NewLine);
                    break;
            }
        }

        /// <summary>
        /// Build reader with custom delimiter values
        /// </summary>
        /// <param name="_tokenDelimiter"></param>
        /// <param name="_tokenLiteral"></param>
        /// <param name="_recordDelimiter"></param>
        public DelimitedWriter(char _tokenDelimiter, char _tokenLiteral, string _recordDelimiter)
        {
            Setup(_tokenDelimiter, _tokenLiteral, _recordDelimiter);
        }

        /// <summary>
        /// Set up properties
        /// </summary>
        /// <param name="_tokenDelimiter"></param>
        /// <param name="_tokenLiteral"></param>
        /// <param name="_recordDelimiter"></param>
        private void Setup(char _tokenDelimiter, char _tokenLiteral, string _recordDelimiter)
        {
            TokenDelimiter = _tokenDelimiter;
            TokenLiteral = _tokenLiteral;
            RecordDelimiter = _recordDelimiter;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Separate record tokens with this delimiter
        /// </summary>
        public char TokenDelimiter { get; internal set; }

        /// <summary>
        /// Mark beginning / end of token literals 
        /// </summary>
        public char TokenLiteral { get; internal set; }

        /// <summary>
        /// Separate records with this delimiter
        /// </summary>
        public string RecordDelimiter { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Write token to a string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string WriteTokenToString(string token)
        {
            string tokenstring = token ?? "";
            // Escape token literal
            tokenstring.Replace(TokenLiteral.ToString(), $"{TokenLiteral}{TokenLiteral}");
            // Surround with token literal if token contains token or record delimiter
            if (tokenstring.Contains(TokenDelimiter) || tokenstring.Contains(RecordDelimiter))
            {
                tokenstring = $"{TokenLiteral}{tokenstring}{TokenLiteral}";
            }
            return tokenstring;
        }

        /// <summary>
        /// Write record to a delimited string 
        /// </summary>
        /// <param name="csvdata"></param>
        /// <returns></returns>
        public string WriteRecordToString(List<string> record)
        {
            string recordstring = "";
            // Write first token to record string
            if (record.Count > 0)
            {
                recordstring = WriteTokenToString(record[0]);
            }
            // Append all other tokens with token delimiter
            for (int i = 1; i < record.Count; i++)
            {
                recordstring += $"{TokenDelimiter}{WriteTokenToString(record[i])}";
            }
            return recordstring;
        }

        /// <summary>
        /// Write all records to a delimited string
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        public string WriteAllRecordsToString(List<List<string>> records)
        {
            string recordsstring = "";
            // Write first record to string
            if (records.Count > 0)
            {
                recordsstring = WriteRecordToString(records[0]);
            }
            // append all other records with record delimiter
            for (int i = 1; i < records.Count; i++)
            {
                recordsstring += $"{RecordDelimiter}{WriteRecordToString(records[i])}";
            }
            return recordsstring;
        }

        /// <summary>
        /// Append record as delimited string to file
        /// </summary>
        /// <param name="record"></param>
        /// <param name="filepath"></param>
        /// <param name="include_record_delim"></param>
        public void AppendRecordToFile(List<string> record, string filepath, bool include_record_delim)
        {
            // Get the record as a delimited string
            string recordstring = WriteRecordToString(record);
            // Create file info for file path
            FileInfo file = new FileInfo(filepath);
            // Create directory for file if it does not exist
            if (!file.Directory.Exists) { file.Directory.Create(); }
            // Append record to file
            if (include_record_delim)
            {
                File.AppendAllText(filepath, $"{RecordDelimiter}{recordstring}");
            }
            else
            {
                File.AppendAllText(filepath, recordstring);
            }
        }

        /// <summary>
        /// Append read record as delimited string to file
        /// </summary>
        /// <param name="record"></param>
        /// <param name="filepath"></param>
        /// <param name="include_record_delim"></param>
        public void AppendReadRecordToFile(ReadRecordResult record, string filepath, bool include_record_delim)
        {
            AppendRecordToFile(record.Tokens, filepath, include_record_delim);
        }

        /// <summary>
        /// Append all records as a delimited string to file
        /// </summary>
        /// <param name="records"></param>
        /// <param name="filepath"></param>
        public void AppendAllRecordsToFile(List<List<string>> records, string filepath)
        {
            // Append first record without record delimiter
            if (records.Count > 0)
            {
                AppendRecordToFile(records[0], filepath, false);
            }
            // Append all other records with record delimiter
            for (int i = 1; i < records.Count; i++)
            {
                AppendRecordToFile(records[i], filepath, true);
            }
        }

        /// <summary>
        /// Append all records as a delimited string to file
        /// </summary>
        /// <param name="records"></param>
        /// <param name="filepath"></param>
        public void AppendAllReadRecordsToFile(List<ReadRecordResult> records, string filepath)
        {
            // Append first record without record delimiter
            if (records.Count > 0)
            {
                AppendRecordToFile(records[0].Tokens, filepath, false);
            }
            // Append all other records with record delimiter
            for (int i = 1; i < records.Count; i++)
            {
                AppendRecordToFile(records[i].Tokens, filepath, true);
            }
        }

        #endregion
    }
}
