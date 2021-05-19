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

        private readonly string description;
        private Point descriptionPosition;

        public ElementControl(string description, Image image, int width, int height, bool zoom = true) 
            : base(image, width, height, zoom)
        {
            this.description = description;
            Hide();
        }

        public ElementControl(Image image, int width, int height, bool zoom = true)
            : base(image, width, height, Form1.Beyond, zoom) { }

        public void ShowImage(Point imagePosition)
        {
            Position = imagePosition;
        }
        
        
        public void ShowDescription(Point descriptionPosition)
        {
            if (description != null) 
                this.descriptionPosition = descriptionPosition;
        }

        public void Hide()
        {
            Position = Form1.Beyond;
            descriptionPosition = Form1.Beyond;
        }

        public bool CursorIsHovered() =>
            AuxiliaryMethods.IsClickedOnArea(Cursor.Position, Position, Bitmap.Size);

        public new void Paint(Graphics graphics)
        {
            base.Paint(graphics);
            if (description != null)
                graphics.DrawString(description, StringStyle.TitleFont, StringStyle.White, new Rectangle(
                    descriptionPosition, new Size(Scl.Get(200), Scl.Get(300))));
        }
    }
}
