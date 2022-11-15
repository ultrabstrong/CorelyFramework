using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Core
{
    [Serializable]
    public class MailSettings
    {
        public string SMTPServer { get; set; }

        public string SMTPUsername { get; set; }

        public string SMTPPw { get; set; }

        public int SMTPPort { get; set; }

        public string SMTPTo { get; set; }
    }
}
