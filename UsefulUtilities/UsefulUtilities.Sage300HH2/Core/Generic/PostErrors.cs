using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Sage300HH2.Core.Generic
{
    [Serializable]
    public class PostErrors
    {
        public List<string> Errors { get; set; }

        public string GetErrorsString()
        {
            if(Errors == null || Errors.Count == 0) { return ""; }
            return string.Join(" | ", Errors);
        }
    }
}
