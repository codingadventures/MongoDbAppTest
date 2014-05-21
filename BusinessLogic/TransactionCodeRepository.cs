using System.Collections.Generic;
using Entities;
using MongoDB.Driver;
using MongoRepository;

namespace DataAccess
{
    public class TransactionCodeRepository : MongoRepository<TransactionCode>
    {
        public void InitializeWithRandomData(IEnumerable<TransactionCode> sampleData, IMongoIndexKeys indexKeys)
        { 
            Collection.Drop();

            Collection.InsertBatch(sampleData);

            Collection.CreateIndex(indexKeys);
        }
    }
}
