using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace NewspaperRuler
{
    public static class ArticleConstructor
    {
        public static List<List<Article>> ArticlesByLevel { get; private set; }

        public static GraphicObject ArticleBackground { get; private set; } = new GraphicObject(new Bitmap(Properties.Resources.Paper, 650, 900));

        public static void Initialize()
        {
            ArticlesByLevel = new List<List<Article>>
            {
                Level1(),
                Level2()
            };
        }

        private static List<Article> Level1()
        {
            return new List<Article>
            {
                ReadArticle(@"Articles\Level1\Article1.txt"),
                ReadArticle(@"Articles\Level1\Article2.txt"),
                ReadArticle(@"Articles\Level1\Article3.txt"),
                ReadArticle(@"Articles\Level1\Article4.txt"),
                ReadArticle(@"Articles\Level1\Article5.txt"),
                ReadArticle(@"Articles\Level1\Article6.txt"),
                ReadArticle(@"Articles\Level1\Article7.txt"),
                ReadArticle(@"Articles\Level1\Article8.txt"),
                ReadArticle(@"Articles\Level1\Article9.txt"),
            };
        }

        private static List<Article> Level2()
        {
            return new List<Article>
            {
                ReadArticle(@"Articles\Level2\Article1.txt"),
                ReadArticle(@"Articles\Level2\Article2.txt"),
                ReadArticle(@"Articles\Level2\Article3.txt"),
                ReadArticle(@"Articles\Level2\Article4.txt"),
                ReadArticle(@"Articles\Level2\Article5.txt"),
                ReadArticle(@"Articles\Level2\Article6.txt"),
                ReadArticle(@"Articles\Level2\Article7.txt"),
                ReadArticle(@"Articles\Level2\Article8.txt"),
                ReadArticle(@"Articles\Level2\Article9.txt"),
                ReadArticle(@"Articles\Level2\Article10.txt"),
            };
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
