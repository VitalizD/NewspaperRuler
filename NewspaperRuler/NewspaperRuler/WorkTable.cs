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
        private bool showIncreasedLoyality;

        private readonly ElementControl decreesBook = new ElementControl("ПРИКАЗЫ", StringStyle.White, Properties.Resources.Book, 120, 100);
        private readonly ElementControl loudspeaker = new ElementControl("ТРЕНДЫ ОБЩ. МНЕНИЯ", StringStyle.White, Properties.Resources.Megaphone, 120, 110);
        private readonly ElementControl menuButton = new ElementControl("ГЛАВНОЕ МЕНЮ", StringStyle.Black, Properties.Resources.Button, 250, 50);
        private readonly ElementControl date = new ElementControl("", StringStyle.Black, Properties.Resources.Button, 280, 50);
        private readonly ElementControl articlesCount = new ElementControl("", StringStyle.White, Properties.Resources.PaperIcon, 37, 57);

        private readonly Sounds sounds;
        private readonly DayEnd dayEnd;
        private readonly Form1 form;
        private GameOver gameOver;
        private readonly End end;

        public Stats Stats { get; private set; }
        //private Stats backup;
        //private Stats tempBackup;

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
            end = new End(changeInterfaceToMainMenu, sounds);
            dayEnd.CreateEventClickOnContinue(MouseDownOnNextDayButton);

            approved = new Stamp(Properties.Resources.Approved, 300, 250);
            rejected = new Stamp(Properties.Resources.Rejected, 300, 250);
            providedStamp = new GraphicObject(Properties.Resources.Approved, 300, 250, Form1.Beyond);

            notifications = new NotificationPanel(Scale.Resolution, sounds.PlayNotification);
            decrees = new InformationPanel(Properties.Resources.BookPart, 480, 750, new Point(-Scale.Get(500), Scale.Get(30)), sounds.PlayPanelShow, sounds.PlayPanelHide);
            trends = new InformationPanel(Properties.Resources.BookPart, 480, 750, new Point(-Scale.Get(500), Scale.Get(30)), sounds.PlayPanelShow, sounds.PlayPanelHide);
            remark = new Remark(Properties.Resources.RemarkBackground, 600, 300, sounds);

            menuButton.ShowImage(new Point(Scale.Resolution.Width - menuButton.Bitmap.Width, 0));
            menuButton.ShowDescription(new Point(menuButton.Position.X + Scale.Get(10), menuButton.Position.Y + Scale.Get(10)));
            menuButton.SetTextAreaSize(new Size(300, 50));

            date.ShowImage(new Point(0, 0));
            date.ShowDescription(new Point(date.Position.X + Scale.Get(10), date.Position.Y + Scale.Get(10)));
            date.SetTextAreaSize(new Size(280, 50));
            date.SetStringFormat(new StringFormat { Alignment = StringAlignment.Center });

            articlesCount.ShowImage(new Point(Scale.Resolution.Width - articlesCount.Bitmap.Width - Scale.Get(35), menuButton.Position.Y + menuButton.Bitmap.Height + Scale.Get(20)));
            articlesCount.ShowDescription(new Point(articlesCount.Position.X - Scale.Get(60), articlesCount.Position.Y + Scale.Get(15)));
            articlesCount.SetTextAreaSize(new Size(50, 0));
            articlesCount.SetStringFormat(new StringFormat { Alignment = StringAlignment.Far });
        }

        public void StartGame(Difficulties difficulty)
        {
            sounds.PlayBeginLevel();
            sounds.PlayMusic();

            RemoveStamps();

            currentArticle = null;
            currentNote = null;

            Stats = new Stats(dayEnd);
            Stats.SetDifficulty(difficulty);

            decreesBook.Hide();
            decrees.Clear();

            loudspeaker.Hide();
            trends.Clear();

            Stats.GoToNextLevel();
            UpdateElementsOnLevel();
            date.Description = Stats.Date.ToString("D");
            NextEvent();
        }

        public void Paint(Graphics graphics)
        {
            graphics.DrawImage(new Bitmap(Properties.Resources.Pen, Scale.Get(50), Scale.Get(500)), Scale.Get(275), Scale.Get(500));
            graphics.DrawImage(new Bitmap(Properties.Resources.Pencil, Scale.Get(100), Scale.Get(400)), Scale.Get(1100), Scale.Get(500));
            graphics.DrawImage(new Bitmap(Properties.Resources.Scissors, Scale.Get(250), Scale.Get(400)), Scale.Get(1200), Scale.Get(500));
            //graphics.DrawImage(new Bitmap(Properties.Resources.Eraser, Scale.Get(120), Scale.Get(70)), Scale.Get(1150), Scale.Get(100));

            paper?.Paint(graphics);
            approved?.Paint(graphics);
            rejected?.Paint(graphics);

            if (showIncreasedLoyality && currentArticle != null)
                graphics.DrawString($"ЛОЯЛЬНОСТЬ  x{currentArticle.Loyality}", StringStyle.TextFont, new SolidBrush(Color.Green),
                    new Rectangle(approved.Position.X + Scale.Get(15), approved.Position.Y - Scale.Get(45), Scale.Get(250), 0),
                    new StringFormat { Alignment = StringAlignment.Center });

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
            articlesCount?.Paint(graphics);
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
                GoToMainMenu();
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

            if (Stats.LevelNumber > 10)
            {
                end.CreateQueue(Stats.GetGameResults());
                changeInterface(end);
                return;
            }

            //backup = (Stats)tempBackup.Clone();
            PrepareLevel();
        }

        public void LoadLevel()
        {
            Stats = new Stats(dayEnd);
            Stats.LoadFromJson();
            //backup = (Stats)Stats.Clone();
            currentArticle = null;
            currentNote = null;
            remark.Hide();
            RemoveStamps();
            PrepareLevel();
        }

        private void PrepareLevel()
        {
            sounds.PlayBeginLevel();
            sounds.PlayMusic();
            Stats.GoToNextLevel();
            notifications.Hide();
            changeInterface(this);
            UpdateElementsOnLevel();
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
            articlesCount.Description = (Stats.Level.Events.Count + 1).ToString();

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
            if (currentArticle.Loyality > 1 && Stats.LevelNumber > 1)
                showIncreasedLoyality = true;
        }

        private void RemoveStamps()
        {
            approved.Position = Form1.Beyond;
            rejected.Position = Form1.Beyond;
            stampsAreVisible = false;
            showIncreasedLoyality = false;
        }

        private void UpdateElementsOnLevel()
        {
            if (Stats.DecreesAreVisible && !decreesBook.IsVisible)
            {
                decreesBook.ShowImage(new Point(0, Scale.Resolution.Height - decreesBook.Bitmap.Height));
                decreesBook.ShowDescription(new Point(decreesBook.Position.X + decreesBook.Bitmap.Width + Scale.Get(10), decreesBook.Position.Y + decreesBook.Bitmap.Height / 2));
            }

            if (Stats.TrendsAreVisible && !loudspeaker.IsVisible)
            {
                loudspeaker.ShowImage(new Point(0, Scale.Resolution.Height - 2 * loudspeaker.Bitmap.Height - Scale.Get(10)));
                loudspeaker.ShowDescription(new Point(loudspeaker.Position.X + loudspeaker.Bitmap.Width + Scale.Get(10), loudspeaker.Position.Y + loudspeaker.Bitmap.Height / 3));
            }

            switch (Stats.LevelNumber)
            {
                case 9:
                    sounds.StopMusic();
                    sounds.PlayFinalMusic1();
                    break;
                case 10:
                    sounds.StopMusic();
                    sounds.StopFinalMusic1();
                    sounds.PlayFinalMusic2();
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
                case Mistake.MassEvent: remarkText.Append("Массовые мероприятия запрещены в связи с ограничениями, созданными с целью уменьшения скорости распространения вируса КРАБ."); break;
                case Mistake.IncorrectGenre: remarkText.Append("Целевая направленность не соответствует содержанию статьи."); break;
                case Mistake.NoGenre: remarkText.Append("Статья без целевой направленности"); break;
                case Mistake.Protests: remarkText.Append("Упоминания о массовых протестах запрещены."); break;
                case Mistake.NewWar: remarkText.Append("Упоминания о начавшейся войне запрещены."); break;
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
                //tempBackup = (Stats)Stats.Clone();
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
            if (AuxiliaryMethods.SaveExists())
                gameOver.CreateMainMenuButtonAndReturnButton(MouseDownOnMainMenuButton, MouseDownOnReturnButton);
            else gameOver.CreateOnlyMainMenuButton(MouseDownOnMainMenuButton);
        }


        private void MouseDownOnReturnButton(object sender, MouseEventArgs e)
        {
            //Stats = backup;
            //sounds.PlayBeginLevel();
            LoadLevel();
        }

        private void MouseDownOnMainMenuButton(object sender, MouseEventArgs e) => GoToMainMenu();

        private void GoToMainMenu()
        {
            sounds.StopAll();
            sounds.PlayMenuButton();
            changeInterfaceToMainMenu();
        }

        public void ExecuteAfterTransition(Form1 form) => form.BackgroundImage = Properties.Resources.Background;
    }
}
