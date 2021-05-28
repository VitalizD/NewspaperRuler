using System.Drawing;

namespace NewspaperRuler
{
    interface IUserInterface
    {
        void Paint(Graphics graphics);
        void MouseDown();
        void MouseUp();
        void MouseMove();
        void EveryTick();
    }
}
