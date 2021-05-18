using System.Drawing;

namespace NewspaperRuler
{
    public static class AuxiliaryMethods
    {
        public static bool IsClickedOnArea(Point mouseLocation, Point position, Size size) =>
            mouseLocation.X >= position.X && mouseLocation.X <= position.X + size.Width
            && mouseLocation.Y >= position.Y && mouseLocation.Y <= position.Y + size.Height;
    }
}
