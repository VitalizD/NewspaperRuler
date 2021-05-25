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
            ArticleConstructor.ArticleBackground = new GraphicObject(Properties.Resources.Paper, 720, 970, 125);
            ArticleConstructor.Initialize();

            // Работать со статическими переменными сложно. Лучше сделай NoteBackground экземплярным и передай Stats куда нужно.
            Stats.NoteBackground = new GraphicObject(Properties.Resources.NoteBackground1, 750, 1000, 125);
            StringStyle.Initialize();

            DoubleBuffered = true;

            workTable = new WorkTable(this);

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackgroundImage = Properties.Resources.Background;

            // можно писать короче: так
            Paint += OnPaint;
            MouseDown += new MouseEventHandler(OnMouseDown);
            MouseUp += new MouseEventHandler(OnMouseUp);
            MouseMove += new MouseEventHandler(OnMouseMove);
            KeyDown += new KeyEventHandler(OnKeyDown);

            var graphicsUpdate = new Timer { Interval = 40 };
            graphicsUpdate.Tick += new EventHandler(OnTick);
            graphicsUpdate.Start();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            workTable.Paint(e.Graphics);
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Так ты рисуешь предыдущее состояние.
            Invalidate(); // вызывает перерисовку
            workTable.Tick(); // обновляет состояние
        }

        private void OnMouseDown(object sender, MouseEventArgs e) => workTable.MouseDown(e);

        private void OnMouseUp(object sender, MouseEventArgs e) => workTable.MouseUp(e);

        private void OnMouseMove(object sender, MouseEventArgs e) => workTable.MouseMove();

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
