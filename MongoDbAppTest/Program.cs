using System.Collections.Generic;
using ModelLibrary;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDbAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
       
            var server = new MongoServer(new MongoServerSettings { Server = new MongoServerAddress("localhost") });
            server.Connect();
            var database = server.GetDatabase("CrewPay");

            var transactionCodes = database.GetCollection<TransactionCode>("TransactionCodes");
            
            transactionCodes.CreateIndex("Code");

            transactionCodes.InsertBatch(new List<TransactionCode> 
            {
                new TransactionCode{Code = "ABC", TransactionType = TransactionType.Absence},
                new TransactionCode{Code = "DEF", TransactionType = TransactionType.Absence},
                new TransactionCode{Code = "GHI", TransactionType = TransactionType.PayProtection}
            });
        }
    }
}
