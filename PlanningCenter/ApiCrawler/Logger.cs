using System;
using System.Drawing;
using Console = Colorful.Console;

namespace ApiCrawler
{
    public static class Logger
    {
        public static void Section(string title) => Console.WriteLine($"----- {title} -----");
        public static void Info(string message) => Console.WriteLine(message);
        public static void Warn(string message) => Console.WriteLine(message, Color.Orange);
    }
}