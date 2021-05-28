using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WMPLib;

namespace NewspaperRuler
{
    public class WorkTable : IUserInterface
    {
        private Article currentArticle;
        private Note currentNote;

        private Waiting outPaper;

        private GraphicObject paper;
        private GraphicObject providedStamp;
        private readonly Stamp approved;
        private readonly Stamp rejected;
        private bool stampsAreVisible;

        private readonly NotificationPanel notifications;
        private readonly InformationPanel decrees;
        private readonly Remark remark;

        private bool paperIsEntering;
        private bool levelCompleted;

        private readonly ElementControl decreesBook = new ElementControl("ПРИКАЗЫ", Properties.Resources.Book, 120, 100);

        private readonly Sounds sounds;
        private readonly Stats stats;
        private readonly DayEnd dayEnd;
        private readonly Form1 form;

        private readonly Action<Interface> changeInterface;

        public WorkTable(Form1 form, Sounds sounds, Stats stats, DayEnd dayEnd, Action<Interface> changeInterface)
        {
            this.sounds = sounds;
            this.form = form;
            this.stats = stats;
            this.dayEnd = dayEnd;
            this.changeInterface = changeInterface;
            dayEnd.CreateEventClickOnContinue(MouseDownOnNextDayButton);

            approved = new Stamp(Properties.Resources.Approved, 300, 250);
            rejected = new Stamp(Properties.Resources.Rejected, 300, 250);
            providedStamp = new GraphicObject(Properties.Resources.Approved, 300, 250, Form1.Beyond);

            notifications = new NotificationPanel(Scale.Resolution, sounds.Notification);
            decrees = new InformationPanel(Properties.Resources.Frame, 500, 800, new Point(-Scale.Get(500), 0), sounds.PanelShow, sounds.PanelHide);
            remark = new Remark(Properties.Resources.RemarkBackground, 600, 300, sounds);

            RemoveStamps();
            stats.GoToNextLevel();
            NextEvent();
        }

        public void Paint(Graphics graphics)
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

        public void EveryTick()
        {
            sounds.EveryTick();
            notifications.EveryTick();
            decrees.EveryTick();
            remark.EveryTick();
            dayEnd.EveryTick();
            outPaper?.EveryTick();
            paper.Move();
            providedStamp.Move();
            CheckPaperPosition();

            if (decreesBook.IsVisible)
            {
                if (decreesBook.CursorIsHovered())
                    decrees.Show();
                else decrees.Hide();
            }

            if (levelCompleted && !remark.Enabled)
                FinishLevel();
        }

        public void MouseDown()
        {
            approved.MouseDown(sounds.StampTake);
            rejected.MouseDown(sounds.StampTake);
            remark.MouseDown();
        }

        public void MouseUp()
        {
            approved.MouseUp();
            rejected.MouseUp();
            if (approved.OnPaper(paper)) SendArticle(true);
            else if (rejected.OnPaper(paper)) SendArticle(false);
            else if (stampsAreVisible)
            {
                approved.Return(sounds.StampReturn);
                rejected.Return(sounds.StampReturn);
            }
        }

        public void MouseMove()
        {
            approved.MouseMove();
            rejected.MouseMove();
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
            changeInterface(Interface.WorkTable);
            AddNewElementsToLevel();
            NextEvent();
        }

        private void NextEvent()
        {
            if (stats.Level.Events.Count == 0)
            {
                levelCompleted = true;
                return;
            }

            var _event = stats.Level.Events.Dequeue();

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
            paper.Position = new Point(-800, (int)(10 / Scale.Factor));
            paperIsEntering = true;
            paper.GoRight();
        }

