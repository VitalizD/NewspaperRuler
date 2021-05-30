using System.Drawing;
using WMPLib;

namespace NewspaperRuler
{
    public class Expense
    {
        public bool Marked { get; private set; }

        public ElementControl Selector { get; }

        public Point InitialPosition { get; set; }

        public ExpenseType Type { get; }

        private static Size selectorSize = new Size(50, 50);

        private readonly WindowsMediaPlayer mark = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer cancel = new WindowsMediaPlayer();
        private readonly WindowsMediaPlayer error = new WindowsMediaPlayer();
        private readonly int cost;

        public Expense(string name, int cost, ExpenseType type)
        {
            this.cost = cost;
            Type = type;
            Selector = new ElementControl(name, StringStyle.White, Properties.Resources.SwitchOff, selectorSize.Width, selectorSize.Height);
            Selector.SetTextAreaSize(new Size(Scale.Get(1400), Scale.Get(50)));
            mark.URL = @"Sounds\ChooseOption.wav";
            cancel.URL = @"Sounds\Cancel.wav";
            error.URL = @"Sounds\Error.wav";
            error.settings.volume = 50;
            error.close();
            mark.close();
            cancel.close();
        }

        public void Paint(Graphics graphics) => Selector.Paint(graphics);

        public int SetMark(int money)
        {
            if (Marked)
            {
                Marked = false;
                cancel.controls.play();
                Selector.Bitmap = new Bitmap(Properties.Resources.SwitchOff, Scale.Get(selectorSize.Width), Scale.Get(selectorSize.Height));
                return cost;
            }
            else
            {
                if (money - cost >= 0)
                {
                    Marked = true;
                    mark.controls.play();
                    Selector.Bitmap = new Bitmap(Properties.Resources.SwitchOn, Scale.Get(selectorSize.Width), Scale.Get(selectorSize.Height));
                    return -cost;
                }
                else error.controls.play();
                return 0;
            }
        }
    }
}
