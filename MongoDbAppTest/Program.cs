using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ModelLibrary;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;

namespace MongoDbAppTest
{
    class Program
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private static readonly int MaxNumberOfElements = (int)Math.Pow(10, 6);

        internal static readonly List<TransactionCode> RandomTransactionCodes =
                    (from r in Enumerable.Range(1, MaxNumberOfElements)
                     select new TransactionCode
                     {
                         ID = r,
                         Code = GenerateRandomString(6),
                         CreatedBy = "System",
                         LastUpdatedDateTime = DateTime.Now,
                         CreatedDateTime = DateTime.Now,
                         IsUserOverridden = false,
                         DutyAbsence = Random.Next(0, MaxNumberOfElements)
                     }).ToList();

        private static string GenerateRandomString(int size)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        static void Main(string[] args)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TransactionCode)))
                BsonClassMap.RegisterClassMap<TransactionCode>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIdMember(cm.GetMemberMap(c => c.ID));
                });


            var mongoDbDatabase = new MongoDbDatabase<TransactionCode>();

            mongoDbDatabase.InitializeWithRandomData(RandomTransactionCodes, "TransactionCode",
                IndexKeys<TransactionCode>
                    .Ascending(code => code.Code)
                    .Ascending(code => code.DutyAbsence));


            var firstElementInList = RandomTransactionCodes.First();
            var middleElementInList = RandomTransactionCodes.ElementAt(MaxNumberOfElements / 2);
            var lastElementInList = RandomTransactionCodes.Last();

            #region [ In-Memory Search ]

            SearchFirstElement(code => code.Code.Equals(firstElementInList.Code) && code.DutyAbsence > 10);
            SearchFirstElement(code => code.Code.Equals(lastElementInList.Code) && code.DutyAbsence > 10);
            SearchFirstElement(code => code.Code.Equals(middleElementInList.Code) && code.DutyAbsence > 10);

            #endregion

            #region [ MongoDb Indexed Search ]

            var tc1 = mongoDbDatabase.SearchFirstItem("TransactionCode", code => firstElementInList.Code.Equals(code.Code) && code.DutyAbsence > 10);
            var tc2 = mongoDbDatabase.SearchFirstItem("TransactionCode", code => lastElementInList.Code.Equals(code.Code) && code.DutyAbsence > 10);
            var tc3 = mongoDbDatabase.SearchFirstItem("TransactionCode", code => middleElementInList.Code.Equals(code.Code) && code.DutyAbsence > 10);

            #endregion

        }

        // Define other methods and classes here
        static TransactionCode SearchFirstElement(Func<TransactionCode, bool> searchFunc)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var found = RandomTransactionCodes.FirstOrDefault(searchFunc);

            stopWatch.Stop();
            Console.WriteLine("SearchElement Time  {0}:", stopWatch.ElapsedMilliseconds);

            return found;
        }

        static IEnumerable<TransactionCode> SearchSequence(Func<TransactionCode, bool> searchAction)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var found = RandomTransactionCodes.Where(searchAction).ToList();

            stopWatch.Stop();

            Console.WriteLine("SearchSequence Time: {0}", stopWatch.ElapsedMilliseconds);

            return found;

        }
    }
}
