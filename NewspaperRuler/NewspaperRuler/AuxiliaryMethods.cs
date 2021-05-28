using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public static class AuxiliaryMethods
    {
        public static bool IsClickedOnArea(Rectangle area) =>
            Cursor.Position.X >= area.X && Cursor.Position.X <= area.X + area.Width
            && Cursor.Position.Y >= area.Y && Cursor.Position.Y <= area.Y + area.Height;
    }
}
