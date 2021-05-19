﻿using System;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace NewspaperRuler
{
    public class WorkTable
    {
        private readonly LevelController paper;
        private readonly Sounds sounds = new Sounds();

        public WorkTable(Control.ControlCollection controls, Stats stats)
        {
            paper = new LevelController(controls, stats, sounds);
        }

        public void Paint(Graphics graphics)
        {
            paper.Paint(graphics);
        }

        public void Tick()
        {
            paper.Tick();
        }

        public void MouseDown(MouseEventArgs e)
        {
            paper.MouseDown(e);
        }

        public void MouseUp(MouseEventArgs e)
        {
            paper.MouseUp(e);
        }

        public void MouseMove()
        {
            paper.MouseMove();
        }
    }
}
