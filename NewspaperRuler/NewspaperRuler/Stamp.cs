using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class Stamp
    {
        public Bitmap Bitmap { get; }
        public Point Position { get; set; }
        private Point initialPosition;
        private bool isDraggable = false;

        public Stamp(Bitmap bitmap, Point position)
        {
            initialPosition = position;
            Position = position;
            Bitmap = bitmap;
        }

        public Stamp(Bitmap bitmap) : this(bitmap, Form1.Beyond) { }

        public void MouseDown(MouseEventArgs e, Action playSound)
        {
            if (CursorOnStamp(e))
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

        public void Paint(Graphics graphics) => graphics.DrawImage(Bitmap, Position);

        public bool OnPaper(GraphicObject paper) =>
            Position.X >= paper.Position.X + Bitmap.Width / 4
            && Position.X <= paper.Position.X + paper.Bitmap.Width - Bitmap.Width
            && Position.Y >= paper.Position.Y + Bitmap.Height / 4
            && Position.Y <= paper.Position.Y + paper.Bitmap.Height - Bitmap.Height;

        public void SetInitialPosition()
        {
             Position = initialPosition;
        }

        public void MoveToInitialPosition(MouseEventArgs e, Action playSound)
        {
            if (!CursorOnStamp(e)) return;
            SetInitialPosition();
            playSound();
        }

        private bool CursorOnStamp(MouseEventArgs e) => AuxiliaryMethods.IsClickedOnArea(e.Location, Position, Bitmap.Size);

        public void SetPosition(Point value)
        {
            Position = value;
            initialPosition = value;
        }
    }
}
