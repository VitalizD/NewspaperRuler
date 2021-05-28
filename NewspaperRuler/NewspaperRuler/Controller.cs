using System.Drawing;

namespace NewspaperRuler
{
    public class Controller
    {
        private readonly Stats stats;
        private readonly Form1 form;

        private readonly WorkTable workTable;
        private readonly DayEnd dayEnd;

        private IUserInterface currentInterface;

        public Controller(Form1 form)
        {
            this.form = form;
            var sounds = new Sounds();

            dayEnd = new DayEnd(sounds, form.Controls);
            stats = new Stats(100, dayEnd);
            workTable = new WorkTable(form, sounds, stats, dayEnd, ChangeInterface);

            ChangeInterface(Interface.WorkTable);
        }

        public void Paint(Graphics graphics)
        {
            currentInterface.Paint(graphics);
        }

        public void MouseDown()
        {
            currentInterface.MouseDown();

            var money = stats.Money;
            stats.Money += dayEnd.MouseDown(money);
        }

        public void MouseUp()
        {
            currentInterface.MouseUp();
        }

        public void MouseMove()
        {
            currentInterface.MouseMove();
        }

        public void EveryTick()
        {
            currentInterface.EveryTick();
        }

        private void ChangeInterface(Interface value)
        {
            switch (value)
            {
                case Interface.WorkTable: 
                    form.BackgroundImage = Properties.Resources.Background;
                    currentInterface = workTable;
                    break;
                case Interface.DayEnd: 
                    form.BackgroundImage = null;
                    form.BackColor = Color.Black;
                    currentInterface = dayEnd;
                    break;
            }
        }
    }
}
