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

        public bool Enabled
        {
            get => isEntering || showed;
        }

        public Remark(Image image, int width, int height, Sounds sounds)
            : base(image, width, height, new Point(Scale.Resolution.Width, Scale.Resolution.Height))
        {
            this.sounds = sounds;
            trashCan = new ElementControl(Properties.Resources.TrashCan, 30, 30);
        }

        public void Add(string remark) => texts.Enqueue($"ЗАМЕЧАНИЕ\n\n{remark}");

        public void Clear() => texts.Clear();

        public new void Paint(Graphics graphics)
        {
            base.Paint(graphics);
            trashCan.Paint(graphics);
            graphics.DrawString(currentText, StringStyle.TextFont, new SolidBrush(Color.DarkRed), new Rectangle(
                Position + new Size(Scale.Get(55), Scale.Get(20)), Bitmap.Size - new Size(Scale.Get(80), Scale.Get(40))));
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
            Position = new Point(Scale.Resolution.Width - Bitmap.Width, Scale.Resolution.Height);
            MovementSpeed = 5;
            currentText = texts.Dequeue();
            sounds.PlayPrintingMachine();
            GoUp();
        }

        public void Hide()
        {
            showed = false;
            MovementSpeed = 100;
            trashCan.Hide();
            sounds.PlayPaper();
            GoRight();
        }

        private bool remarkIsCompletelyOut;
        public void EveryTick()
        {
            base.Move();
            if (Position.Y < Scale.Resolution.Height - Bitmap.Height - Scale.Get(50))
            {
                isEntering = false;
                showed = true;
                Position = new Point(Position.X, Scale.Resolution.Height - Bitmap.Height - Scale.Get(50));
                Stop();
                if (texts.Count == 0)
                    trashCan.ShowImage(Position + Bitmap.Size - trashCan.Bitmap.Size - new Size(Scale.Get(20), Scale.Get(20)));
                else Hide();
            }
            else if (Position.Y < Scale.Resolution.Height - (int)(Bitmap.Height / 1.15) && !remarkIsCompletelyOut)
            {
                remarkIsCompletelyOut = true;
                MovementSpeed = 30;
                GoUp();
            }
            else if (Position.X > Scale.Resolution.Width)
            {
                Position = new Point(Scale.Resolution.Width, Position.Y);
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
