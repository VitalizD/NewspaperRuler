using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public partial class Stats : ICloneable
    {
        public static string MonetaryCurrencyName { get; } = "ТОКЕНОВ";
        private readonly GraphicObject noteBackground;
        private readonly DayEnd dayEnd;

        private int degreeGovernmentAnger = 0;
        private int rentDebts = 0;
        private int productsDebts = 0;
        private int heatingDebts = 0;

        private int rent = 120;
        private int productsCost = 40;
        private int heatingCost = 30;

        private int money;
        private int loyalityFactor = 1;

        public DateTime Date { get; private set; } = new DateTime(1987, 9, 26);

        public int Money
        {
            get => money;
            set
            {
                if (value < 0) money = 0;
                else money = value;
            }
        }

        public int LevelNumber { get; private set; } = 1;

        public int Loyality { get; private set; }
        
        public LevelData Level { get; set; }

        public List<Dictionary<string, bool>> EventFlags { get; }

        public Stats(DayEnd dayEnd)
        {
            Money = 100;
            this.dayEnd = dayEnd;
            noteBackground = new GraphicObject(Properties.Resources.NoteBackground1, 750, 1000, 125);
            EventFlags = new List<Dictionary<string, bool>>
            {
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["ArticleAboutChampionWasApproved"] = false,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["ArticleOnProhibitionWeaponsWasApproved"] = false,
                    ["MainCharacterWasOnDate"] = false,
                    ["ArticleOnMassStarvationWasApproved"] = false,
                    ["ArticleOnDryRationsWasApproved"] = false,
                    ["ArticleAboutSalaryDelayWasApproved"] = false,
                    ["SalaryIncreased"] = false

                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["MainCharacterWasOnDate"] = false,
                    ["ArticleOnProhibitionWeaponsWasApproved"] = false,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["TheMainCharacterPaidForSilence"] = true,
                    ["TheMainCharacterPaidLarisa"] = true,
                    ["MainCharacterGaveOutAboutSecretEditorialOffice"] = false,
                    ["MissingPersonNoticeWasPublished"] = false,
                    ["AnnouncementOfDisappearanceOfGalinasHusbandWasApproved"] = false,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["MainCharacterWentToFestival"] = true,
                }
            };
        }

        public void IncreaseLevelNumber() => LevelNumber++;

        public void SetFlagToTrue(string flag)
        {
            if (!EventFlags[LevelNumber - 1].ContainsKey(flag))
                throw new Exception($"The \"{flag}\" flag doesn't exist in the collection {EventFlags[LevelNumber - 1]}");
            EventFlags[LevelNumber - 1][flag] = true;
        }

        public void GoToNextLevel()
        {
            Date = Date.AddDays(1);
            Level = new LevelData(CreateNotes(), ArticleConstructor.ArticlesByLevel[LevelNumber - 1], loyalityFactor);
            CreateIntroduction();
            Level.BuildEventQueue();
        }

        public void FinishLevel()
        {
            Loyality += Level.Loyality;
            Money += Level.Salary - Level.GetTotalFine();

            LevelNumber++;

            CreateWarnings();

            dayEnd.StatsTexts.Add(GetLabel($"Лояльность граждан:\t\t{Loyality}"));
            dayEnd.StatsTexts.Add(GetLabel($"Зарплата:\t\t{Level.Salary}"));
            if (Level.CurrentFine != 0) dayEnd.StatsTexts.Add(GetLabel($"Штраф:\t\t-{Level.GetTotalFine()}"));

            switch (LevelNumber - 1)
            {
                case 1:
                    dayEnd.InformationTexts.Add(GetLabel("Отметьте расходы, которые можете себе позволить."));
                    break;
                case 2:
                    rent += 30;
                    dayEnd.InformationTexts.Add(GetLabel("Квартплата увеличена."));
                    if (!EventFlags[0]["ArticleAboutChampionWasApproved"])
                    {
                        Money += 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Подарок от Галины Руш:\t\t50"));
                    }
                    if (EventFlags[1]["MainCharacterWasOnDate"])
                    {
                        Money -= 30;
                        dayEnd.StatsTexts.Add(GetLabel($"Посещение бара \"Алый цветок\":\t\t-30"));
                    }
                    break;
                case 3:
                    if (EventFlags[0]["ArticleAboutChampionWasApproved"])
                    {
                        Money -= 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Штраф от Министерства социальной защиты:\t\t-50"));
                        dayEnd.InformationTexts.Add(GetLabel("Министерство социальной защиты налагает штраф за нарушение"));
                        dayEnd.InformationTexts.Add(GetLabel("неприкосновенности частной жизни гражданки Галины Руш."));
                    }
                    if (EventFlags[1]["SalaryIncreased"])
                    {
                        Money += 100;
                        dayEnd.StatsTexts.Add(GetLabel($"Бонус к зарплате:\t\t100"));
                        dayEnd.InformationTexts.Add(GetLabel("Ваша сегодняшняя зарплата увеличена за хорошую работу."));
                    }
                    break;
                case 4:
                    productsCost += 10;
                    dayEnd.InformationTexts.Add(GetLabel("Цены на продукты повысились."));
                    dayEnd.Expenses.Add(new Expense($"Плата за молчание:\t\t250 {MonetaryCurrencyName}", 250, ExpenseType.Stranger));
                    if (EventFlags[2]["ArticleOnProhibitionWeaponsWasApproved"])
                    {
                        Money -= 75;
                        dayEnd.StatsTexts.Add(GetLabel($"Вычет из зарплаты:\t\t-75"));
                        dayEnd.Expenses.Add(new Expense($"План Ларисы:\t\t25 {MonetaryCurrencyName}", 25, ExpenseType.Larisa));
                    }
                    break;
                case 5:
                    dayEnd.InformationTexts.Add(GetLabel("Становится холодно. Пора платить за отопление."));
                    dayEnd.InformationTexts.Add(GetLabel("Вы можете пойти на фестиваль света, оплатив стоимость билета."));
                    dayEnd.Expenses.Add(new Expense($"Фестиваль света:\t\t30 {MonetaryCurrencyName}", 30, ExpenseType.Festival));
                    if (EventFlags[3]["TheMainCharacterPaidLarisa"] && EventFlags[3]["TheMainCharacterPaidForSilence"])
                    {
                        Money += 200;
                        dayEnd.StatsTexts.Add(GetLabel($"Возврат похищенных денег:\t\t200"));
                    }
                    if (!EventFlags[3]["TheMainCharacterPaidForSilence"] && !EventFlags[3]["TheMainCharacterPaidLarisa"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Сегодня последний шанс подкупить шантажистку."));
                        dayEnd.Expenses.Add(new Expense($"Плата за молчание:\t\t250 {MonetaryCurrencyName}", 250, ExpenseType.Stranger));
                    }
                    else if (EventFlags[3]["TheMainCharacterPaidForSilence"] && !EventFlags[3]["TheMainCharacterPaidLarisa"])
                        dayEnd.InformationTexts.Add(GetLabel("Шантажистка получила Ваши деньги."));
                    break;
            }
            dayEnd.StatsTexts.Add(GetLabel($"Итого:\t\t{Money} {MonetaryCurrencyName}"));

            dayEnd.Expenses.Add(new Expense($"Квартплата:\t\t{rent} {MonetaryCurrencyName}", rent, ExpenseType.Rent));
            dayEnd.Expenses.Add(new Expense($"Продукты:\t\t{productsCost} {MonetaryCurrencyName}", productsCost, ExpenseType.Products));
            if (LevelNumber > 5)
                dayEnd.Expenses.Add(new Expense($"Отопление:\t\t{heatingCost} {MonetaryCurrencyName}", heatingCost, ExpenseType.Heating));

            dayEnd.RecalculatePositions();

            Label GetLabel(string text)
            {
                return new Label
                {
                    Text = text,
                    ForeColor = Color.White,
                    Font = StringStyle.TitleFont,
                    AutoSize = true,
                };
            }

            void CreateWarnings()
            {
                if (degreeGovernmentAnger == 3 || degreeGovernmentAnger == 4)
                    dayEnd.InformationTexts.Add(GetLabel("Руководство ищет нового кандидата на Ваше место."));
                if (degreeGovernmentAnger == 1 || degreeGovernmentAnger == 2)
                    dayEnd.InformationTexts.Add(GetLabel("Руководство недовольно Вашей работой. Не забывайте об обязательных приказах."));

                if (productsDebts == 1)
                    dayEnd.InformationTexts.Add(GetLabel("Вы голодны."));
                else if (productsDebts == 2)
                    dayEnd.InformationTexts.Add(GetLabel("Вы умираете от голода."));

                if (heatingDebts == 1)
                    dayEnd.InformationTexts.Add(GetLabel("Вы озябли."));
                else if (heatingDebts == 2)
                    dayEnd.InformationTexts.Add(GetLabel("Вы заболели."));

                if (rentDebts == 1 || rentDebts == 2)
                    dayEnd.InformationTexts.Add(GetLabel("У Вас остались неоплаченные долги по счетам."));
                else if (rentDebts == 3 || rentDebts == 4)
                    dayEnd.InformationTexts.Add(GetLabel("Коммунальное хозяйство собирается выселить Вас из квартиры."));
            }
        }

        public void UpdateReprimandScore()
        {
            if (Level.ReprimandScore < 3)
            {
                if (EventFlags[LevelNumber - 1].ContainsKey("MinistryIsSatisfied"))
                {
                    if (degreeGovernmentAnger > 0) degreeGovernmentAnger--;
                    EventFlags[LevelNumber - 1]["MinistryIsSatisfied"] = true;
                }
            }
            else degreeGovernmentAnger += Level.ReprimandScore - 1;
        }

        public void ApplyExpenses()
        {
            foreach (var expence in dayEnd.Expenses)
            {
                if (expence.Marked) continue;
                switch (expence.Type)
                {
                    case ExpenseType.Rent: rentDebts += 3; break;
                    case ExpenseType.Products: productsDebts += 2; break;
                    case ExpenseType.Heating: heatingDebts += 2; break;
                    case ExpenseType.Stranger: EventFlags[3]["TheMainCharacterPaidForSilence"] = false; break;
                    case ExpenseType.Larisa: EventFlags[3]["TheMainCharacterPaidLarisa"] = false; break;
                    case ExpenseType.Festival: EventFlags[4]["MainCharacterWentToFestival"] = false; break;
                }
            }
            if (rentDebts > 0) rentDebts--;
            if (productsDebts > 0) productsDebts--;
            if (heatingDebts > 0) heatingDebts--;
        }

        private string[] Read(string path)
        {
            var result = new List<string>();
            if (!File.Exists(path))
                return new string[0];
            var reader = new StreamReader(path);
            var line = reader.ReadLine();
            while (line != null)
            {
                result.Add(line);
                line = reader.ReadLine();
            }
            return result.ToArray();
        }

        public string[] GetDecrees() => Read($"Decrees\\DecreesLevel{LevelNumber}.txt");

        public string[] GetTrends() => Read($"Trends\\TrendsLevel{LevelNumber}.txt");

        public object Clone() => MemberwiseClone();

        public GameOver CheckLoss(Control.ControlCollection controls)
        {
            if (degreeGovernmentAnger >= 5)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Fired, 450, 350),
                    "Вы уволены. Министерство цензуры и печати нашло Вам замену. " +
                    "Вы вернулись к своей семье, где Вам суждено жить в бедности до конца своих дней...");

            if (productsDebts >= 3)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Dead, 450, 450),
                    "Вы потеряли сознание из-за сильного голода. Последнее, что Вы помните, — перед обмороком Вы мылись в душе...");

            if (rentDebts >= 5)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Expired, 500, 270),
                    "Вас выселили из квартиры. Вам пришлось вернуться в деревню к семье, " +
                    "но Вы не можете ежедневно ходить на работу из-за дальнего расстояния. " +
                    "Вас уволили. Вы обречены жить в бедности до конца своих дней...");

            return null;
        }

        public void SetDifficulty(Difficulties difficulty)
        {
            if (difficulty is Difficulties.Normal) loyalityFactor = 1;
            else loyalityFactor = 2;
        }
    }
}
