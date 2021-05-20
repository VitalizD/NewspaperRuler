using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace NewspaperRuler
{
    public class Remark : GraphicObject
    {
        private readonly Sounds sounds;
        private readonly ElementControl trashCan;
        private readonly Queue<string> texts = new Queue<string>();
        private string currentText = "";
        private bool isEntering;
        private bool showed;

        public Remark(Image image, int width, int height, Sounds sounds)
            : base(image, width, height, new Point(Scl.Resolution.Width, Scl.Resolution.Height))
        {
            this.sounds = sounds;
            trashCan = new ElementControl(Properties.Resources.TrashCan, 30, 30);
            //Position = Form1.Beyond;
        }

        public void Add(string remark)
        {
            texts.Enqueue($"ЗАМЕЧАНИЕ\n\n{remark}");
        }

        public new void Paint(Graphics graphics)
        {
            base.Paint(graphics);
            trashCan.Paint(graphics);
            graphics.DrawString(currentText, StringStyle.TextFont, StringStyle.Black, new Rectangle(
                Position + new Size(Scl.Get(55), Scl.Get(20)), Bitmap.Size - new Size(Scl.Get(80), Scl.Get(40))));
        }

        public void Show()
        {
            if (isEntering) return;
            EnterRemark();
        }

        private void EnterRemark()
        {
            if (showed)
            {
                Hide();
                return;
            }
            if (texts.Count == 0) return;
            isEntering = true;
            Position = new Point(Scl.Resolution.Width - Bitmap.Width, Scl.Resolution.Height);
            MovementSpeed = 5;
            currentText = texts.Dequeue();
            sounds.PrintingMachine();
            GoUp();
        }

        private void Hide()
        {
            showed = false;
            MovementSpeed = 100;
            trashCan.Hide();
            sounds.Paper();
            GoRight();
        }

        private bool remarkIsCompletelyOut;
        public void Tick()
        {
            base.Move();
            if (Position.Y < Scl.Resolution.Height - Bitmap.Height - Scl.Get(50))
            {
                isEntering = false;
                showed = true;
                Position = new Point(Position.X, Scl.Resolution.Height - Bitmap.Height - Scl.Get(50));
                Stop();
                if (texts.Count == 0)
                    trashCan.ShowImage(Position + Bitmap.Size - trashCan.Bitmap.Size - new Size(Scl.Get(20), Scl.Get(20)));
                else Hide();
            }
            else if (Position.Y < Scl.Resolution.Height - (int)(Bitmap.Height / 1.15) && !remarkIsCompletelyOut)
            {
                remarkIsCompletelyOut = true;
                MovementSpeed = 30;
                GoUp();
            }
            else if (Position.X > Scl.Resolution.Width)
            {
                Position = new Point(Scl.Resolution.Width, Position.Y);
                remarkIsCompletelyOut = false;
                Stop();
                EnterRemark();
            }
        }

        public void MouseDown()
        {
            if (trashCan.CursorIsHovered())
                Hide();
        }
    }
}
