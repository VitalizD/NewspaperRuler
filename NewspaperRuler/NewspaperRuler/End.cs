using System.Drawing;
using System.Collections.Generic;
using System;

namespace NewspaperRuler
{
    public class End : IUserInterface
    {
        private readonly Action goToMainMenu;
        private Queue<(string, Bitmap)> texts;
        private string currentText = "";
        private Bitmap currentBitmap;
        private readonly Sounds sounds;

        public End(Action goToMainMenu, Sounds sounds)
        {
            this.sounds = sounds;
            this.goToMainMenu = goToMainMenu;
        }

        public void CreateQueue(params (string text, Bitmap bitmap)[] values) => texts = new Queue<(string, Bitmap)>(values);

        private void Next()
        {
            //sounds.PlaySuddenness();
            (currentText, currentBitmap) = texts.Dequeue();
        }

        public void EveryTick() { }

        public void MouseDown()
        {
            if (texts.Count > 0)
                Next();
            else
            {
                sounds.StopAll();
                goToMainMenu();
            }
        }

        public void MouseMove() { }

        public void MouseUp() { }

        public void Paint(Graphics graphics)
        {
            if (currentBitmap != null)
                graphics.DrawImage(currentBitmap, Scale.Resolution.Width / 2 - Scale.Get(250), Scale.Get(100));

            graphics.DrawString(currentText, StringStyle.BigFont, StringStyle.White, new Rectangle(
                Scale.Resolution.Width / 2 - Scale.Get(600), Scale.Resolution.Height / 2 + Scale.Get(210), Scale.Get(1200), Scale.Get(600)),
                StringStyle.Center);
        }

        public void ExecuteAfterTransition(Form1 form)
        {
            form.BackgroundImage = null;
            form.BackColor = Color.Black;
            sounds.StopAll();
            sounds.PlayEnd();
            Next();
        }
    }
}
