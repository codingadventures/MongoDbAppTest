using System;
using MongoRepository;

namespace Entities
{
    public class TransactionCode : Entity
    { 
        public string Code { get; set; }


        public string DisplayCode { get; set; }

        public string Description { get; set; }

        public bool IsGroundActivity { get; set; }

        public bool IsUserOverridden { get; set; }

        public int? CreatedByID { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public int? LastUpdatedByID { get; set; }

        public string LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedDateTime { get; set; }

        public int DutyAbsence { get; set; }

        public TransactionCode()
        {
        }

        public TransactionCode(string code)
        {
            Code = code;
        }

        public TransactionCode(TransactionCode transactionCodeToCopy)
        {
            Code = transactionCodeToCopy.Code;
        }


        public override string ToString()
        {
            return string.Format("{0}", Code);
        }

    }
}
