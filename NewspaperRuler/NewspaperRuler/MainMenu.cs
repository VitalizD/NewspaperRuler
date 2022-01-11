using System;
using System.Drawing;
using System.IO;

namespace NewspaperRuler
{
    public class MainMenu : IUserInterface
    {
        private readonly Sounds sounds;
        private readonly Form1 form;
        private readonly Action changeInterface;
        private readonly Action<Difficulties> startGame;
        private readonly Action loadGame;

        private readonly TextBoxControl newGame;
        private readonly TextBoxControl continueGame;
        private readonly TextBoxControl exit;
        private readonly TextBoxControl normal;
        private readonly TextBoxControl easy;
        private readonly TextBoxControl back;

        private string description = "";

        private readonly Waiting createBeginText;
        private readonly GraphicObject logo = new GraphicObject(Properties.Resources.Logo, 550, 220);
        private Difficulties newGameDifficulty;

        private Difficulties savedGameDifficulty;
        private DateTime savedGameDate;

        private string beginText = "";

        public MainMenu(Form1 form, Sounds sounds, Action changeInterface, Action<Difficulties> startGame,
            Action loadGame)
        {
            this.sounds = sounds;
            this.form = form;
            this.changeInterface = changeInterface;
            this.startGame = startGame;
            this.loadGame = loadGame;
            createBeginText = new Waiting(GoTransition);
            var imageButton = Properties.Resources.Button;
            var width = 300;
            var height = 100;
            var shiftX = 30;
            var shiftY = 30;
            var font = StringStyle.BigFont;
            var brush = StringStyle.Black;
            newGame = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "НОВАЯ ИГРА", font, brush);
            continueGame = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "ПРОДОЛЖИТЬ", font, brush);
            exit = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "ВЫХОД", font, brush);
            normal = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "ОБЫЧНЫЙ", font, brush);
            easy = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "ЛЁГКИЙ", font, brush);
            back = new TextBoxControl(imageButton, width, height, shiftX, shiftY, "НАЗАД", font, brush);
        }

        public void EveryTick() 
        {
            newGame.EveryTick();
            exit.EveryTick();
            easy.EveryTick();
            normal.EveryTick();
            back.EveryTick();
            continueGame.EveryTick();

            createBeginText.EveryTick();

            if (newGame.CursorIsHovered()) description = "Начните строить СВОЮ историю.";
            else if (exit.CursorIsHovered()) description = "Хотите выйти? Мы будем по Вам скучать...";
            else if (normal.CursorIsHovered())
                description = "Оригинальная сложность игры. Вам предстоит столкнуться со множеством трудностей.";
            else if (easy.CursorIsHovered())
                description = "Для тех, кто хочет играть в расслабленном режиме.";
            else if (back.CursorIsHovered()) description = "";
            else if (continueGame.CursorIsHovered())
                description = "Продолжите игру с последнего сохранённого дня." +
                    $"\n\n{savedGameDate:D}\nСложность: {Translation.ruDifficulties[savedGameDifficulty]}";
            else description = "";
        }

        public void MouseDown() 
        {
            if (exit.CursorIsHovered())
                form.Close();
            else if (newGame.CursorIsHovered())
                ClickStart();
            else if (back.CursorIsHovered())
                ClickBack();
            else if (normal.CursorIsHovered())
                ClickDifficulty(Difficulties.Normal);
            else if (easy.CursorIsHovered())
                ClickDifficulty(Difficulties.Easy);
            else if (continueGame.CursorIsHovered())
                ClickContinue();
            else if (beginText != "")
            {
                changeInterface();
                startGame(newGameDifficulty);
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

        private void ClickContinue()
        {
            Leave();
            loadGame();
        }

        private void RemovePhase1()
        {
            if (AuxiliaryMethods.SaveExists())
                continueGame.GoLeft();
            newGame.GoLeft();
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
            if (AuxiliaryMethods.SaveExists())
            {
                continueGame.Position = new Point(0, Scale.Get(230));
                var savedGame = AuxiliaryMethods.GetSave();
                savedGameDate = new DateTime(savedGame.Year, savedGame.Mouth, savedGame.Day).AddDays(1);
                savedGameDifficulty = savedGame.Difficulty;
            }
            newGame.Position = new Point(0, Scale.Get(350));
            exit.Position = new Point(0, Scale.Get(470));
            newGame.GoRight();
            exit.GoRight();
            continueGame.GoRight();
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
            sounds.PlayMenuButton();
            Leave();
            this.newGameDifficulty = difficulty;
            RemovePhase2();
            if (AuxiliaryMethods.SaveExists())
                File.Delete(SavedData.name);
            logo.RemoveFromLayout();
            createBeginText.WaitAndExecute(25);
        }

        private void Leave()
        {
            sounds.StopMainMenu();
            sounds.StopTitle();
            form.BackgroundImage = null;
            form.BackColor = Color.Black;
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
            newGame.Paint(graphics);
            exit.Paint(graphics);
            normal.Paint(graphics);
            easy.Paint(graphics);
            back.Paint(graphics);
            continueGame.Paint(graphics);

            logo.Paint(graphics);
            graphics.DrawString(description, StringStyle.BigFont, StringStyle.White, new Rectangle
                (Scale.Get(40), Scale.Resolution.Height - Scale.Get(200), Scale.Get(700), Scale.Get(400)));

            graphics.DrawString(beginText, StringStyle.BigFont, StringStyle.White, new Rectangle
                (Scale.Resolution.Width / 2 - Scale.Get(450), Scale.Resolution.Height / 2 - Scale.Get(100), Scale.Get(900), Scale.Get(400)),
                StringStyle.Center);
        }

        public void ExecuteAfterTransition(Form1 form)
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
