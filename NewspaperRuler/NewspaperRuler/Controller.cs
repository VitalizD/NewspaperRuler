using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class Controller
    {
        private readonly Form1 form;
        private readonly Sounds sounds;

        private readonly WorkTable workTable;
        private readonly DayEnd dayEnd;
        private readonly MainMenu menu;

        private IUserInterface currentInterface;

        public Controller(Form1 form)
        {
            this.form = form;
            sounds = new Sounds();

            dayEnd = new DayEnd(sounds, form.Controls);
            workTable = new WorkTable(form, sounds, dayEnd, ChangeInterface, () => ChangeInterface(menu));
            menu = new MainMenu(form, sounds, () => ChangeInterface(workTable), workTable.StartGame);

            ChangeInterface(menu);
        }

        public void Paint(Graphics graphics)
        {
            currentInterface.Paint(graphics);
        }

        public void MouseDown()
        {
            currentInterface.MouseDown();

            if (workTable.Stats is null) return;
            var money = workTable.Stats.Money;
            workTable.Stats.Money += dayEnd.MouseDown(money);
        }

        public void MouseUp() => currentInterface.MouseUp();

        public void MouseMove() => currentInterface.MouseMove();

        public void EveryTick()
        {
            sounds.EveryTick();
            currentInterface.EveryTick();
        }

        private void ChangeInterface(IUserInterface value)
        {
            value.ExecuteAfterTransition(form);
            currentInterface = value;
            form.Controls.Clear();
        }
    }
}
