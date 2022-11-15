﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Services
{

    [Serializable]
    public enum BadRequestType
    {
        InvalidApiKey,
        InvalidConnectionId,
        InvalidParameters,
        InvalidHeaders,
        InvalidBody
    }
}