        private void CheckPaperPosition()
        {
            if (!paper.IsMoving) return;
            if (paperIsEntering)
            {
                var center = Scale.Resolution.Width / 2 - paper.Bitmap.Width / 2;
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
            else if (paper.IsMoving && (paper.Position.X > Scale.Resolution.Width || paper.Position.X < -paper.Bitmap.Width))
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
            if (isApproved) ApproveArticle();
            else RejectArticle();

            sounds.StampPut();
            RemoveStamps();
            outPaper.WaitAndExecute(10);
        }

        private void ApproveArticle()
        {
            providedStamp = new GraphicObject(approved.Bitmap, approved.Bitmap.Size - new Size(50, 50), approved.Position + new Size(25, 25), false);
            outPaper = new Waiting(new Action(() => { paper.GoRight(); providedStamp.GoRight(); }));
            if (currentArticle.Flag != "") stats.SetFlagToTrue(currentArticle.Flag);
            stats.Level.IncreaseLoyality(currentArticle.Loyality);
            stats.Level.IncreaseReprimandScore(currentArticle.ReprimandScore);
            if (currentArticle.Title != "") notifications.Add($"В сегодняшнем выпуске: {currentArticle.Title}");
            else notifications.Add($"В сегодняшнем выпуске: {currentArticle.ExtractBeginning()}...");
            CheckForMistake();
        }

        private void RejectArticle()
        {
            providedStamp = new GraphicObject(rejected.Bitmap, rejected.Bitmap.Size - new Size(50, 50), rejected.Position + new Size(25, 25), false);
            outPaper = new Waiting(new Action(() => { paper.GoLeft(); providedStamp.GoLeft(); }));
            if (currentArticle.Loyality != 0) stats.Level.IncreaseLoyality(-1);
        }

        private void SendNote()
        {
            paper.GoLeft();
            currentNote.HideButtons(form.Controls);
        }

        private void CreateStamps()
        {
            approved.SetPosition(new Point(paper.Position.X - approved.Bitmap.Width, Scale.Get(300)));
            rejected.SetPosition(new Point(paper.Position.X + paper.Bitmap.Width, Scale.Get(300)));
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
            switch (stats.LevelNumber)
            {
                case 2:
                    decreesBook.ShowImage(new Point(0, Scale.Resolution.Height - decreesBook.Bitmap.Height));
                    decreesBook.ShowDescription(new Point(decreesBook.Position.X + decreesBook.Bitmap.Width + Scale.Get(10), decreesBook.Position.Y + decreesBook.Bitmap.Height / 2));
                    //decrees.Add("№ 34.10. Отклонять статьи пессимистического характера");
                    //decrees.Add("№ 34.11. Отклонять статьи без заголовка");
                    break;
                    //case 3:
                    //    decrees.RemoveAt(0);
                    //    decrees.Add("№ 34.12. Запрещены упоминания о войне");
                    //    decrees.Add("№ 34.13. Отклонять статьи об умышленных убийствах");
                    //    break;
            }
            var newDecrees = stats.GetDecrees();
            decrees.Clear();
            decrees.Add(newDecrees);
        }

        private void CheckForMistake()
        {
            var remarkText = new StringBuilder();
            switch (currentArticle.Mistake)
            {
                case Mistake.NoTitle: remarkText.Append("Статья без заголовка."); break;
                case Mistake.PessimisticArticle: remarkText.Append("Статьи пессимистического характера подлежат отказу."); break;
                case Mistake.PremeditatedMurder: remarkText.Append("Упоминания об умышленных убийствах запрещены."); break;
                case Mistake.War: remarkText.Append("Упоминания о войне запрещены."); break;
                default: return;
            }
            if (currentArticle.ReprimandScore == 0)
            {
                stats.Level.IncreaseFine();
                if (stats.Level.CurrentFine == 0)
                    remarkText.Append("\n\nПри повторных ошибках будет наложен штраф.");
                else remarkText.Append($"\n\nШтраф: {stats.Level.CurrentFine} ТОКЕНОВ");
            }
            else remarkText.Append("\n\nЗамечание без штрафа.");
            remark.Add(remarkText.ToString());
        }

        private void FinishLevel()
        {
            levelCompleted = false;
            stats.FinishLevel();
            dayEnd.ShowAll();
            changeInterface(Interface.DayEnd);
        }
    }
}
