using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace NewspaperRuler
{
    public class DayEnd
    {
        public List<Label> StatsTexts { get; } = new List<Label>();

        public List<Expense> Expenses { get; } = new List<Expense>();

        public List<Label> InformationTexts { get; private set; } = new List<Label>();

        private readonly Label continueButton;

        private readonly Control.ControlCollection controls;
        private readonly Sounds sounds;

        private int waitBeforeShowingNextText = 0;

        private bool animationIsProgress;

        private readonly List<Label> showedTexts = new List<Label>();
        private bool showedExpenses;

        public DayEnd(Sounds sounds, Control.ControlCollection controls)
        {
            this.sounds = sounds;
            this.controls = controls;
            continueButton = new Label
            {
                Text = "СЛЕДУЮЩИЙ ДЕНЬ",
                Font = new Font(StringStyle.FontName, 20, FontStyle.Bold),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(Scl.Get(400), Scl.Get(50))
            };
            continueButton.Location = new Point(Scl.Resolution.Width / 2 - Scl.Get(continueButton.Width / 2), Scl.Resolution.Height - continueButton.PreferredHeight - Scl.Get(20));
        }

        public void CreateEventClickOnContinue(MouseEventHandler clickEvent) => continueButton.MouseDown += clickEvent;

        public void RecalculatePositions()
        {
            var x = Scl.Get(100);
            var y = Scl.Get(80);
            Change(InformationTexts);
            y += Scl.Get(50);
            Change(StatsTexts);
            y += Scl.Get(50);
            for (var i = 0; i < Expenses.Count; i++)
            {
                Expenses[i].InitialPosition = new Point(Scl.Resolution.Width - Expenses[i].Selector.Bitmap.Width - Scl.Get(200), y);
                y += Scl.Get(55);
            }

            void Change(List<Label> texts)
            {
                foreach (var text in texts)
                {
                    text.Location = new Point(x, y);
                    y += text.PreferredHeight + Scl.Get(5);
                }
            }
        }

        public void Paint(Graphics graphics)
        {
            graphics.DrawString("ИТОГИ ДНЯ", StringStyle.TitleFont, StringStyle.White, new Point(Scl.Resolution.Width / 2 - Scl.Get(125), Scl.Get(10)));
            foreach (var label in showedTexts)
                graphics.DrawString(label.Text, label.Font, new SolidBrush(label.ForeColor), label.Location);
            if (showedExpenses)
                foreach (var expense in Expenses)
                    expense.Paint(graphics);
        }

        public void Tick()
        {
            if (waitBeforeShowingNextText > 0)
            {
                waitBeforeShowingNextText--;
                if (waitBeforeShowingNextText == 0)
                    Print();
            }
        }

        public void MouseDown(ref int money)
        {
            foreach (var _switch in Expenses)
            {
                if (_switch.Selector.CursorIsHovered())
                {
                    _switch.SetMark(ref money);
                    foreach (var stat in StatsTexts)
                        if (stat.Text.Substring(0, 5) == "Итого")
                            stat.Text = $"Итого:\t\t{money} {Stats.MonetaryCurrencyName}";
                }
            }
        }

        public void ShowAll()
        {
            if (animationIsProgress) return;
            animationIsProgress = true;
            waitBeforeShowingNextText = 10;
        }

        private int iterator = 0;
        private bool flag;
        private void Print()
        {
            waitBeforeShowingNextText = 10;
            if (!flag)
            {
                if (iterator == InformationTexts.Count || InformationTexts.Count == 0)
                {
                    flag = true;
                    iterator = -1;
                }
                else showedTexts.Add(InformationTexts[iterator]);
            }
            else if (iterator < StatsTexts.Count) showedTexts.Add(StatsTexts[iterator]);
            else
            {
                iterator = -1;
                animationIsProgress = false;
                showedExpenses = true;
                waitBeforeShowingNextText = 0;
                controls.Add(continueButton);
                ShowExpenses();
            }
            sounds.PanelHide();
            iterator++;
        }

        private void ShowExpenses()
        {
            for (var i = 0; i < Expenses.Count; i++)
            {
                Expenses[i].Selector.ShowImage(Expenses[i].InitialPosition);
                Expenses[i].Selector.ShowDescription(new Point(Scl.Get(100), Expenses[i].InitialPosition.Y));
            }
        }

        public void Reset()
        {
            InformationTexts.Clear();
            StatsTexts.Clear();
            Expenses.Clear();
            showedExpenses = false;
            flag = false;
            showedTexts.Clear();
            controls.Remove(continueButton);
        }
    }
}
