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
        private readonly Action playSound;
        private Size textAreaSize;

        public ElementControl(string description, Image image, int width, int height, bool zoom = true) 
            : base(image, width, height, zoom)
        {
            this.Description = description;
            textAreaSize = new Size(Scl.Get(200), Scl.Get(300));
            Hide();
        }

        public ElementControl(Image image, int width, int height, bool zoom = true)
            : base(image, width, height, Form1.Beyond, zoom) { }

        public ElementControl(string description, Action playerOnClick, Image image, int width, int height, bool zoom = true)
            : this(description, image, width, height, zoom)
        {
            playSound = playerOnClick;
        }

        public ElementControl(Action playerOnClick, Image image, int width, int height, bool zoom = true)
            : this(null, playerOnClick, image, width, height, zoom) { }

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

        public bool CursorIsHovered() =>
            AuxiliaryMethods.IsClickedOnArea(Cursor.Position, Position, Bitmap.Size);

        public void PlaySound()
        {
            if (playSound is null) return;
            playSound();
        }

        public new void Paint(Graphics graphics)
        {
            base.Paint(graphics);
            if (Description != null)
                graphics.DrawString(Description, StringStyle.TitleFont, StringStyle.White, new Rectangle
                (descriptionPosition, textAreaSize));
        }

        public void SetTextAreaSize(Size value) => textAreaSize = value;
    }
}
