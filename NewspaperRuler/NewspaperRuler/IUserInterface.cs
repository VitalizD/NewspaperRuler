using System.Drawing;

namespace NewspaperRuler
{
    public interface IUserInterface
    {
        void Paint(Graphics graphics);
        void ExecuteAfterTransition(Form1 form);
        void MouseDown();
        void MouseUp();
        void MouseMove();
        void EveryTick();
    }
}
