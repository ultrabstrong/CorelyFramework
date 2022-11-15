using System;
using System.Collections.Generic;

namespace UsefulUtilities.Data.Delimited
{
    [Serializable]
    public class ReadRecordResult
    {
        /// <summary>
        /// Tokens in record
        /// </summary>
        public List<string> Tokens { get; set; } = new List<string>();

        /// <summary>
        /// Byte start position of record in record set
        /// </summary>
        public long StartPosition { get; set; } = 0;

        /// <summary>
        /// Byte length of record
        /// </summary>
        public long Length { get; set; } = 0;

        /// <summary>
        /// Does file have more records
        /// </summary>
        public bool HasMore { get; set; } = true;

        /// <summary>
        /// Byte end position of record in record set
        /// </summary>
        public long EndPosition => StartPosition + Length;
    }
}
