using System.Drawing;

namespace NewspaperRuler
{
    public class GraphicObject
    {
        public Bitmap Bitmap { get; set; }
        public Point Position { get; set; }

        public bool IsMoving { get { return shift != new Size(0, 0); } }

        private Size shift = new Size(0, 0);
        private readonly int movementSpeed;

        public GraphicObject(Image image, int width, int height, Point position, int movementSpeed, bool zoom = true)
        {
            if (image == null) Bitmap = null;
            else if (zoom) Bitmap = new Bitmap(image, Scl.Get(width), Scl.Get(height));
            else Bitmap = new Bitmap(image, width, height);
            Position = position;
            this.movementSpeed = movementSpeed;
        }

        public GraphicObject(Image image, int width, int height, Point position, bool zoom = true)
            : this(image, width, height, position, 125, zoom) { }

        public GraphicObject(Image image, Size size, Point position, bool zoom = true) 
            : this(image, size.Width, size.Height, position, zoom) { }

        public GraphicObject(Image image, Size size, Point position, int movementSpeed, bool zoom = true)
            : this(image, size.Width, size.Height, position, movementSpeed, zoom) { }

        public GraphicObject(Image image, int width, int height, bool zoom = true) 
            : this(image, width, height, new Point(), zoom) { }

        public GraphicObject(Image image, int width, int height, int movementSpeed, bool zoom = true) 
            : this(image, width, height, new Point(), movementSpeed, zoom) { }

        public GraphicObject(Point position) : this(null, 1, 1, position) { }

        public GraphicObject(Point position, int movementSpeed) : this(null, 1, 1, position, movementSpeed) { }

        public void Paint(Graphics graphics)
        {
            if (Bitmap is null) return;
            graphics.DrawImage(Bitmap, Position);
        }

        public void GoLeft() => shift = new Size(-movementSpeed, 0);

        public void GoRight() => shift = new Size(movementSpeed, 0);

        public void GoDown() => shift = new Size(0, movementSpeed);

        public void GoUp() => shift = new Size(0, -movementSpeed);

        public void Stop() => shift = new Size(0, 0);

        public void Move() => Position += shift;
    }
}
