using System.Drawing;

namespace NewspaperRuler
{
    public class TextBoxControl : GraphicObject
    {
        private readonly string text;
        private readonly Size textShift;
        private readonly Font font;
        private readonly SolidBrush brush;

        public TextBoxControl(Image image, int width, int height, int textShiftX, int textShiftY, string text,
            Font font, SolidBrush brush)
            : base(image, width, height, Form1.Beyond, 50)
        {
            this.font = font;
            this.brush = brush;
            this.text = text;
            textShift = new Size(Scale.Get(textShiftX), Scale.Get(textShiftY));
        }

        public new void Paint(Graphics graphics)
        {
            base.Paint(graphics);
            graphics.DrawString(text, font, brush, new Rectangle
                (Position.X + textShift.Width, Position.Y + textShift.Height, Bitmap.Width - 2 * textShift.Width, Bitmap.Height),
                new StringFormat { Alignment = StringAlignment.Center });
        }

        public void EveryTick()
        {
            Move();
            if (!IsMoving) return;
            if (Position.X > Scale.Get(100))
                Stop(new Point(Scale.Get(100), Position.Y));
            else if (Position.X < 0)
                Stop(new Point(-Bitmap.Width, Position.Y));
        }
    }
}
