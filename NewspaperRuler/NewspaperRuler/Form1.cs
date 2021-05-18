using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public partial class Form1 : Form
    {
        public static Point Beyond { get; } = new Point(-5000, 0);

        private readonly WorkTable workTable;

        public Form1()
        {
            InitializeComponent();
            ArticleConstructor.Initialize();
            DoubleBuffered = true;

            var stats = new Stats(20);
            workTable = new WorkTable(Controls, stats);

            Paint += new PaintEventHandler(OnPaint);
            MouseDown += new MouseEventHandler(OnMouseDown);
            MouseUp += new MouseEventHandler(OnMouseUp);
            MouseMove += new MouseEventHandler(OnMouseMove);

            var graphicsUpdate = new Timer() { Interval = 40 };
            graphicsUpdate.Tick += new EventHandler(OnTick);
            graphicsUpdate.Start();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            workTable.Paint(e.Graphics);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.WindowState = FormWindowState.Maximized;
            this.BackgroundImage = Properties.Resources.Background;
        }

        private void OnTick(object sender, EventArgs e)
        {
            Invalidate();
            workTable.Tick();
        }

        private void OnMouseDown(object sender, MouseEventArgs e) => workTable.MouseDown(e);

        private void OnMouseUp(object sender, MouseEventArgs e) => workTable.MouseUp(e);

        private void OnMouseMove(object sender, MouseEventArgs e) => workTable.MouseMove();
    }
}
