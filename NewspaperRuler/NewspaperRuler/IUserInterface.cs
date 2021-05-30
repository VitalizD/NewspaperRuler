using System.Drawing;

namespace NewspaperRuler
{
    public interface IUserInterface
    {
        void Paint(Graphics graphics);
        void SetFormBackground(Form1 form);
        void MouseDown();
        void MouseUp();
        void MouseMove();
        void EveryTick();
    }
}
