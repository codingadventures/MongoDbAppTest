using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDbAppTest
{
    internal class MongoDbDatabase<T> where T : class
    {
        internal const string DatabaseName = "CrewPayR&D";
        private readonly MongoServer _server = new MongoServer(new MongoServerSettings { Server = new MongoServerAddress("localhost") });
        private readonly MongoDatabase _database;
        public MongoDbDatabase()
        {
            _server.Connect();
            _database = _server.GetDatabase(DatabaseName);
        }

        internal void InitializeWithRandomData(IEnumerable<T> sampleData, string collectionName, IMongoIndexKeys indexKeys)
        {
            var collection = _database.GetCollection<T>(collectionName);

            collection.Drop();

            foreach (var data in sampleData)
            {
                collection.Save(data);
            }

            collection.CreateIndex(indexKeys);
            //collection.CreateIndex(new IndexKeysBuilder().Ascending("Code"))
            //collection.CreateIndex("Code");
        }


        internal List<T> SearchItem(string collectionName, System.Linq.Expressions.Expression<Func<T, bool>> searchFunc)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var collection = _database.GetCollection<T>(collectionName).AsQueryable<T>();

            var ret = collection.Where(searchFunc).ToList();

            stopWatch.Stop();
            Console.WriteLine("SearchItem Time {0}", stopWatch.ElapsedMilliseconds);
            return ret;
        }

        internal T SearchFirstItem(string collectionName, System.Linq.Expressions.Expression<Func<T, bool>> searchFunc)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var collection = _database.GetCollection<T>(collectionName).AsQueryable<T>();

            var ret = collection.FirstOrDefault(searchFunc);

            stopWatch.Stop();

            Console.WriteLine("SearchItem Time {0}", stopWatch.ElapsedMilliseconds);

            return ret;
        }


    }
}
