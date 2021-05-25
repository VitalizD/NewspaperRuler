using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace NewspaperRuler
{
    public static class ArticleConstructor
    {
        public static List<List<Article>> ArticlesByLevel { get; private set; }

        public static GraphicObject ArticleBackground { get; set; }

        public static void Initialize()
        {
            ArticlesByLevel = Directory.GetDirectories("Articles")
                .Select(GetArticlesFromDirectory)
                .ToList();
        }

        private static List<Article> GetArticlesFromDirectory(string directory)
        {
            return Directory.GetFiles(directory)
                .Select(ReadArticle)
                .ToList();
        }

        private static Article ReadArticle(string path)
        {
            var reader = new StreamReader(path);
            var title = "";
            var genre = "";
            var loyality = 0;
            var reprimandScore = 0;
            var mistake = Mistake.None;
            var flag = "";
            // Руками разбирать формат статьи неудобно. Возьми лучше Newtonwoft.Json или System.Text.Json
            while (true)
            {
                var line = reader.ReadLine();
                var tag = new StringBuilder();
                var flagContent = false;
                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i] == ':')
                    {
                        var remainingPart = "";
                        if (i != line.Length - 1 && i != line.Length - 2) 
                            remainingPart = line.Substring(i + 2);
                        switch (tag.ToString())
                        {
                            case "Loyality": loyality = int.Parse(remainingPart); break;
                            case "Reprimand": reprimandScore = int.Parse(remainingPart); break;
                            case "Title": title = remainingPart; break;
                            case "Genre": genre = remainingPart; break;
                            case "Content": flagContent = true; break;
                            case "Mistake": Enum.TryParse(remainingPart, out mistake); break;
                            case "Flag": flag = remainingPart; break;
                            default: throw new ArgumentException($"This tag doesn't exist: {tag}");
                        }
                        break;
                    }
                    tag.Append(line[i]);
                    if (i == line.Length - 1) throw new Exception($"Tag separator \":\" wasn't found: {path}");
                }
                if (flagContent) break;
            }
            var text = reader.ReadToEnd();
            return new Article(ArticleBackground, text, title, genre, mistake, loyality, reprimandScore, flag);
        }
    }
}
