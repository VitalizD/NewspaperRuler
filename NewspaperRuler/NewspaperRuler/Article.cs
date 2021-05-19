using System;
using System.Drawing;

namespace NewspaperRuler
{
    public class Article
    {
        public string Title { get; }
        public string Genre { get; }
        public string Text { get; }
        public GraphicObject Background { get; }
        public Mistake Mistake { get; }
        public int Loyality { get; }
        public int ReprimandScore { get; }
        public string Flag { get; }

        public Article(GraphicObject background, string text, string title, string genre, Mistake mistake, int loyality, int reprimandScore, string flag)
        {
            Background = background;
            this.Text = text;
            this.Title = title;
            this.Genre = genre;
            Mistake = mistake;
            Loyality = loyality;
            ReprimandScore = reprimandScore;
            Flag = flag;
        }

        public Article(GraphicObject background, string text) : this(background, text, "", "", Mistake.None, 0, 0, "") { }
        public Article(GraphicObject background, string text, Mistake mistake) : this(background, text, "", "", mistake, 0, 0, "") { }
        public Article(GraphicObject background, string text, string title) : this(background, text, title, "", Mistake.None, 0, 0, "") { }
        public Article(GraphicObject background, string text, string title, Mistake mistake) : this(background, text, title, "", mistake, 0, 0, "") { }
        public Article(GraphicObject background, string text, string title, string genre) : this(background, text, title, genre, Mistake.None, 0, 0, "") { }
        public Article(GraphicObject background, string text, string title, string genre, Mistake mistake) : this(background, text, title, genre, mistake, 0, 0, "") { }
        public Article(GraphicObject background, string text, string title, string genre, int loyality) : this(background, text, title, genre, Mistake.None, loyality, 0, "") { }
        public Article(GraphicObject background, string text, string title, string genre, int loyality, int reprimandScore) : this(background, text, title, genre, Mistake.None, loyality, reprimandScore, "") { }
        public Article(GraphicObject background, string text, int loyality) : this(background, text, null, null, Mistake.None, loyality, 0, "") { }
        public Article(GraphicObject background, string text, string title, int loyality) : this(background, text, title, "", Mistake.None, loyality, 0, "") { }

        public void Paint(Graphics graphics)
        {
            graphics.DrawString(Text, StringStyle.TextFont, StringStyle.Brush, new Rectangle(
                Background.Position + new Size(Scl.Get(30), Scl.Get(160)), Background.Bitmap.Size - new Size(60, 100)));
            if (Title != null)
                graphics.DrawString(Title, StringStyle.TitleFont, StringStyle.Brush, new Rectangle(
                    Background.Position + new Size(Scl.Get(30), Scl.Get(70)), new Size(Background.Bitmap.Width - Scl.Get(90), Scl.Get(100))), 
                    new StringFormat { Alignment = StringAlignment.Center });
        }

        public string ExtractBeginning() => Text.Substring(0, Scl.Get(55));
    }
}
