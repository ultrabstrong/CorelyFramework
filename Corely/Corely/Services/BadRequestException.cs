using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Services
{
    [Serializable]
    public class BadRequestException : Exception
    {
        public BadRequestException(BadRequestType type) : base() { Type = type; }

        public BadRequestException(BadRequestType type, string message) : base(message) { Type = type; }

        public BadRequestException(BadRequestType type, string message, Exception innerException) : base(message, innerException) { Type = type; }

        public BadRequestType Type { get; set; }
    }
}
