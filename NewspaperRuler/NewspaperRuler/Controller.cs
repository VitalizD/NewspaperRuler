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

        private Waiting waitIntro;

        public Controller(Form1 form)
        {
            this.form = form;
            sounds = new Sounds();

            dayEnd = new DayEnd(sounds, form.Controls);
            workTable = new WorkTable(form, sounds, dayEnd, ChangeInterface, () => ChangeInterface(menu));
            menu = new MainMenu(form, sounds, () => ChangeInterface(workTable), workTable.StartGame,
                workTable.LoadLevel);

            form.BackColor = Color.Black;
            waitIntro = new Waiting(() => ChangeInterface(menu));
            waitIntro.WaitAndExecute(150);
        }

        public void Paint(Graphics graphics)
        {
            if (currentInterface is null)
                graphics.DrawString(
                "Ресурсы (изображения, звуки) взяты из открытых источников " +
                "и используются в некоммерческих целях.\n\nЗапрещено любое коммерческое использование ресурсов игры " +
                "без письменного разрешения авторов (правообладателей) ресурсов." +
                "\n\nСоздатель игры не несёт ответственность за использование авторских ресурсов.",
                //"Спасибо за проявленный к игре интерес!",
                StringStyle.BigFont, StringStyle.White, new Rectangle(Scale.Get(200), Scale.Resolution.Height / 2 - Scale.Get(150), Scale.Get(1000), 0), StringStyle.Center);
            currentInterface?.Paint(graphics);
        }

        public void MouseDown()
        {
            currentInterface?.MouseDown();

            if (workTable.Stats is null) return;
            var money = workTable.Stats.Money;
            workTable.Stats.Money += dayEnd.MouseDown(money);
        }

        public void MouseUp() => currentInterface?.MouseUp();

        public void MouseMove() => currentInterface?.MouseMove();

        public void EveryTick()
        {
            sounds?.EveryTick();
            waitIntro?.EveryTick();
            currentInterface?.EveryTick();
        }

        private void ChangeInterface(IUserInterface value)
        {
            value.ExecuteAfterTransition(form);
            currentInterface = value;
            form.Controls.Clear();
        }
    }
}
