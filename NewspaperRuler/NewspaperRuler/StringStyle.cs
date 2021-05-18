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
        public static Font TextFont { get; } = new Font(fontName, 16);
        public static Font TitleFont { get; } = new Font(fontName, 20, FontStyle.Bold);
        public static Font BigFont { get; } = new Font(fontName, 24);
        public static SolidBrush Brush { get; } = new SolidBrush(Color.Black);
    }
}
