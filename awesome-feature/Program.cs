using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using FizzWare.NBuilder;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to db");
            var cluster = Cluster.Builder().AddContactPoints("146.185.168.95").Build();
            var session = cluster.Connect();
            var repl = new Dictionary<string, string>
            {
                {"class", "NetworkTopologyStrategy" },
                {"DC1", "3" },
                {"replicationfactor", "3" }
            };
            session.DeleteKeyspaceIfExists("test");

            session.CreateKeyspaceIfNotExists("test", repl, true);
            session.ChangeKeyspace("test");

            MappingConfiguration.Global.Define(new Map<User>().TableName("users")
                                                .PartitionKey(u => u.Username)
                                                .ClusteringKey(u => u.CreatedAt)
                                                .Column(u => u.Username, cm => cm.WithName("username"))
                                                .Column(u => u.FullName, cm => cm.WithName("fullName"))
                                                .Column(u => u.CreatedAt, cm => cm.WithName("createdAt"))
                                                .Column(u => u.Email, cm => cm.WithName("email"))
                                                );

            var users = new Table<User>(session);
            users.CreateIfNotExists();
            users.SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
            var list = users.Execute().ToList();
            Console.WriteLine("Read " + list.Count);

            Console.WriteLine("Building entries");
            var elements = Builder<User>.CreateListOfSize(10000).All().
                                With(x => x.Username = Faker.NameFaker.Name())
                                .With(x => x.Email = Faker.InternetFaker.Email())
                                .With(x => x.FullName = Faker.NameFaker.FirstName() + " " + Faker.NameFaker.LastName())
                                .With(x => x.CreatedAt = Faker.DateTimeFaker.DateTime())
                                .Build();

            Console.WriteLine("Starting to write");
            var sw = new Stopwatch();
            sw.Start();

            foreach (var item in elements)
            {
                var cqli = users.Insert(item).SetConsistencyLevel(ConsistencyLevel.LocalQuorum);
                cqli.Execute();
            }

            sw.Stop();

            Console.WriteLine("All done in " + sw.Elapsed.TotalSeconds);
            Console.ReadLine();
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public override string ToString()
        {
            return FullName;
        }
    }
}
