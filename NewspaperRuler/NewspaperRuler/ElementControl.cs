using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class ElementControl : GraphicObject
    {
        public bool IsVisible
        {
            get { return Position.X > -Bitmap.Width && Position.Y > -Bitmap.Height; }
        }

        public string Description { get; set; }

        private Point descriptionPosition = Form1.Beyond;
        private readonly SolidBrush brush;
        private readonly Action playSound;
        private Size textAreaSize;
        private StringFormat stringFormat;

        public ElementControl(string description, SolidBrush brush, Image image, int width, int height, bool zoom = true) 
            : base(image, width, height, zoom)
        {
            this.Description = description;
            this.brush = brush;
            textAreaSize = new Size(Scale.Get(200), Scale.Get(300));
            Hide();
        }

        public ElementControl(Image image, int width, int height, bool zoom = true)
            : base(image, width, height, Form1.Beyond, zoom) { }

        public ElementControl(string description, SolidBrush brush, Action playerOnClick, Image image, int width, int height, bool zoom = true)
            : this(description, brush, image, width, height, zoom)
        {
            playSound = playerOnClick;
        }

        public ElementControl(Action playerOnClick, Image image, int width, int height, bool zoom = true)
            : this(null, null, playerOnClick, image, width, height, zoom) { }

        public void ShowImage(Point imagePosition)
        {
            Position = imagePosition;
        }
        
        public void ShowDescription(Point descriptionPosition)
        {
            if (Description != null) 
                this.descriptionPosition = descriptionPosition;
        }

        public void Hide()
        {
            Position = Form1.Beyond;
            descriptionPosition = Form1.Beyond;
        }

        public void PlaySound()
        {
            if (playSound is null) return;
            playSound();
        }

        public new void Paint(Graphics graphics)
        {
            base.Paint(graphics);
            if (Description != null)
            {
                if (stringFormat == null)
                    graphics.DrawString(Description, StringStyle.TitleFont, brush, new Rectangle
                        (descriptionPosition, textAreaSize));
                else
                    graphics.DrawString(Description, StringStyle.TitleFont, brush, new Rectangle
                        (descriptionPosition, textAreaSize), stringFormat);
            }
        }

        public void SetTextAreaSize(Size value) => textAreaSize = value;

        public void SetStringFormat(StringFormat value) => stringFormat = value;
    }
}
