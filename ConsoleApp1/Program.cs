using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var list = new List<test>();
            list.Add(new test() { Id = 1, Name = "333" });
            list.Add(new test() { Id = 1, Name = "333" });
            list.Add(new test() { Id = 1, Name = "333" });
            list.Add(new test() { Id = 1, Name = "333" });
            list.Add(new test() { Id = 1, Name = "333" });
            list.Add(new test() { Id = 1, Name = "333" });
            list.Add(new test() { Id = 2, Name = "333" });
            list.Add(new test() { Id = 2, Name = "333" });
            list.Add(new test() { Id = 3, Name = "333" });
            list.Add(new test() { Id = 5, Name = "333" });
        }
    }

    class test
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
