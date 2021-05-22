using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class Note
    {
        private readonly string text;

        public GraphicObject Background { get; }
        public string Flag { get; }
        public Label ContinueButton { get; }
        public Label PositiveOption { get; }
        public Label NegativeOption { get; }
        public string PositiveMessage { get; }
        public string NegativeMessage { get; }

        public Note(GraphicObject background, string text, string continueButton)
        {
            this.text = text;
            Background = background;
            if (continueButton != null)
                this.ContinueButton = GetOption(continueButton, new Size(Scl.Get(300), Scl.Get(50)),
                    new Point(Scl.Resolution.Width / 2 - Scl.Get(150), Scl.Resolution.Height - Scl.Get(60)));
        }

        public Note(GraphicObject background, string text, string positiveOption, string negativeOption, string positiveMessage, string negativeMessage, string flag)
            : this(background, text, null)
        {
            Flag = flag;
            PositiveMessage = positiveMessage;
            NegativeMessage = negativeMessage;
            this.PositiveOption = GetOption(positiveOption, new Size(Scl.Get(300), Scl.Get(50)),
                new Point(Scl.Resolution.Width / 2 - Scl.Get(305), Scl.Resolution.Height - Scl.Get(60)));
            this.NegativeOption = GetOption(negativeOption, new Size(Scl.Get(300), Scl.Get(50)),
                new Point(Scl.Resolution.Width / 2 + Scl.Get(5), Scl.Resolution.Height - Scl.Get(60)));
        }

        private Label GetOption(string text, Size size, Point position)
        {
            return new Label
            {
                Text = text,
                Size = size,
                Location = position,
                Font = StringStyle.TitleFont,
                ForeColor = Color.Brown,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        public void Paint(Graphics graphics)
        {
            graphics.DrawString(text, StringStyle.TextFont, StringStyle.Black, new Rectangle(
                Background.Position + new Size(Scl.Get(30), Scl.Get(30)), Background.Bitmap.Size - new Size(Scl.Get(60), Scl.Get(60))));
        }

        public void HideButtons(Control.ControlCollection controls)
        {
            if (ContinueButton != null) controls.Remove(ContinueButton);
            if (PositiveOption != null) controls.Remove(PositiveOption);
            if (NegativeOption != null) controls.Remove(NegativeOption);
        }

        public void ShowButtons(Control.ControlCollection controls)
        {
            if (ContinueButton != null) controls.Add(ContinueButton);
            if (PositiveOption != null) controls.Add(PositiveOption);
            if (NegativeOption != null) controls.Add(NegativeOption);
        }

        public void CreateClickEventForOptions(MouseEventHandler clickEvent)
        {
            if (ContinueButton != null) ContinueButton.MouseDown += new MouseEventHandler(clickEvent);
            if (NegativeOption != null) NegativeOption.MouseDown += new MouseEventHandler(clickEvent);
            if (PositiveOption != null) PositiveOption.MouseDown += new MouseEventHandler(clickEvent);
        }
    }
}
