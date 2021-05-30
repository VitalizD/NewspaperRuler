using System.Drawing;
using System.Drawing.Text;

namespace NewspaperRuler
{
    public static class StringStyle
    {
        private readonly static FontFamily fontFamily;

        public static string FontNameForLabels { get; } = "Bookman Old Style";
        public static Font TextFont { get; }
        public static Font TitleFont { get; }
        public static Font BigFont { get; }
        public static SolidBrush Black { get; } = new SolidBrush(Color.Black);
        public static SolidBrush White { get; } = new SolidBrush(Color.White);
        public static Pen Pen { get; } = new Pen(Color.Black);
        public static StringFormat Center { get; } = new StringFormat { Alignment = StringAlignment.Center };

        static StringStyle()
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(@"font.ttf"); 
            fontFamily = fontCollection.Families[0];
            TextFont = new Font(fontFamily, Scale.Get(16));
            TitleFont = new Font(fontFamily, Scale.Get(20), FontStyle.Bold);
            BigFont = new Font(fontFamily, Scale.Get(23));
        }
    }
}
