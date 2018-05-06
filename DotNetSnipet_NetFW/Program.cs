﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uzgoto.DotNetSnipet.Convertor;

namespace DotNetSnipet_NetFW
{
    class Program
    {
        static void Main(string[] args)
        {
            var expected = new Entity[]
            {
                new Entity(){Id=1,Name="id1",RegisterdAt=DateTime.Parse("2018-5-1"),Value=332.31m },
                new Entity(){Id=1,Name="id2",RegisterdAt=DateTime.Parse("2018-5-2"),Value=332.32m },
                new Entity(){Id=3,Name="id3",RegisterdAt=DateTime.Parse("2018-5-3"),Value=332.33m },
            };

            var sw = new Stopwatch();

            var msgs = new string[5];
            for (var i = 0; i < 5; i++)
            {
                sw.Start();
                var reader = expected.AsDataReader();
                var asDataReader = sw.Elapsed;
                var result = reader.AsEnumerable<Entity>();
                var asEnumerable = sw.Elapsed;

                var eq = expected.SequenceEqual(result);
                msgs[i] = $"{i}, {eq}, {asDataReader}, {asEnumerable}, {sw.Elapsed}";
                sw.Reset();
            }
            Array.ForEach(msgs, msg => Console.WriteLine(msg));
            Console.ReadKey();
        }
    }

    class Entity : IEquatable<Entity>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime RegisterdAt { get; set; }
        public decimal Value { get; set; }

        public bool Equals(Entity other)
        {
            return
                this.Id == other.Id &&
                this.Name == other.Name &&
                this.RegisterdAt == other.RegisterdAt &&
                this.Value == other.Value;
        }
    }
}