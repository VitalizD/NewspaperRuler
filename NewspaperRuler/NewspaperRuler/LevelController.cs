using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class LevelController
    {
        private readonly Sounds sounds;
        private readonly Stats stats;
        private readonly Form1 form;

        private bool paperIsEntering;

        private Interface currentInterface = Interface.WorkTable;

        private Article currentArticle;
        private Note currentNote;

        private int waitBeforeOutPaper = 0;
        private Action movePaperToSide;

        private GraphicObject paper;
        private GraphicObject providedStamp = new GraphicObject(Properties.Resources.Approved, 300, 250, Form1.Beyond);
        // Эти инициализваторы не работают, т.к. в конструкторе эти поля перетираются
        private readonly Stamp approved = new Stamp(new Bitmap(Properties.Resources.Approved, 300, 250));
        private readonly Stamp rejected = new Stamp(new Bitmap(Properties.Resources.Rejected, 300, 250));
        private bool stampsAreVisible;

        private readonly NotificationPanel notifications;
        private readonly InformationPanel decrees;
        private readonly Remark remark;

        private readonly ElementControl decreesBook = new ElementControl("ПРИКАЗЫ", Properties.Resources.Book, 120, 100);

        private readonly DayEnd dayEnd;

        private bool levelCompleted;

        public LevelController(Form1 form, Sounds sounds)
        {
            this.sounds = sounds;
            this.form = form;

            approved = new Stamp(new Bitmap(Properties.Resources.Approved, Scl.Get(300), Scl.Get(250)));
            rejected = new Stamp(new Bitmap(Properties.Resources.Rejected, Scl.Get(300), Scl.Get(250)));

            notifications = new NotificationPanel(new Point(0, -Scl.Get(120)), Scl.Resolution, this.sounds.Notification);
            decrees = new InformationPanel(Properties.Resources.Frame, 500, 800, new Point(-Scl.Get(500), 0), sounds.PanelShow, sounds.PanelHide);
            remark = new Remark(Properties.Resources.RemarkBackground, 600, 300, sounds);

            dayEnd = new DayEnd(sounds, form.Controls);
            dayEnd.CreateEventClickOnContinue(MouseDownOnNextDayButton);

            stats = new Stats(100, dayEnd);

            ChangeInterface(Interface.WorkTable);
            RemoveStamps();
            stats.GoToNextLevel();
            NextEvent(false);
        }

        public void Paint(Graphics graphics)
        {
            // лучше отрисовку сделать на полиморфизме, а не на Enum. Можно завести интерфейс IUserInterface
            // с методом Paint(Graphics graphics)
            // и две реализации - DayEnd и WorkTable
            if (currentInterface is Interface.WorkTable)
            {
                paper.Paint(graphics);
                approved.Paint(graphics);
                rejected.Paint(graphics);
                providedStamp.Paint(graphics);
                currentArticle?.Paint(graphics);
                currentNote?.Paint(graphics);
                notifications.Paint(graphics);
                decreesBook.Paint(graphics);
                decrees.Paint(graphics);
                remark.Paint(graphics);
            }
            else if (currentInterface is Interface.DayEnd) dayEnd.Paint(graphics);
        }

        public void MouseDown(MouseEventArgs e)
        {
            approved.MouseDown(e, sounds.StampTake);
            rejected.MouseDown(e, sounds.StampTake);
            remark.MouseDown();

            var money = stats.Money;
            dayEnd.MouseDown(ref money);
            stats.Money = money;
        }

        private void MouseDownOnOption(object sender, MouseEventArgs e)
        {
            sounds.ChooseOption();
            var label = sender as Label;
            if (label == currentNote.PositiveOption)
            {
                stats.SetFlagToTrue(currentNote.Flag);
                notifications.Add(currentNote.PositiveMessage);
            }
            else if (label == currentNote.NegativeOption)
                notifications.Add(currentNote.NegativeMessage);
            SendNote();
        }

        private void MouseDownOnNextDayButton(object sender, MouseEventArgs e)
        {
            sounds.BeginLevel();
            stats.ApplyExpenses();
            dayEnd.Reset();
            stats.GoToNextLevel();
            ChangeInterface(Interface.WorkTable);
            AddNewElementsToLevel();
            NextEvent(false);
        }

        public void MouseUp(MouseEventArgs e)
        {
            approved.MouseUp();
            rejected.MouseUp();
            if (approved.OnPaper(paper)) SendArticle(true);
            else if (rejected.OnPaper(paper)) SendArticle(false);
            else if (stampsAreVisible)
            {
                approved.MoveToInitialPosition(e, sounds.StampReturn);
                rejected.MoveToInitialPosition(e, sounds.StampReturn);
            }
        }

        public void MouseMove()
        {
            approved.MouseMove();
            rejected.MouseMove();
        }

        private void NextEvent(bool isRandom = true)
        {
            if (stats.Level.Events.Count == 0) 
            {
                levelCompleted = true;
                return;
            }
            var index = 0;
            if (isRandom) index = new Random().Next(stats.Level.Events.Count);
            var _event = stats.Level.Events[index];
            stats.Level.Events.RemoveAt(index);
            if (_event.GetType() == typeof(Article))
            {
                var article = _event as Article;
                paper = article.Background;
                StartEvent(article);
            }
            else if (_event.GetType() == typeof(Note))
            {
                var note = _event as Note;
                paper = note.Background;
                StartEvent(note);
            }
        }

        private void StartEvent(Article article)
        {
            currentArticle = article;
            paper.Bitmap = article.Background.Bitmap;
            EnterPaper();
        }

        private void StartEvent(Note note)
        {
            currentNote = note;
            paper.Bitmap = note.Background.Bitmap;
            currentNote.CreateClickEventForOptions(MouseDownOnOption);
            EnterPaper();
        }

        private void EnterPaper()
        {
            sounds.Paper();
            paper.Position = new Point(-800, (int)(10 / Scl.Factor));
            paperIsEntering = true;
            paper.GoRight();
        }

        public void Tick()
        {
            sounds.Tick();
            notifications.Tick();
            decrees.Tick();
            remark.Tick();
            dayEnd.Tick();
            paper.Move();
            providedStamp.Move();
            CheckPaperPosition();

            // здесь стоит выделить компонент для ожидания, иначе в других мастах будет аналогичный код.
            if (waitBeforeOutPaper > 0)
            {
                waitBeforeOutPaper -= 1;
                if (waitBeforeOutPaper == 0) 
                    movePaperToSide();
            }

            if (decreesBook.IsVisible)
            {
                if (decreesBook.CursorIsHovered())
                    decrees.Show();
                else decrees.Hide();
            }

            if (levelCompleted && !remark.Enabled)
                FinishLevel();
        }

        private void CheckPaperPosition()
        {
            if (!paper.IsMoving) return;
            if (paperIsEntering)
            {
                var center = Scl.Resolution.Width / 2 - paper.Bitmap.Width / 2;
                if (paper.Position.X > center)
                {
                    sounds.StampEnter();
                    paperIsEntering = false;
                    paper.Position = new Point(center, paper.Position.Y);
                    paper.Stop();
                    if (currentArticle != null) CreateStamps();
                    else currentNote?.ShowButtons(form.Controls);
                }
            }
            else if (paper.IsMoving && (paper.Position.X > Scl.Resolution.Width || paper.Position.X < -paper.Bitmap.Width))
            {
                notifications.Show();
                remark.Show();
                currentArticle = null;
                currentNote = null;
                paper.Stop();
                providedStamp.Stop();
                NextEvent();
            }
        }

        private void SendArticle(bool isApproved)
        {
            // по сути этот метод - это два разных метода и лучше их разделить
            if (isApproved)
            {
                providedStamp = new GraphicObject(approved.Bitmap, approved.Bitmap.Size - new Size(50, 50), approved.Position + new Size(25, 25), false);
                movePaperToSide = new Action(() => { paper.GoRight(); providedStamp.GoRight(); });
                if (currentArticle.Flag != "") stats.SetFlagToTrue(currentArticle.Flag);
                stats.Level.IncreaseLoyality(currentArticle.Loyality);
                stats.Level.IncreaseReprimandScore(currentArticle.ReprimandScore);
                if (currentArticle.Title != "") notifications.Add($"В сегодняшнем выпуске: {currentArticle.Title}");
                else notifications.Add($"В сегодняшнем выпуске: {currentArticle.ExtractBeginning()}...");
                CheckForMistake();
            }
            else
            {
                providedStamp = new GraphicObject(rejected.Bitmap, rejected.Bitmap.Size - new Size(50, 50), rejected.Position + new Size(25, 25), false);
                movePaperToSide = new Action(() => { paper.GoLeft(); providedStamp.GoLeft(); });
                if (currentArticle.Loyality != 0) stats.Level.IncreaseLoyality(-1);
            }
            sounds.StampPut();
            RemoveStamps();
            waitBeforeOutPaper = 10;
        }

        private void SendNote()
        {
            paper.GoLeft();
            currentNote.HideButtons(form.Controls);
        }

        private void CreateStamps()
        {
            approved.SetPosition(new Point(paper.Position.X - approved.Bitmap.Width, Scl.Get(300)));
            rejected.SetPosition(new Point(paper.Position.X + paper.Bitmap.Width, Scl.Get(300)));
            stampsAreVisible = true;
        }

        private void RemoveStamps()
        {
            approved.Position = Form1.Beyond;
            rejected.Position = Form1.Beyond;
            stampsAreVisible = false;
        }

        private void AddNewElementsToLevel()
        {
            // эти вещи стоит унести в ресурсы как и текст уровней
            switch (stats.LevelNumber)
            {
                case 2:
                    decreesBook.ShowImage(new Point(0, Scl.Resolution.Height - decreesBook.Bitmap.Height));
                    decreesBook.ShowDescription(new Point(decreesBook.Position.X + decreesBook.Bitmap.Width + Scl.Get(10), decreesBook.Position.Y + decreesBook.Bitmap.Height / 2));
                    decrees.Add("№ 34.10. Отклонять статьи пессимистического характера");
                    decrees.Add("№ 34.11. Отклонять статьи без заголовка");
                    break;
            }
        }

        private void CheckForMistake()
        {
            var remarkText = new StringBuilder();
            switch (currentArticle.Mistake)
            {
                case Mistake.NoTitle: remarkText.Append("Статья без заголовка.\n\n"); break;
                default: return;
            }
            stats.Level.IncreaseFine();
            if (stats.Level.CurrentFine == 0)
                remarkText.Append("При повторной ошибке мы наложим штраф, сумма которого будет вычтена из Вашей заработной платы.");
            else remarkText.Append($"Штраф: {stats.Level.CurrentFine} ТОКЕНОВ");
            remark.Add(remarkText.ToString());
        }

        private void ChangeInterface(Interface value)
        {
            currentInterface = value;
            if (value is Interface.WorkTable)
                form.BackgroundImage = Properties.Resources.Background;
            else if (value is Interface.DayEnd)
            {
                form.BackgroundImage = null;
                form.BackColor = Color.Black;
            }
        }

        private void FinishLevel()
        {
            levelCompleted = false;
            stats.FinishLevel();
            dayEnd.ShowAll();
            ChangeInterface(Interface.DayEnd);
        }
    }
}
