using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace PlaceholderGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args.FirstOrDefault();

            //If there is no file - notify user and create empty template
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Console.WriteLine("Please provide file name of placeholder(s) data file (CSV).");
                var txt = new List<string>
                {
                    "ImageName (no ext),Text,TextSize (font size),TxtColor (ffffff)," +
                    "Width  (int),Height (int),BgColor (ffffff),Format (png)",
                    string.Empty
                };
                File.WriteAllLines("example.csv", txt);

                Console.WriteLine("===================");
                Console.WriteLine("Completed. Press any key to exit.");
                Console.ReadKey();
            }
            else
            {
                var lines = File.ReadAllLines(fileName);
                var placeholderList = new List<Placeholder>();
                foreach (var line in lines.Skip(1)) placeholderList.Add(Map(line));
                var folder = fileName.Split('.')[0].ToUpper();
                Directory.CreateDirectory(folder);
                foreach (var placeholder in placeholderList)
                {
                    Console.Write($"Downloading {placeholder.ImageName}.{placeholder.Format} ...");
                    DownloadAndSavePlaceholder(folder, placeholder);
                    Console.WriteLine("Saved!");
                }

                Console.WriteLine("===================");
                Console.WriteLine("Completed. Press any key to exit.");
                Console.ReadKey();
                Process.Start(folder);
            }
        }

        #region HELP METHODS AND CLASSES
        private static void DownloadAndSavePlaceholder(string folder, Placeholder p)
        {
            var path = Path.Combine(folder, $"{p.ImageName}.{p.Format}");
            var url = GetUrl(p.TextSize, p.BgColor, p.TxtColor, p.Text, p.Width, p.Height, p.Format);
            using (var client = new WebClient()) client.DownloadFile(new Uri(url), path);
        }

        private static Placeholder Map(string line)
        {
            var fields = line.Split(',');
            return new Placeholder()
            {
                ImageName = fields[0],
                Text = fields[1],
                TextSize = fields[2],
                TxtColor = fields[3],
                Width = fields[4],
                Height = fields[5],
                BgColor = fields[6],
                Format = fields[7],
            };
        }

        private static string GetUrl(string textSize, string bgColor, string txtColor,
            string text, string width, string height, string format)
        {
            return    "https://placeholdit.imgix.net/~text" +
                      $"?txtsize={textSize}" +
                      $"&bg={bgColor}" +
                      $"&txtclr={txtColor}" +
                      $"&txt={(text.Replace(" ", "+"))}" +
                      $"&w={width}" +
                      $"&h={height}" +
                      $"&fm={format}";
        }

        private class Placeholder
        {
            public string ImageName { get; set; }
            public string Text { get; set; }
            public string TextSize { get; set; }
            public string TxtColor { get; set; }
            public string Width { get; set; }
            public string Height { get; set; }
            public string BgColor { get; set; }
            public string Format { get; set; }
        }
        #endregion
    }
}
