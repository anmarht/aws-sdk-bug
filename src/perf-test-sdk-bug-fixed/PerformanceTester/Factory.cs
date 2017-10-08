using Amazon;
using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTester
{
    public static class Factory
    {
        #region Profile
        static string _credentialsPath;
        static string profileName = "default";

        class Profile
        {
            public string AccessKeyId { get; set; }
            public string SecretAccessKey { get; set; }
            public string SessionToken { get; set; }
            public string Region { get; set; }
        }

        static Profile GetAwsCredentials()
        {
            var sr = System.IO.File.OpenText(_credentialsPath);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();

                if (line.Trim() == string.Format("[{0}]", profileName))
                {
                    var profile = new Profile();
                    profile.AccessKeyId = sr.ReadLine().Trim().Split('=')[1];
                    profile.SecretAccessKey = sr.ReadLine().Trim().Split('=')[1];
                    profile.SessionToken = sr.ReadLine().Trim().Split('=')[1];
                    if (!sr.EndOfStream)
                        profile.Region = sr.ReadLine().Trim().Split('=')[1];

                    return profile;
                }
            }

            sr.Dispose();
            return null;
        }
        #endregion

        private static Profile _profile;

        private static Dictionary<string, IAmazonDynamoDB> _dynamoDbClients;
        private static object _dynamoDbclientsDictionaryLock;
        private static bool _useLocalAwsCredentials;

        public static void Setup(bool useLocalAwsCredentials = false, string credentialsPath = "")
        {
            _credentialsPath = credentialsPath;
            _useLocalAwsCredentials = useLocalAwsCredentials;
            if (_useLocalAwsCredentials)
            {
                _profile = GetAwsCredentials();

                if (_profile == null)
                    throw new Exception("Null Profile");
            }

            _dynamoDbClients = new Dictionary<string, IAmazonDynamoDB>();
            _dynamoDbclientsDictionaryLock = new object();
        }

        public static IAmazonDynamoDB GetDynamoDbClient()
        {
            var rep = RegionEndpoint.GetBySystemName(_profile.Region);
            if (_useLocalAwsCredentials)
                return new AmazonDynamoDBClient(_profile.AccessKeyId, _profile.SecretAccessKey, _profile.SessionToken, rep);
            else
                return new AmazonDynamoDBClient();
        }
    }
}
