using System;
using System.Drawing;

namespace NewspaperRuler
{
    public class MainMenu : IUserInterface
    {
        private readonly Sounds sounds;
        private readonly Form1 form;
        private readonly Action changeInterface;
        private readonly Action<Difficulties> startGame;

        private readonly TextBoxControl start;
        private readonly TextBoxControl exit;
        private readonly TextBoxControl normal;
        private readonly TextBoxControl easy;
        private readonly TextBoxControl back;

        private string description = "";

        private readonly Waiting createBeginText;
        private readonly GraphicObject logo = new GraphicObject(Properties.Resources.Logo, 550, 220);

        private Difficulties difficulty;

        private string beginText = "";

        public MainMenu(Form1 form, Sounds sounds, Action changeInterface, Action<Difficulties> startGame)
        {
            this.sounds = sounds;
            this.form = form;
            this.changeInterface = changeInterface;
            this.startGame = startGame;
            createBeginText = new Waiting(GoTransition);
            var imageButton = Properties.Resources.Button;
            var width = 300;
            var height = 100;
            var shiftX = 30;
            var shiftY = 30;
            var font = StringStyle.BigFont;
            var brush = StringStyle.Black;
            start = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "СТАРТ", font, brush);
            exit = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "ВЫХОД", font, brush);
            normal = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "ОБЫЧНЫЙ", font, brush);
            easy = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "ЛЁГКИЙ", font, brush);
            back = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "НАЗАД", font, brush);
        }

        public void EveryTick() 
        {
            start.EveryTick();
            exit.EveryTick();
            easy.EveryTick();
            normal.EveryTick();
            back.EveryTick();

            createBeginText.EveryTick();

            if (start.CursorIsHovered()) description = "Начните строить СВОЮ историю.";
            else if (exit.CursorIsHovered()) description = "Хотите выйти? Мы будем по Вам скучать...";
            else if (normal.CursorIsHovered())
                description = "Оригинальная сложность игры. Вам предстоит столкнуться со множеством трудностей.";
            else if (easy.CursorIsHovered())
                description = "Для тех, кто хочет играть в расслабленном режиме.";
            else if (back.CursorIsHovered()) description = "Назад.";
            else description = "";
        }

        public void MouseDown() 
        {
            if (exit.CursorIsHovered()) 
                form.Close();
            else if (start.CursorIsHovered()) 
                ClickStart();
            else if (back.CursorIsHovered()) 
                ClickBack();
            else if (normal.CursorIsHovered()) 
                ClickDifficulty(Difficulties.Normal);
            else if (easy.CursorIsHovered()) 
                ClickDifficulty(Difficulties.Easy);
            else if (beginText != "")
            {
                changeInterface();
                startGame(difficulty);
            }
        }

        private void ClickStart()
        {
            sounds.PlayMenuButton();
            RemovePhase1();
            EnterPhase2();
        }

        private void ClickBack()
        {
            sounds.PlayMenuButton();
            RemovePhase2();
            EnterPhase1();
        }

        private void RemovePhase1()
        {
            start.GoLeft();
            exit.GoLeft();
        }

        private void RemovePhase2()
        {
            normal.GoLeft();
            easy.GoLeft();
            back.GoLeft();
        }

        private void EnterPhase1()
        {
            start.Position = new Point(0, Scale.Get(300));
            exit.Position = new Point(0, Scale.Get(420));
            start.GoRight();
            exit.GoRight();
        }

        private void EnterPhase2()
        {
            normal.Position = new Point(0, Scale.Get(200));
            easy.Position = new Point(0, Scale.Get(320));
            back.Position = new Point(0, Scale.Get(470));
            normal.GoRight();
            easy.GoRight();
            back.GoRight();
        }

        private void ClickDifficulty(Difficulties difficulty)
        {
            sounds.StopMainMenu();
            sounds.StopTitle();
            sounds.PlayMenuButton();
            this.difficulty = difficulty;
            RemovePhase2();
            form.BackgroundImage = null;
            form.BackColor = Color.Black;
            logo.RemoveFromLayout();
            createBeginText.WaitAndExecute(25);
        }

        private void GoTransition()
        {
            sounds.PlaySuddenness();
            beginText = "Сюжетные события подстраиваются под действия игрока." +
                "\nИстория будет развиваться так, как захотите Вы.";
        }

        public void MouseMove() { }

        public void MouseUp() { }

        public void Paint(Graphics graphics)
        {
            start.Paint(graphics);
            exit.Paint(graphics);
            normal.Paint(graphics);
            easy.Paint(graphics);
            back.Paint(graphics);

            logo.Paint(graphics);
            graphics.DrawString(description, StringStyle.BigFont, StringStyle.White, new Rectangle
                (Scale.Get(40), Scale.Resolution.Height - Scale.Get(200), Scale.Get(700), Scale.Get(400)));

            graphics.DrawString(beginText, StringStyle.BigFont, StringStyle.White, new Rectangle
                (Scale.Resolution.Width / 2 - Scale.Get(450), Scale.Resolution.Height / 2 - Scale.Get(100), Scale.Get(900), Scale.Get(400)),
                StringStyle.Center);
        }

        public void SetFormBackground(Form1 form)
        {
            var image = new Bitmap(Properties.Resources.MenuBackground, Scale.Get(1480), Scale.Get(1100));
            form.BackgroundImage = image;
            sounds.PlayTitle();
            sounds.PlayMainMenu();
            logo.Position = new Point(Scale.Get(360), Scale.Get(80));
            beginText = "";
            EnterPhase1();
        }
    }
}
