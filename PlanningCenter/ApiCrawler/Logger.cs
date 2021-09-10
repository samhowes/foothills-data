using System;

namespace ApiCrawler
{
    public static class Logger
    {
        public static void Section(string title) => Console.WriteLine($"----- {title} -----");
        public static void Info(string message) => Console.WriteLine(message);
    }
}