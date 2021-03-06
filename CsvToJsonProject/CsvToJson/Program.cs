﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace CsvToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Java Script Serializer
                var javaScriptSerializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            
                //Items
                Console.Write("Creating itemsDb.json...");
                var itemsCsvContent = File.ReadAllLines("items.csv");
                var itemsList = new List<Item>();
                foreach (var line in itemsCsvContent.Skip(1)) itemsList.Add(new Item(line));
                var itemsJson = javaScriptSerializer.Serialize(itemsList);
                var formattedItemsJson = JsonFormatter.Format(itemsJson);
                File.WriteAllText("..\\AdAstra\\Assets\\StreamingAssets\\itemsDb.json", formattedItemsJson);
                Console.WriteLine("Completed!");


                //Items enum cs file (I know doesn't really fit here but I can kill 2 birds with 1 stone :)
                Console.Write("Creating ItemId.cs...");
                var allItems = new Dictionary<string, string>();//ItemId-ItemFunction
                foreach (var line in itemsCsvContent.Skip(1)) allItems.Add(line.Split(',')[0], line.Split(',')[5]);
                var csFileContent = GetCsFileContent(allItems);
                File.WriteAllText("..\\AdAstra\\Assets\\Scripts\\Models\\ItemId.cs", csFileContent);
                Console.WriteLine("Completed!");


                //Blueprints
                Console.Write("Creating blueprintsDb.json...");
                var blueprintsCsvContent = File.ReadAllLines("blueprints.csv");
                var blueprintsList = new List<Blueprint>();
                foreach (var line in blueprintsCsvContent.Skip(1)) blueprintsList.Add(new Blueprint(line));
                var blueprintsJson = javaScriptSerializer.Serialize(blueprintsList);
                var formattedBlueprintsJson = JsonFormatter.Format(blueprintsJson);
                File.WriteAllText("..\\AdAstra\\Assets\\StreamingAssets\\blueprintsDb.json", formattedBlueprintsJson);
                Console.WriteLine("Completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if(ex.InnerException != null) Console.WriteLine(ex.InnerException);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==========");
            Console.WriteLine("Press any key to EXIT");
            Console.ReadKey();
        }

        #region ITEM
        private class Item
        {
            public string ItemId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string MaxStackSize { get; set; }
            public string SpriteName { get; set; }
            public string ItemFunction { get; set; }
            public Dictionary<string, string> FunctionProperties { get; set; }

            public Item(string line)
            {
                var properties = line.Split(',');

                ItemId = properties[0];
                Title = properties[1];
                Description = properties[2];
                MaxStackSize = properties[3];
                SpriteName = properties[4];
                ItemFunction = properties[5];
                FunctionProperties = new Dictionary<string, string>();

                for (var i = 6; i < properties.Length; i = i+2)
                {
                    var prop = properties[i];
                    if (string.IsNullOrEmpty(prop)) continue;
                    var value = properties[i + 1];
                    FunctionProperties.Add(prop, value);
                }
            }
        }

        private static string GetCsFileContent(Dictionary<string, string> allItems)
        {
            //ItemFunction - List of ItemId
            var grouped = allItems.GroupBy(r => r.Value).ToDictionary(t => t.Key, t => t.Select(r => r.Key).ToList());

            //Create all CS file content
            var sb = new StringBuilder();

            sb.AppendLine("namespace Assets.Scripts.Models");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic enum ItemId");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tNone,");
            sb.AppendLine();

            foreach (var gr in grouped)
            {
                sb.AppendLine($"\t\t//========== {gr.Key}");
                foreach (var item in gr.Value)
                {
                    sb.AppendLine($"\t\t{item},");
                }
                sb.AppendLine();
                sb.AppendLine();
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }
        #endregion

        #region BLUEPRINT
        private class Blueprint
        {
            public string ResultItemId { get; private set; }
            public string ResultItemCount { get; private set; }
            public string CraftingTime { get; private set; }
            public Dictionary<string, string> Requirements { get; private set; }

            public Blueprint(string line)
            {
                var properties = line.Split(',');
                
                ResultItemId = properties[0];
                ResultItemCount = properties[1];
                CraftingTime = float.Parse(properties[2]).ToString("F1");
                Requirements = new Dictionary<string, string>();

                for (var i = 3; i < properties.Length; i = i + 2)
                {
                    var req = properties[i];
                    if (string.IsNullOrEmpty(req)) continue;
                    var count = properties[i + 1];
                    Requirements.Add(req, count);
                }
            }
        }
        #endregion

        #region JSON FORMATTER
        private class JsonFormatter
        {
            public static string Format(string jsonString)
            {
                var stringBuilder = new StringBuilder();
                var escaping = false;
                var inQuotes = false;
                var indentation = 0;

                foreach (char character in jsonString)
                {
                    if (escaping)
                    {
                        escaping = false;
                        stringBuilder.Append(character);
                    }
                    else
                    {
                        if (character == '\\')
                        {
                            escaping = true;
                            stringBuilder.Append(character);
                        }
                        else if (character == '\"')
                        {
                            inQuotes = !inQuotes;
                            stringBuilder.Append(character);
                        }
                        else if (!inQuotes)
                        {
                            if (character == ',')
                            {
                                stringBuilder.Append(character);
                                stringBuilder.Append("\r\n");
                                stringBuilder.Append('\t', indentation);
                            }
                            else if (character == '[' || character == '{')
                            {
                                stringBuilder.Append(character);
                                stringBuilder.Append("\r\n");
                                stringBuilder.Append('\t', ++indentation);
                            }
                            else if (character == ']' || character == '}')
                            {
                                stringBuilder.Append("\r\n");
                                stringBuilder.Append('\t', --indentation);
                                stringBuilder.Append(character);
                            }
                            else if (character == ':')
                            {
                                stringBuilder.Append(character);
                                stringBuilder.Append('\t');
                            }
                            else
                            {
                                stringBuilder.Append(character);
                            }
                        }
                        else
                        {
                            stringBuilder.Append(character);
                        }
                    }
                }

                return stringBuilder.ToString();
            }
        }
        #endregion
    }
}
