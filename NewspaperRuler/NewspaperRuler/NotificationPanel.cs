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
        private int waitBeforeOutNotification = 0;
        private readonly Action playSound;

        public NotificationPanel(Point position, Action sound) : base(position)
        {
            rectangle = new Rectangle(position, new Size(1550, 40));
            playSound = sound;
        }

        public new void Paint(Graphics graphics)
        {
            graphics.DrawRectangle(new Pen(StringStyle.Brush), rectangle);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(210, 180, 140)), rectangle);
            graphics.DrawString(text, StringStyle.BigFont, StringStyle.Brush, rectangle, 
                new StringFormat { Alignment = StringAlignment.Center });
        }

        public void Add(string text) => notifications.Enqueue(text);

        public void Show()
        {
            if (enabled) return;
            ShowPanel();
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

        public void Tick()
        {
            rectangle.Location = Position;
            if (!isMoving && waitBeforeOutNotification > 0)
            {
                waitBeforeOutNotification--;
                if (waitBeforeOutNotification == 0) 
                    GoUp();
            }
            else CheckPosition();
        }

        private void CheckPosition()
        {
            if (Position.Y > 0)
            {
                Position = new Point(Position.X, 0);
                StopMoving();
                waitBeforeOutNotification = 80;
            }
            else if (Position.Y < -40)
            {
                Position = new Point(Position.X, -40);
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
