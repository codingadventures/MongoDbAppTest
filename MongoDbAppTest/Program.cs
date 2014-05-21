using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using DataAccess;
using Entities;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

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

        private static readonly TransactionCodeRepository TransactionCodeRepo = new TransactionCodeRepository();

        static void Main(string[] args)
        {
            TransactionCodeRepo.InitializeWithRandomData(RandomTransactionCodes,
               IndexKeys<TransactionCode>
                   .Ascending(code => code.Code)
                   .Ascending(code => code.DutyAbsence));

            var firstElementInList = RandomTransactionCodes.First();
            var middleElementInList = RandomTransactionCodes.ElementAt(MaxNumberOfElements / 2);
            var lastElementInList = RandomTransactionCodes.Last();
            var dutyAbsenceValue = Random.Next(0, MaxNumberOfElements / 8);

            #region [ In-Memory Search ]

            SearchElementInMemory(code => code.Code.Equals(firstElementInList.Code) && code.DutyAbsence > dutyAbsenceValue);
            SearchElementInMemory(code => code.Code.Equals(lastElementInList.Code) && code.DutyAbsence > dutyAbsenceValue);
            SearchElementInMemory(code => code.Code.Equals(middleElementInList.Code) && code.DutyAbsence > dutyAbsenceValue);

            #endregion

            #region [ MongoDb Indexed Search ]

            SearchElementInDatabase(new List<IMongoQuery> 
            {
               Query<TransactionCode>.EQ(code => code.Code,firstElementInList.Code),
               Query<TransactionCode>.GT(code => code.DutyAbsence,dutyAbsenceValue)
            });
            SearchElementInDatabase(new List<IMongoQuery> 
            {
               Query<TransactionCode>.EQ(code => code.Code,lastElementInList.Code),
               Query<TransactionCode>.GT(code => code.DutyAbsence,dutyAbsenceValue)
            });

            SearchElementInDatabase(new List<IMongoQuery> 
            {
               Query<TransactionCode>.EQ(code => code.Code,middleElementInList.Code),
               Query<TransactionCode>.GT(code => code.DutyAbsence,dutyAbsenceValue)
            });

            #endregion
            Console.WriteLine("Press a key to terminate...");
            Console.ReadLine();
        }

        // Define other methods and classes here
        static void SearchElementInMemory(Func<TransactionCode, bool> searchFunc)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var found = RandomTransactionCodes.Where(searchFunc).ToList();

            stopWatch.Stop();

            Console.WriteLine("SearchElement Time  {0}:", stopWatch.ElapsedMilliseconds);
        }

        static void SearchElementInDatabase(IEnumerable<IMongoQuery> query)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var found = TransactionCodeRepo.Collection.Find(Query.And(query));

            stopWatch.Stop();

            Console.WriteLine("SearchSequence Time: {0}", stopWatch.ElapsedMilliseconds);
        }
    }
}
