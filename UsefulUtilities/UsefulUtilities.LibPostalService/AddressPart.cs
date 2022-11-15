using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsefulUtilities.LibPostalService
{
    public class AddressPart
    {
        public AddressPart() { }

        public AddressPart(string _name, string _value) 
        { 
            Name = _name;
            Value = _value;
        }

        public string Name { get; set; }
        
        public string Value { get; set; }
    }
}