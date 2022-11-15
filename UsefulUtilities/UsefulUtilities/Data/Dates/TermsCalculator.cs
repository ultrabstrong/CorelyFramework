using System;
using System.Text.RegularExpressions;

namespace UsefulUtilities.Data.Dates
{
    public class TermsCalculator
    {
        #region Constructor

        /// <summary>
        /// Initialize and calculate due date(s) from terms
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="invoicedate"></param>
        public TermsCalculator(string terms, DateTime invoicedate)
        {
            // Set varalbes
            Terms = terms;
            InvoiceDate = invoicedate;
            // Calculate due dates
            CalulateDueDates();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Terms string to calculate due date from
        /// </summary>
        public string Terms { get; internal set; }

        /// <summary>
        /// Invoice date to use for relative due date calculation
        /// </summary>
        public DateTime InvoiceDate { get; internal set; }

        /// <summary>
        /// First due date extracted from terms
        /// </summary>
        public DateTime DueDate { get; internal set; }

        /// <summary>
        /// ? Were due dates found from terms provided
        /// </summary>
        public bool DueDatesFound { get; internal set; }

        /// <summary>
        /// ? Is there a discount for early payment
        /// </summary>
        public bool HasDiscountDate { get; internal set; }

        /// <summary>
        /// Discount for early payment
        /// </summary>
        public string Discount { get; set; }

        /// <summary>
        /// Second due date extracted from terms
        /// </summary>
        public DateTime DiscountDate { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Calculate due dates from terms and invoice date
        /// </summary>
        private void CalulateDueDates()
        {
            // Create regex and match terms
            string termsregex = @"(\d{0,2}%)? *(\d{2}\w{0,2})( NET (\d{2}\w{0,2}))*";
            Regex reg = new Regex(termsregex, RegexOptions.IgnoreCase);
            Match match = reg.Match(Terms);

            // Set discount if discount found
            if (!string.IsNullOrWhiteSpace(match.Groups[1]?.Value))
            {
                Group group = match.Groups[1];
                Discount = group.Value;
                HasDiscountDate = true;
            }

            // Calculate duedate from first term
            if (!string.IsNullOrWhiteSpace(match.Groups[2]?.Value))
            {
                Group group = match.Groups[2];
                // Set discount or base date
                if (HasDiscountDate) { DiscountDate = CalcDateForString(group.Value); }
                else { DueDate = CalcDateForString(group.Value); }
                // Set flag that due dates were found
                DueDatesFound = true;
            }
            // Calculate duedate from second term
            if (!string.IsNullOrWhiteSpace(match.Groups[4]?.Value))
            {
                Group group = match.Groups[4];
                DueDate = CalcDateForString(group.Value);
            }
        }

        /// <summary>
        /// Calculate due date for trimmed terms
        /// </summary>
        /// <param name="trimmedterm"></param>
        /// <returns></returns>
        private DateTime CalcDateForString(string trimmedterm)
        {
            DateTime duedate;
            if (trimmedterm.ToLower().Contains("th"))
            {
                // Calculate due date as a day of this invoice month
                int dayOfMonth = Convert.ToInt32(trimmedterm.ToLower().Replace("th", ""));
                duedate = new DateTime(InvoiceDate.Year, InvoiceDate.Month, dayOfMonth);
                // If the calculated due date on fixed day is after relative date then set to next month
                if (duedate <= InvoiceDate)
                {
                    duedate = duedate.AddMonths(1);
                }
            }
            else
            {
                // Calculate due date as relative date
                int daysTillDue = Convert.ToInt32(trimmedterm);
                duedate = InvoiceDate.AddDays(daysTillDue);
            }
            return duedate;
        }

        #endregion

    }
}
