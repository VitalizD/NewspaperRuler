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
        public int NumberInQueue { get; }

        /// <param name="numberInQueue">Укажите -1, чтобы установить случайное место в очереди.</param>
        public Article(GraphicObject background, string text, string title, string genre, Mistake mistake, int loyality, int reprimandScore, string flag, int numberInQueue)
        {
            Background = background;
            Text = text;
            Title = title;
            Genre = genre;
            Mistake = mistake;
            Loyality = loyality;
            ReprimandScore = reprimandScore;
            Flag = flag;
            NumberInQueue = numberInQueue;
        }

        public Article(GraphicObject background, string text, string title, int numberInQueue) 
            : this(background, text, title, "", Mistake.None, 0, 0, "", numberInQueue) { }

        public void Paint(Graphics graphics)
        {
            graphics.DrawString(Text, StringStyle.TextFont, StringStyle.Black, new Rectangle(
                Background.Position + new Size(Scale.Get(30), Scale.Get(160)), Background.Bitmap.Size - new Size(60, 120)));
            if (Title != null)
                graphics.DrawString(Title, StringStyle.TitleFont, StringStyle.Black, new Rectangle(
                    Background.Position + new Size(Scale.Get(30), Scale.Get(70)), new Size(Background.Bitmap.Width - Scale.Get(90), Scale.Get(100))), 
                    StringStyle.Center);
            if (Genre != null)
                graphics.DrawString(Genre, StringStyle.TextFont, StringStyle.Black, new Rectangle(
                    Background.Position + new Size(Scale.Get(30), Scale.Get(130)), new Size(Background.Bitmap.Width - Scale.Get(100), Scale.Get(40))),
                    StringStyle.Right);
        }

        public string ExtractBeginning() => Text.Substring(0, Scale.Get(55));
    }
}
