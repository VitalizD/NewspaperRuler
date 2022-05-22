using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace NewspaperRuler
{
    public class DayEnd : IUserInterface
    {
        public List<Label> StatsTexts { get; } = new List<Label>();

        public List<Expense> Expenses { get; } = new List<Expense>();

        public List<Label> InformationTexts { get; private set; } = new List<Label>();

        private readonly Label continueButton;

        private readonly Control.ControlCollection controls;
        private readonly Sounds sounds;

        private readonly Waiting showNextText;

        private bool animationIsProgress;

        private readonly List<Label> showedTexts = new List<Label>();
        private bool showedExpenses;

        public DayEnd(Sounds sounds, Control.ControlCollection controls)
        {
            this.sounds = sounds;
            this.controls = controls;
            showNextText = new Waiting(Print);
            continueButton = new Label
            {
                Text = "СЛЕДУЮЩИЙ ДЕНЬ",
                Font = new Font(StringStyle.FontNameForLabels, Scale.Get(20), FontStyle.Bold),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(Scale.Get(400), Scale.Get(50))
            };
            continueButton.Location = new Point(Scale.Resolution.Width / 2 - Scale.Get(continueButton.Width / 2), Scale.Resolution.Height - continueButton.PreferredHeight - Scale.Get(20));
        }

        public void CreateEventClickOnContinue(MouseEventHandler clickEvent) => continueButton.MouseDown += clickEvent;

        public void RecalculatePositions()
        {
            var x = Scale.Get(100);
            var y = Scale.Get(80);
            Change(InformationTexts);
            y += Scale.Get(30);
            Change(StatsTexts);
            y += Scale.Get(30);
            for (var i = 0; i < Expenses.Count; i++)
            {
                Expenses[i].InitialPosition = new Point(Scale.Resolution.Width - Expenses[i].Selector.Bitmap.Width - Scale.Get(200), y);
                y += Scale.Get(55);
            }

            void Change(List<Label> texts)
            {
                foreach (var text in texts)
                {
                    text.Location = new Point(x, y);
                    y += text.PreferredHeight;
                }
            }
        }

        public void Paint(Graphics graphics)
        {
            graphics.DrawString("ИТОГИ ДНЯ", StringStyle.TitleFont, StringStyle.White, new Point(Scale.Resolution.Width / 2 - Scale.Get(125), Scale.Get(10)));
            foreach (var label in showedTexts)
                graphics.DrawString(label.Text, label.Font, new SolidBrush(label.ForeColor), label.Location);
            if (showedExpenses)
                foreach (var expense in Expenses)
                    expense.Paint(graphics);
        }

        public void EveryTick()
        {
            showNextText.EveryTick();
        }

        public int MouseDown(int money)
        {
            var increaseInMoney = 0;
            foreach (var _switch in Expenses)
            {
                if (_switch.Selector.CursorIsHovered())
                {
                    increaseInMoney = _switch.SetMark(money);
                    foreach (var stat in StatsTexts)
                    {
                        if (stat.Text.Substring(0, 5) == "Итого")
                        {
                            stat.Text = $"Итого:\t\t{money + increaseInMoney} {Stats.MonetaryCurrencyName}";
                            break;
                        }
                    }
                    break;
                }
            }
            return increaseInMoney;
        }

        public void MouseDown() { }

        public void ShowAll()
        {
            if (animationIsProgress) return;
            animationIsProgress = true;
            showNextText.WaitAndExecute(10);
        }

        private int iterator = 0;
        private bool flag;
        private void Print()
        {
            showNextText.WaitAndExecute(10);
            sounds.PlayPoint();
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
                showNextText.Cancel();
                controls.Add(continueButton);
                ShowExpenses();
                sounds.StopPanelHide();
                sounds.PlaySuddenness();
            }
            iterator++;
        }

        private void ShowExpenses()
        {
            for (var i = 0; i < Expenses.Count; i++)
            {
                Expenses[i].Selector.ShowImage(Expenses[i].InitialPosition);
                Expenses[i].Selector.ShowDescription(new Point(Scale.Get(100), Expenses[i].InitialPosition.Y));
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

        public void MouseUp() { }

        public void MouseMove() { }

        public void ExecuteAfterTransition(Form1 form)
        {
            form.BackgroundImage = null;
            form.BackColor = Color.Black;
        }
    }
}
