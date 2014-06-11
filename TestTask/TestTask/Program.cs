﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplicationTest
{
    public static class Program
    {
        private static readonly Regex imgRegex = new Regex(@"\<img.+?src=\""(?<imgsrc>.+?)\"".+?\>", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        private static void Main()
        {
            var urls = new[] {
                "http://vk.com",
                "http://pikabu.ru",
                "http://www.fc-zenit.ru/",
                "https://vk.com/svyadey",
                "http://pikabu.ru/story/detskie_obidyi_2316261",
                "http://touchin.ru/"
            };

            Parallel.ForEach(urls, DownloadFiles);

            Console.WriteLine("Загрузка закончена");
            Console.ReadKey();
        }

        private static void DownloadFiles(string site)
        {
            string data;
            Console.WriteLine(site);
            Console.WriteLine("Загрузка страницы");
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(site))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        data = reader.ReadToEnd();
                    }
                }
            }

            Console.WriteLine("Загрузка картинок");

            string directory = new Uri(site).Host;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                imgRegex.Matches(data)
                    .Cast<Match>()
                    .Select(m => m.Groups["imgsrc"].Value.Trim())
                    .Distinct()
                    .Select(url => (url.Contains("http://") || url.Contains("https://")) ? url : (site + url))
                    .Select(url => new { url, name = url.Split(new[] { '/' }).Last() })
                    .Where(arg => Regex.IsMatch(arg.name, @"[^\s\/]\.(jpg|png|gif|bmp)\Z"))
                    .AsParallel()
                    .WithDegreeOfParallelism(6)
                    .ForAll(value =>
                    {
                        string savePath = Path.Combine(directory, value.name);
                        try
                        {
                            using (var localClient = new WebClient())
                            {
                                localClient.DownloadFile(value.url, savePath);
                            }
                        }
                        catch (WebException e)
                        {
                            Console.WriteLine(e.Message + "(Incorrect url)");
                        }
                        Console.WriteLine("{0} загружен", value.name);
                    });

            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.Message + "(something has gone wrong)");
            }
        }
    }
}