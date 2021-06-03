using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
        private readonly InformationPanel trends;
        private readonly Remark remark;

        private bool paperIsEntering;
        private bool levelCompleted;

        private readonly ElementControl decreesBook = new ElementControl("ПРИКАЗЫ", StringStyle.White, Properties.Resources.Book, 120, 100);
        private readonly ElementControl loudspeaker = new ElementControl("ТРЕНДЫ ОБЩ. МНЕНИЯ", StringStyle.White, Properties.Resources.Megaphone, 120, 110);
        private readonly ElementControl menuButton = new ElementControl("МЕНЮ", StringStyle.Black, Properties.Resources.Button, 130, 50);
        private readonly ElementControl date = new ElementControl("", StringStyle.Black, Properties.Resources.Button, 280, 50);

        private readonly Sounds sounds;
        private readonly DayEnd dayEnd;
        private readonly Form1 form;
        private GameOver gameOver;

        public Stats Stats { get; private set; }
        private Stats backup;
        private Stats tempBackup;

        private readonly Action<IUserInterface> changeInterface;
        private readonly Action changeInterfaceToMainMenu;

        public WorkTable(Form1 form, Sounds sounds, DayEnd dayEnd, 
            Action<IUserInterface> changeInterface, Action changeInterfaceToMainMenu)
        {
            this.sounds = sounds;
            this.form = form;
            this.dayEnd = dayEnd;
            this.changeInterface = changeInterface;
            this.changeInterfaceToMainMenu = changeInterfaceToMainMenu;
            dayEnd.CreateEventClickOnContinue(MouseDownOnNextDayButton);

            approved = new Stamp(Properties.Resources.Approved, 300, 250);
            rejected = new Stamp(Properties.Resources.Rejected, 300, 250);
            providedStamp = new GraphicObject(Properties.Resources.Approved, 300, 250, Form1.Beyond);

            notifications = new NotificationPanel(Scale.Resolution, sounds.PlayNotification);
            decrees = new InformationPanel(Properties.Resources.Frame, 500, 800, new Point(-Scale.Get(500), 0), sounds.PlayPanelShow, sounds.PlayPanelHide);
            trends = new InformationPanel(Properties.Resources.Frame, 500, 800, new Point(-Scale.Get(500), 0), sounds.PlayPanelShow, sounds.PlayPanelHide);
            remark = new Remark(Properties.Resources.RemarkBackground, 600, 300, sounds);

            menuButton.ShowImage(new Point(Scale.Resolution.Width - menuButton.Bitmap.Width, 0));
            menuButton.ShowDescription(new Point(menuButton.Position.X + Scale.Get(10), menuButton.Position.Y + Scale.Get(10)));

            date.ShowImage(new Point(0, 0));
            date.ShowDescription(new Point(date.Position.X + Scale.Get(10), date.Position.Y + Scale.Get(10)));
            date.SetTextAreaSize(new Size(300, 50));
        }

        public void StartGame(Difficulties difficulty)
        {
            sounds.PlayBeginLevel();
            sounds.PlayMusic();

            RemoveStamps();

            Stats = new Stats(dayEnd);
            Stats.SetDifficulty(difficulty);

            decreesBook.Hide();
            decrees.Clear();

            loudspeaker.Hide();
            trends.Clear();

            Stats.GoToNextLevel();
            date.Description = Stats.Date.ToString("D");
            NextEvent();
        }

        public void Paint(Graphics graphics)
        {
            graphics.DrawImage(new Bitmap(Properties.Resources.Pen, Scale.Get(50), Scale.Get(500)), Scale.Get(275), Scale.Get(500));
            graphics.DrawImage(new Bitmap(Properties.Resources.Pencil, Scale.Get(100), Scale.Get(400)), Scale.Get(1100), Scale.Get(500));
            graphics.DrawImage(new Bitmap(Properties.Resources.Scissors, Scale.Get(250), Scale.Get(400)), Scale.Get(1200), Scale.Get(500));
            graphics.DrawImage(new Bitmap(Properties.Resources.Eraser, Scale.Get(120), Scale.Get(70)), Scale.Get(1150), Scale.Get(100));
            paper?.Paint(graphics);
            approved?.Paint(graphics);
            rejected?.Paint(graphics);
            providedStamp?.Paint(graphics);
            currentArticle?.Paint(graphics);
            currentNote?.Paint(graphics);
            menuButton?.Paint(graphics);
            date?.Paint(graphics);
            notifications?.Paint(graphics);
            decreesBook?.Paint(graphics);
            loudspeaker?.Paint(graphics);
            decrees?.Paint(graphics);
            trends?.Paint(graphics);
            remark?.Paint(graphics);
        }

        public void EveryTick()
        {
            notifications.EveryTick();
            decrees.EveryTick();
            trends.EveryTick();
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

            if (loudspeaker.IsVisible)
            {
                if (loudspeaker.CursorIsHovered())
                    trends.Show();
                else trends.Hide();
            }

            if (levelCompleted && !remark.Enabled)
                FinishLevel();
        }

        public void MouseDown()
        {
            approved.MouseDown(sounds.PlayStampTake);
            rejected.MouseDown(sounds.PlayStampTake);
            remark.MouseDown();

            if (menuButton.CursorIsHovered())
                GoMainMenu();
        }

        public void MouseUp()
        {
            approved.MouseUp();
            rejected.MouseUp();
            if (approved.OnPaper(paper)) SendArticle(true);
            else if (rejected.OnPaper(paper)) SendArticle(false);
            else if (stampsAreVisible)
            {
                approved.Return(sounds.PlayStampReturn);
                rejected.Return(sounds.PlayStampReturn);
            }
        }

        public void MouseMove()
        {
            approved.MouseMove();
            rejected.MouseMove();
        }

        private void MouseDownOnOption(object sender, MouseEventArgs e)
        {
            sounds.PlayChooseOption();
            var label = sender as Label;
            if (label == currentNote.PositiveOption)
            {
                Stats.SetFlagToTrue(currentNote.Flag);
                notifications.Add(currentNote.PositiveMessage);
            }
            else if (label == currentNote.NegativeOption)
                notifications.Add(currentNote.NegativeMessage);
            SendNote();
        }

        private void MouseDownOnNextDayButton(object sender, MouseEventArgs e)
        {
            Stats.ApplyExpenses();
            dayEnd.Reset();

            gameOver = Stats.CheckLoss(form.Controls);
            if (gameOver != null)
            {
                GoToGameOver();
                return;
            }

            backup = (Stats)tempBackup.Clone();
            sounds.PlayBeginLevel();
            sounds.PlayMusic();
            Stats.GoToNextLevel();
            changeInterface(this);
            UpdateElements();
            NextEvent();
        }

        private void NextEvent()
        {
            if (Stats.Level.Events.Count == 0)
            {
                levelCompleted = true;
                return;
            }

            var _event = Stats.Level.Events.Dequeue();

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
            sounds.PlayPaper();
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
                    sounds.PlayStampEnter();
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

            sounds.PlayStampPut();
            RemoveStamps();
            outPaper.WaitAndExecute(10);
        }

        private void ApproveArticle()
        {
            providedStamp = new GraphicObject(approved.Bitmap, approved.Bitmap.Size - new Size(50, 50), approved.Position + new Size(25, 25), false);
            outPaper = new Waiting(new Action(() => { paper.GoRight(); providedStamp.GoRight(); }));
            if (currentArticle.Flag != "") Stats.SetFlagToTrue(currentArticle.Flag);
            Stats.Level.IncreaseLoyality(currentArticle.Loyality);
            Stats.Level.IncreaseReprimandScore(currentArticle.ReprimandScore);
            if (currentArticle.Title != "") notifications.Add($"В сегодняшнем выпуске: {currentArticle.Title}");
            else notifications.Add($"В сегодняшнем выпуске: {currentArticle.ExtractBeginning()}...");
            CheckForMistake();
        }

        private void RejectArticle()
        {
            providedStamp = new GraphicObject(rejected.Bitmap, rejected.Bitmap.Size - new Size(50, 50), rejected.Position + new Size(25, 25), false);
            outPaper = new Waiting(new Action(() => { paper.GoLeft(); providedStamp.GoLeft(); }));
            if (currentArticle.Loyality != 0) Stats.Level.IncreaseLoyality(-1);
        }

        private void SendNote()
        {
            paper.GoLeft();
            currentNote.HideButtons(form.Controls);
        }

        private void CreateStamps()
        {
            approved.SetPosition(new Point(paper.Position.X - approved.Bitmap.Width, Scale.Get(250)));
            rejected.SetPosition(new Point(paper.Position.X + paper.Bitmap.Width, Scale.Get(250)));
            stampsAreVisible = true;
        }

        private void RemoveStamps()
        {
            approved.Position = Form1.Beyond;
            rejected.Position = Form1.Beyond;
            stampsAreVisible = false;
        }

        private void UpdateElements()
        {
            switch (Stats.LevelNumber)
            {
                case 2:
                    decreesBook.ShowImage(new Point(0, Scale.Resolution.Height - decreesBook.Bitmap.Height));
                    decreesBook.ShowDescription(new Point(decreesBook.Position.X + decreesBook.Bitmap.Width + Scale.Get(10), decreesBook.Position.Y + decreesBook.Bitmap.Height / 2));
                    break;
                case 5:
                    loudspeaker.ShowImage(new Point(0, Scale.Resolution.Height - 2 * loudspeaker.Bitmap.Height - Scale.Get(10)));
                    loudspeaker.ShowDescription(new Point(loudspeaker.Position.X + loudspeaker.Bitmap.Width + Scale.Get(10), loudspeaker.Position.Y + loudspeaker.Bitmap.Height / 3));
                    break;
            }

            var newDecrees = Stats.GetDecrees();
            decrees.Clear();
            decrees.Add(newDecrees);

            var newTrends = Stats.GetTrends();
            trends.Clear();
            trends.Add(newTrends);

            date.Description = Stats.Date.ToString("D");
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
                case Mistake.Deprecated: remarkText.Append("Устаревшая информация. Сверяйте дату в статье с текущей датой."); break;
                case Mistake.Future: remarkText.Append("Информация \"из будущего\". Сверяйте дату в статье с текущей датой."); break;
                case Mistake.MissingPerson: remarkText.Append("Объявления о пропажах людей подлежат отказу."); break;
                case Mistake.Advertisement: remarkText.Append("Рекламные объявления запрещены."); break;
                case Mistake.Personality: remarkText.Append("Статьи с упоминаниями конкретных личностей запрещены."); break;
                case Mistake.ObsceneLanguage: remarkText.Append("Кодекс чести государственной газеты предписывает редактору отклонять все статьи с ненормативной лексикой."); break;
                case Mistake.Opposition: remarkText.Append("Публикация статей оппозиционного характера запрещена."); break;
                case Mistake.Virus: remarkText.Append("Упоминания о вирусе КРАБ запрещены."); break;
                default: return;
            }
            if (currentArticle.ReprimandScore == 0)
            {
                Stats.Level.IncreaseFine();
                if (Stats.Level.CurrentFine == 0)
                    remarkText.Append("\n\nПри повторных ошибках будет наложен штраф.");
                else remarkText.Append($"\n\nШтраф: {Stats.Level.CurrentFine} ТОКЕНОВ");
            }
            else remarkText.Append("\n\nЗамечание без штрафа.");
            remark.Add(remarkText.ToString());
        }

        private void FinishLevel()
        {
            levelCompleted = false;
            notifications.Clear();
            remark.Clear();
            Stats.UpdateReprimandScore();
            GoToDayEndOrLoss();
        }

        private void GoToDayEndOrLoss()
        {
            gameOver = Stats.CheckLoss(form.Controls);
            if (gameOver is null)
            {
                tempBackup = (Stats)Stats.Clone();
                Stats.FinishLevel();
                dayEnd.ShowAll();
                changeInterface(dayEnd);
            }
            else GoToGameOver();
        }

        private void GoToGameOver()
        {
            changeInterface(gameOver);
            sounds.StopMusic();
            sounds.PlayGameOver();
            if (backup is null) gameOver.CreateOnlyMainMenuButton(MouseDownOnMainMenuButton);
            else gameOver.CreateMainMenuButtonAndReturnButton(MouseDownOnMainMenuButton, MouseDownOnReturnButton);
        }


        private void MouseDownOnReturnButton(object sender, MouseEventArgs e)
        {
            Stats = backup;
            sounds.PlayBeginLevel();
            GoToDayEndOrLoss();
        }

        private void MouseDownOnMainMenuButton(object sender, MouseEventArgs e) => GoMainMenu();

        private void GoMainMenu()
        {
            sounds.StopMusic();
            sounds.PlayMenuButton();
            changeInterfaceToMainMenu();
        }

        public void SetFormBackground(Form1 form) => form.BackgroundImage = Properties.Resources.Background;
    }
}
