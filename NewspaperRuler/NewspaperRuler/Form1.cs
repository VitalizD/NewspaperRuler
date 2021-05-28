using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public partial class Form1 : Form
    {
        public static Point Beyond { get; } = new Point(-5000, 0);

        private readonly Controller controller;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;

            controller = new Controller(this);

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackgroundImage = Properties.Resources.Background;

            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
            KeyDown += OnKeyDown;

            var graphicsUpdate = new Timer { Interval = 40 };
            graphicsUpdate.Tick += OnTick;
            graphicsUpdate.Start();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            controller.Paint(e.Graphics);
        }

        private void OnTick(object sender, EventArgs e)
        {
            controller.EveryTick();
            Invalidate();
        }

        private void OnMouseDown(object sender, MouseEventArgs e) => controller.MouseDown();

        private void OnMouseUp(object sender, MouseEventArgs e) => controller.MouseUp();

        private void OnMouseMove(object sender, MouseEventArgs e) => controller.MouseMove();

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
