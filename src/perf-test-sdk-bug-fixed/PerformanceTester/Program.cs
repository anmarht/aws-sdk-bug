using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerformanceTester
{
    class Program
    {
        static IAmazonDynamoDB _dynamoDbClient;
        static Table _table;
        static string _tableName = "PerfTestTable";
        static string _credentialsPath = @"C:\Users\admin\.aws\credentials";
        static List<Task> _allTasks;
        static List<Task> _allDynamoUpdateTasks;
        static Stopwatch _stopWatch;
        static string _recordFields;
        static int _exceptionCount;

        static void Initialize()
        {
            ServicePointManager.DefaultConnectionLimit = 1000;
            Factory.Setup(true, _credentialsPath);

            _dynamoDbClient = Factory.GetDynamoDbClient();
            _table = Table.LoadTable(_dynamoDbClient, _tableName);
        }

        static string GenerateRecordFields()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 5; i++)
                sb.Append(@",""Field" + i + @""":""Field" + i + @"Value""");

            return sb.ToString();
        }

        static void WriteToDynamo(int id)
        {
            //return Task.Run(() =>
            //{
            Console.Write("UpdateItemAsync Id {0}\r", id);
            var dynamoDbRecordJson = string.Format(@"{{""Id"":""{0}""{1}}}", id, _recordFields);
            var item = Document.FromJson(dynamoDbRecordJson);
            try
            {
                _allDynamoUpdateTasks.Add(_table.UpdateItemAsync(item));
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _exceptionCount);
            }

            //});
        }

        static void Main(string[] args)
        {
            Initialize();
            _recordFields = GenerateRecordFields();

            _allTasks = new List<Task>();
            _allDynamoUpdateTasks = new List<Task>();
            _stopWatch = new Stopwatch();

            ThreadPool.SetMinThreads(100, 100);
            Console.WriteLine();
            _stopWatch.Start();
            for (var id = 0; id < 10000; id++)
            {
                WriteToDynamo(id);
            }

            // Task.WhenAll(_allTasks).Wait();
            Task.WhenAll(_allDynamoUpdateTasks).Wait();
            _stopWatch.Stop();
            Console.WriteLine();
            Console.WriteLine("Total MS = {0}, Total Ticks = {1}, Total Exceptions = {2}  ", _stopWatch.ElapsedMilliseconds, _stopWatch.ElapsedTicks, _exceptionCount);

            Console.ReadKey();

        }
    }
}
