using System.Drawing;
using System.Drawing.Text;

namespace NewspaperRuler
{
    public static class StringStyle
    {
        private static FontFamily fontFamily;

        public static string FontName { get; } = "Bookman Old Style";
        public static Font TextFont { get; private set; }
        public static Font TitleFont { get; private set; }
        public static Font BigFont { get; private set; }
        public static SolidBrush Black { get; } = new SolidBrush(Color.Black);
        public static SolidBrush White { get; } = new SolidBrush(Color.White);
        public static Pen Pen { get; } = new Pen(Color.Black);

        public static void Initialize()
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(@"7784.ttf"); 
            fontFamily = fontCollection.Families[0];
            TextFont = new Font(fontFamily, Scl.Get(16));
            TitleFont = new Font(fontFamily, Scl.Get(20), FontStyle.Bold);
            BigFont = new Font(fontFamily, Scl.Get(23));
        }
    }
}
