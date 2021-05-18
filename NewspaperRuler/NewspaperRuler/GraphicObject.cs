using System.Drawing;

namespace NewspaperRuler
{
    public class GraphicObject
    {
        public Bitmap Bitmap { get; set; }
        public Point Position { get; set; }

        public bool IsMoving { get { return shift != new Size(0, 0); } }

        private Size shift = new Size(0, 0);

        public GraphicObject(Bitmap bitmap, Point position)
        {
            this.Bitmap = bitmap;
            this.Position = position;
        }

        public GraphicObject(Bitmap bitmap) : this(bitmap, new Point()) { }

        public GraphicObject(Point position) : this(null, position) { }

        public GraphicObject(GraphicObject another) : this(another.Bitmap, another.Position) { }

        public void Paint(Graphics graphics)
        {
            if (Bitmap is null) return;
            graphics.DrawImage(Bitmap, Position);
        }

        public void GoLeft() => shift = new Size(-125, 0);

        public void GoRight() => shift = new Size(125, 0);

        public void GoDown() => shift = new Size(0, 5);

        public void GoUp() => shift = new Size(0, -5);

        public void Stop() => shift = new Size(0, 0);

        public void Move() => Position += shift;
    }
}
