using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class Paper
    {
        private readonly Sounds sounds;

        private GraphicObject picture = new GraphicObject(new Bitmap(Properties.Resources.Paper, 650, 900), new Point());

        private readonly Control.ControlCollection controls;

        private bool isEntering;

        private Article currentArticle;
        private Note currentNote;

        private GraphicObject providedStamp = new GraphicObject(Properties.Resources.Approved, Form1.Beyond);

        private int waitBeforeOutPaper = 0;
        private Action movePaperToSide;

        private readonly Stamp approved = new Stamp(new Bitmap(Properties.Resources.Approved, 300, 250), new Point(100, 200));
        private readonly Stamp rejected = new Stamp(new Bitmap(Properties.Resources.Rejected, 300, 250), new Point(1050, 200));
        private bool stampsAreVisible;

        private readonly Stats stats;

        private readonly NotificationPanel notifications;

        public Paper(Control.ControlCollection controls, Stats stats, Sounds sounds)
        {
            this.sounds = sounds;
            this.controls = controls;
            this.stats = stats;
            notifications = new NotificationPanel(new Point(0, -100), this.sounds.Notification);
            RemoveStamps();
            stats.GoToNextLevel();
            NextEvent(false);
        }

        public void Paint(Graphics graphics)
        {
            picture.Paint(graphics);
            approved.Paint(graphics);
            rejected.Paint(graphics);
            providedStamp.Paint(graphics);
            currentArticle?.Paint(graphics);
            currentNote?.Paint(graphics);
            notifications.Paint(graphics);
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
            if (approved.OnPaper(picture)) SendArticle(true);
            else if (rejected.OnPaper(picture)) SendArticle(false);
            else if (stampsAreVisible)
            {
                approved.SetInitialPosition(e, sounds.StampReturn);
                rejected.SetInitialPosition(e, sounds.StampReturn);
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
                picture = article.Background;
                StartEvent(article);
            }
            else if (_event.GetType() == typeof(Note))
            {
                var note = _event as Note;
                picture = note.Background;
                StartEvent(note);
            }
        }

        private void StartEvent(Article article)
        {
            currentArticle = article;
            picture.Bitmap = article.Background.Bitmap;
            EnterPaper();
        }

        private void StartEvent(Note note)
        {
            currentNote = note;
            picture.Bitmap = note.Background.Bitmap;
            currentNote.CreateClickEventForOptions(MouseDownOnOption);
            EnterPaper();
        }

        private void EnterPaper()
        {
            sounds.Paper();
            picture.Position = new Point(-800, 20);
            isEntering = true;
            picture.GoRight();
        }

        public void Tick()
        {
            sounds.Tick();
            notifications.Tick();
            notifications.Move();
            picture.Move();
            providedStamp.Move();
            CheckPosition();
            if (waitBeforeOutPaper > 0)
            {
                waitBeforeOutPaper -= 1;
                if (waitBeforeOutPaper == 0) 
                    movePaperToSide();
            }
        }

        private void CheckPosition()
        {
            if (!picture.IsMoving) return;
            if (isEntering)
            {
                if (picture.Position.X >= 400)
                {
                    sounds.StampEnter();
                    isEntering = false;
                    picture.Position = new Point(400, picture.Position.Y);
                    picture.Stop();
                    if (currentArticle != null) CreateStamps();
                    else currentNote?.ShowButtons(controls);
                }
            }
            else if (picture.IsMoving && (picture.Position.X >= 2000 || picture.Position.X <= -800))
            {
                notifications.Show();
                currentArticle = null;
                currentNote = null;
                picture.Stop();
                providedStamp.Stop();
                NextEvent();
            }
        }

        private void SendArticle(bool isApproved)
        {
            if (isApproved)
            {
                providedStamp = new GraphicObject(new Bitmap(approved.Bitmap, approved.Bitmap.Size - new Size(50, 50)), approved.Position + new Size(25, 25));
                movePaperToSide = new Action(() => { picture.GoRight(); providedStamp.GoRight(); });
                if (currentArticle.Flag != "") stats.SetFlagToTrue(currentArticle.Flag);
                stats.Level.IncreaseLoyality(currentArticle.Loyality);
                stats.Level.IncreaseReprimandScore(currentArticle.ReprimandScore);
                if (currentArticle.Title != "") notifications.Add($"В сегодняшнем выпуске: {currentArticle.Title}");
                else notifications.Add($"В сегодняшнем выпуске: {currentArticle.ExtractBeginning()}...");
            }
            else
            {
                providedStamp = new GraphicObject(new Bitmap(rejected.Bitmap, rejected.Bitmap.Size - new Size(50, 50)), rejected.Position + new Size(25, 25));
                movePaperToSide = new Action(() => { picture.GoLeft(); providedStamp.GoLeft(); });
                stats.Level.IncreaseLoyality(-1);
            }
            sounds.StampPut();
            RemoveStamps();
            waitBeforeOutPaper = 10;
        }

        private void SendNote()
        {
            picture.GoLeft();
            currentNote.HideButtons(controls);
        }

        private void CreateStamps()
        {
            approved.SetInitialPosition();
            rejected.SetInitialPosition();
            stampsAreVisible = true;
        }

        private void RemoveStamps()
        {
            approved.Position = Form1.Beyond;
            rejected.Position = Form1.Beyond;
            stampsAreVisible = false;
        }
    }
}
