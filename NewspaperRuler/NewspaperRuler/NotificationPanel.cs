using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class NotificationPanel : GraphicObject
    {
        private Rectangle rectangle;
        private string text = "";
        private readonly Queue<string> notifications = new Queue<string>();
        private bool isMoving;
        private bool enabled;
        private readonly Action playSound;
        private readonly Waiting outNotification;

        public NotificationPanel(Size resolution, Action sound) : base(new Point(0, -Scale.Get(45)), 5)
        {
            rectangle = new Rectangle(new Point(0, -Scale.Get(45)), new Size(resolution.Width, Scale.Get(40)));
            playSound = sound;
            outNotification = new Waiting(GoUp);
        }

        public new void Paint(Graphics graphics)
        {
            graphics.DrawRectangle(new Pen(StringStyle.Black), rectangle);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(210, 180, 140)), rectangle);
            graphics.DrawString(text, StringStyle.BigFont, StringStyle.Black, rectangle, 
                new StringFormat { Alignment = StringAlignment.Center });
        }

        public void Add(string text) => notifications.Enqueue(text);

        public void Clear() => notifications.Clear();

        public void Show()
        {
            if (enabled) return;
            ShowPanel();
        }

        public void Hide()
        {
            Clear();
            StopMoving();
            enabled = false;
            Position = new Point(Position.X, -55);
        }

        private void ShowPanel()
        {
            if (notifications.Count == 0)
            {
                enabled = false;
                return;
            }
            text = notifications.Dequeue();
            enabled = true;
            isMoving = true;
            playSound();
            GoDown();
        }

        public void EveryTick()
        {
            rectangle.Location = Position;
            Move();
            if (!isMoving) 
                outNotification.EveryTick();
            CheckPosition();
        }

        private void CheckPosition()
        {
            if (Position.Y > 0)
            {
                Position = new Point(Position.X, 0);
                StopMoving();
                outNotification.WaitAndExecute(80);
            }
            else if (Position.Y < -55)
            {
                Position = new Point(Position.X, -55);
                StopMoving();
                ShowPanel();
            }
        }

        private void StopMoving()
        {
            Stop();
            isMoving = false;
        }
    }
}
