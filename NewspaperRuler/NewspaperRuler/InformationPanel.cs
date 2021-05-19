﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class InformationPanel : GraphicObject
    {
        private readonly List<(string text, Rectangle rectangle)> areas = new List<(string, Rectangle)>();
        private bool enabled;
        private readonly Action playSoundPanelShow;
        private readonly Action playSoundPanelHide;

        public InformationPanel(Image image, int width, int height, Point position, Action playSoundPanelShow, Action playSoundPanelHide, bool zoom = true)
            : base(image, width, height, position, 125, zoom)
        {
            this.playSoundPanelHide = playSoundPanelHide;
            this.playSoundPanelShow = playSoundPanelShow;
        }

        public void Add(string text)
        {
            areas.Add((text, new Rectangle(new Point(), new Size(Scl.Get(350), Scl.Get(50)))));
        }

        public new void Paint(Graphics graphics)
        {
            base.Paint(graphics);
            for (var i = 0; i < areas.Count; i++)
            {
                var rectangle = areas[i].rectangle;
                rectangle.Location = new Point(Position.X + Scl.Get(100), Position.Y + Scl.Get(70) + (Scl.Get(10)  + rectangle.Height) * i);
                graphics.DrawRectangle(StringStyle.Pen, rectangle);
                graphics.FillRectangle(new SolidBrush(Color.LightGray), rectangle);
                graphics.DrawString(areas[i].text, StringStyle.TextFont, StringStyle.Black, rectangle);
            }
        }

        public void Show()
        {
            if (enabled) return;
            GoRight();
        }

        public void Hide()
        {
            if (!enabled) return;
            enabled = false;
            GoLeft();
        }

        public void Tick()
        {
            Move();
            if (Position.X > -Bitmap.Width / 5)
            {
                Stop();
                Position = new Point(-Bitmap.Width / 5, Position.Y);
                enabled = true;
                playSoundPanelShow();
            }
            else if (Position.X < -Bitmap.Width)
            {
                Stop();
                Position = new Point(-Bitmap.Width, Position.Y);
                playSoundPanelHide();
            }
        }
    }
}
