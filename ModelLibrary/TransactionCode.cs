using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public enum TransactionType
    {
        Absence,
        PayProtection
    }
    public class TransactionCode
    {
        public string Code { get; set; }

        public TransactionType TransactionType { get; set; }
    }
}
