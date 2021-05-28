using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class Stamp : GraphicObject
    {
        private Point initialPosition;
        private bool isDraggable = false;

        public Stamp(Image image, int width, int height, Point position) 
            : base(image, width, height, position)
        {
            initialPosition = position;
        }

        public Stamp(Image image, int width, int height) : this(image, width, height, Form1.Beyond) { }

        public void MouseDown(Action playSound)
        {
            if (CursorOnStamp())
            {
                playSound();
                isDraggable = true;
            }
        }

        public void MouseUp()
        {
            if (!isDraggable) return;
            isDraggable = false;
        }

        public void MouseMove()
        {
            if (isDraggable) 
                Position = new Point(Cursor.Position.X - Bitmap.Width / 2, Cursor.Position.Y - Bitmap.Height / 2);
        }

        public bool OnPaper(GraphicObject paper) =>
            Position.X >= paper.Position.X + Bitmap.Width / 4
            && Position.X <= paper.Position.X + paper.Bitmap.Width - Bitmap.Width
            && Position.Y >= paper.Position.Y + Bitmap.Height / 4
            && Position.Y <= paper.Position.Y + paper.Bitmap.Height - Bitmap.Height;

        public void SetInitialPosition()
        {
             Position = initialPosition;
        }

        public void Return(Action playSound)
        {
            if (!CursorOnStamp()) return;
            SetInitialPosition();
            playSound();
        }

        private bool CursorOnStamp() 
            => AuxiliaryMethods.IsClickedOnArea(new Rectangle(Position, Bitmap.Size));

        public void SetPosition(Point value)
        {
            Position = value;
            initialPosition = value;
        }
    }
}
