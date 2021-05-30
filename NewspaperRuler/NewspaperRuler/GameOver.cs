using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class GameOver : IUserInterface
    {
        private readonly Control.ControlCollection controls;

        private readonly GraphicObject iconOver;
        private readonly string message;
        private string suggestion;
        private Label returnButton;
        private Label mainMenuButton;

        public GameOver(Control.ControlCollection controls, GraphicObject icon, string message)
        {
            iconOver = icon;
            iconOver.Position = new Point(Scale.Resolution.Width / 2 - icon.Bitmap.Width / 2, Scale.Get(40));
            this.message = message;
            this.controls = controls;
        }

        public void CreateOnlyMainMenuButton(MouseEventHandler action)
        {
            suggestion = "";
            mainMenuButton = GetLabel("В МЕНЮ", Scale.Resolution.Width / 2 - Scale.Get(200));
            mainMenuButton.MouseDown += action;

            controls.Add(mainMenuButton);
        }

        public void CreateMainMenuButtonAndReturnButton(MouseEventHandler actionForMainMenuButton,
            MouseEventHandler actionForReturnButton)
        {
            suggestion = "Откатить прогресс на 1 день назад?";
            returnButton = GetLabel("ОТКАТ", Scale.Resolution.Width / 2 - Scale.Get(405));
            mainMenuButton = GetLabel("В МЕНЮ", Scale.Resolution.Width / 2 + Scale.Get(5));

            mainMenuButton.MouseDown += actionForMainMenuButton;
            returnButton.MouseDown += actionForReturnButton;

            controls.Add(mainMenuButton);
            controls.Add(returnButton);
        }

        private Label GetLabel(string text, int x)
        {
            return new Label
            {
                Text = text,
                Size = new Size(Scale.Get(400), Scale.Get(50)),
                Location = new Point(x, Scale.Resolution.Height - Scale.Get(70)),
                Font = new Font(StringStyle.FontNameForLabels, Scale.Get(25), FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
            };
        }

        public void EveryTick() { }

        public void MouseDown() { }

        public void MouseMove() { }

        public void MouseUp() { }

        public void Paint(Graphics graphics)
        {
            graphics.DrawString(message, StringStyle.BigFont, StringStyle.White, new Rectangle
                (Scale.Resolution.Width / 2 - Scale.Get(450), Scale.Resolution.Height / 2 + Scale.Get(80), Scale.Get(900), Scale.Get(400)),
                new StringFormat { Alignment = StringAlignment.Center });

            if (suggestion != null)
                graphics.DrawString(suggestion, StringStyle.BigFont, StringStyle.White, new Rectangle
                    (Scale.Resolution.Width / 2 - Scale.Get(450), Scale.Resolution.Height / 2 + Scale.Get(250), Scale.Get(900), Scale.Get(400)),
                    new StringFormat { Alignment = StringAlignment.Center });

            iconOver.Paint(graphics);
        }

        public void SetFormBackground(Form1 form)
        {
            form.BackgroundImage = null;
            form.BackColor = Color.Black;
        }
    }
}
