using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class LevelController
    {
        private readonly Sounds sounds;

        private GraphicObject paper;

        private readonly Control.ControlCollection controls;

        private bool isEntering;

        private Article currentArticle;
        private Note currentNote;

        private int waitBeforeOutPaper = 0;
        private Action movePaperToSide;

        private GraphicObject providedStamp = new GraphicObject(Properties.Resources.Approved, 300, 250, Form1.Beyond);
        private readonly Stamp approved = new Stamp(new Bitmap(Properties.Resources.Approved, 300, 250));
        private readonly Stamp rejected = new Stamp(new Bitmap(Properties.Resources.Rejected, 300, 250));
        private bool stampsAreVisible;

        private readonly Stats stats;

        private readonly NotificationPanel notifications;
        private readonly InformationPanel decrees;

        private readonly ElementControl decreesBook = new ElementControl("ПРИКАЗЫ", Properties.Resources.Book, 120, 100);

        public LevelController(Control.ControlCollection controls, Stats stats, Sounds sounds)
        {
            this.sounds = sounds;
            this.controls = controls;
            this.stats = stats;

            approved = new Stamp(new Bitmap(Properties.Resources.Approved, Scl.Get(300), Scl.Get(250)));
            rejected = new Stamp(new Bitmap(Properties.Resources.Rejected, Scl.Get(300), Scl.Get(250)));

            notifications = new NotificationPanel(new Point(0, -Scl.Get(120)), Scl.Resolution, this.sounds.Notification);
            decrees = new InformationPanel(Properties.Resources.Frame, 500, 800, new Point(-Scl.Get(500), 0), sounds.PanelShow, sounds.PanelHide);

            RemoveStamps();
            stats.GoToNextLevel();
            NextEvent(false);
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
        }

        public void MouseDown(MouseEventArgs e)
        {
            approved.MouseDown(e, sounds.StampTake);
            rejected.MouseDown(e, sounds.StampTake);
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
                stats.FinishLevel();
                AddNewElementsToLevel();
                NextEvent(false);
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
            paper.Position = new Point(-800, 20);
            isEntering = true;
            paper.GoRight();
        }

        public void Tick()
        {
            sounds.Tick();
            notifications.Tick();
            decrees.Tick();
            paper.Move();
            providedStamp.Move();
            CheckPosition();
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
        }

        private void CheckPosition()
        {
            if (!paper.IsMoving) return;
            if (isEntering)
            {
                var center = Scl.Resolution.Width / 2 - paper.Bitmap.Width / 2;
                if (paper.Position.X > center)
                {
                    sounds.StampEnter();
                    isEntering = false;
                    paper.Position = new Point(center, paper.Position.Y);
                    paper.Stop();
                    if (currentArticle != null) CreateStamps();
                    else currentNote?.ShowButtons(controls);
                }
            }
            else if (paper.IsMoving && (paper.Position.X > Scl.Resolution.Width || paper.Position.X < -paper.Bitmap.Width))
            {
                notifications.Show();
                currentArticle = null;
                currentNote = null;
                paper.Stop();
                providedStamp.Stop();
                NextEvent();
            }
        }

        private void SendArticle(bool isApproved)
        {
            if (isApproved)
            {
                providedStamp = new GraphicObject(approved.Bitmap, approved.Bitmap.Size - new Size(50, 50), approved.Position + new Size(25, 25), false);
                movePaperToSide = new Action(() => { paper.GoRight(); providedStamp.GoRight(); });
                if (currentArticle.Flag != "") stats.SetFlagToTrue(currentArticle.Flag);
                stats.Level.IncreaseLoyality(currentArticle.Loyality);
                stats.Level.IncreaseReprimandScore(currentArticle.ReprimandScore);
                if (currentArticle.Title != "") notifications.Add($"В сегодняшнем выпуске: {currentArticle.Title}");
                else notifications.Add($"В сегодняшнем выпуске: {currentArticle.ExtractBeginning()}...");
            }
            else
            {
                providedStamp = new GraphicObject(rejected.Bitmap, rejected.Bitmap.Size - new Size(50, 50), rejected.Position + new Size(25, 25), false);
                movePaperToSide = new Action(() => { paper.GoLeft(); providedStamp.GoLeft(); });
                stats.Level.IncreaseLoyality(-1);
            }
            sounds.StampPut();
            RemoveStamps();
            waitBeforeOutPaper = 10;
        }

        private void SendNote()
        {
            paper.GoLeft();
            currentNote.HideButtons(controls);
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
    }
}
