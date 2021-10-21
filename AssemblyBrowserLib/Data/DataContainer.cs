using System;
using System.Collections.Generic;
using System.Text;

namespace AssemblyBrowserLib.Data
{
    public class DataContainer : MemberInfo
    {
        public List<MemberInfo> Members { get; set; }

        public DataContainer(string @namespace, string @class, string signature, List<MemberInfo> members) : base(@namespace, @class)
        {
            Signature = signature;
            Members = members;
        }
        public DataContainer(string @namespace, string @class) : base(@namespace, @class)
        {
            Members = new List<MemberInfo>();
        }
    }
}
