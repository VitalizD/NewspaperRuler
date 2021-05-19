using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewspaperRuler
{
    public static class StringStyle
    {
        private static readonly string fontName = "Bookman Old Style";
        public static Font TextFont { get; private set; }
        public static Font TitleFont { get; private set; }
        public static Font BigFont { get; private set; }
        public static SolidBrush Black { get; } = new SolidBrush(Color.Black);
        public static SolidBrush White { get; } = new SolidBrush(Color.White);
        public static Pen Pen { get; } = new Pen(Color.Black);

        public static void Initialize()
        {
            TextFont = new Font(fontName, Scl.Get(16));
            TitleFont = new Font(fontName, Scl.Get(20), FontStyle.Bold);
            BigFont = new Font(fontName, Scl.Get(23));
        }
    }
}
