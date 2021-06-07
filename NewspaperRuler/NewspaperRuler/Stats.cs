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

        public List<Dictionary<string, bool>> Flags { get; }

        public Stats(DayEnd dayEnd)
        {
            this.dayEnd = dayEnd;
            noteBackground = new GraphicObject(Properties.Resources.NoteBackground, 750, 1000, 125);
            Flags = new List<Dictionary<string, bool>>
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
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["WifeIsFaithful"] = false,
                    ["MainCharacterHelpedGrasshoppers"] = false,
                    ["MainCharacterBoughtPresentForHisSon"] = true,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["MainCharacterHelpedGrasshoppersFirstTime"] = false,
                    ["MainCharacterHelpedGrasshoppersSecondTime"] = false,
                    ["GalinaWillHelpMainCharacterFreeCharge"] = false,
                    ["MainCharacterPaidGalina"] = true,
                    ["MedicineWasDeliveredToWife"] = true,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["MainCharacterHelpedGrasshoppersFirstTime"] = false,
                    ["MainCharacterHelpedGrasshoppersSecondTime"] = false,
                    ["MainCharacterHelpedGrasshoppersThirdTime"] = false,
                    ["SonStayedAtHome"] = false,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["GrasshoppersEliminated"] = false,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["WifesOperationPaid"] = true,
                    ["MainCharacterBoughtFakePassports"] = true,
                    ["WifeAlive"] = false,
                    ["SonAlive"] = false,
                    ["MainCharacterIsFree"] = false,
                }
            };
        }

        public void IncreaseLevelNumber() => LevelNumber++;

        public (string, Bitmap)[] GetGameResults()
        {
            var result = new List<(string, Bitmap)>();
            if ((Flags[6]["MedicineWasDeliveredToWife"] || Flags[9]["WifesOperationPaid"]) && Flags[7]["SonStayedAtHome"])
            {
                result.Add(("Жена выздоровела. Сын избежал призыва на войну. Все живы.", new Bitmap(Properties.Resources.WifeAliveSonAlive, Scale.Get(500), Scale.Get(500))));
                Flags[9]["WifeAlive"] = true;
                Flags[9]["SonAlive"] = true;
            }
            else if ((Flags[6]["MedicineWasDeliveredToWife"] || Flags[9]["WifesOperationPaid"]) && !Flags[7]["SonStayedAtHome"])
            {
                result.Add(("Жена выздоровела. Сына забрали на войну. Он не вернулся...", new Bitmap(Properties.Resources.WifeAliveSonDead, Scale.Get(500), Scale.Get(500))));
                if (!Flags[5]["WifeIsFaithful"])
                    result.Add(("Однако жена не могла смириться с Вашим предательством и смертью сына. Она свела свою жизнь с концами...", new Bitmap(Properties.Resources.Gallows, Scale.Get(500), Scale.Get(500))));
                else Flags[9]["WifeAlive"] = true;
            }
            else if (!(Flags[6]["MedicineWasDeliveredToWife"] || Flags[9]["WifesOperationPaid"]) && Flags[7]["SonStayedAtHome"])
            {
                Flags[9]["SonAlive"] = true;
                result.Add(("Жена умерла. Сын избежал призыва на войну.", new Bitmap(Properties.Resources.WifeDeadSonAlive, Scale.Get(500), Scale.Get(500))));
            }
            else result.Add(("Жена умерла. Сына забрали на войну. Он не вернулся...", new Bitmap(Properties.Resources.WifeDeadSonDead, Scale.Get(500), Scale.Get(500))));
            if (!Flags[8]["GrasshoppersEliminated"])
                result.Add(("\"Кузнечики\" осуществили попытку захватить гос. власть, однако государство оказалось сильнее...", new Bitmap(Properties.Resources.Grasshopper, Scale.Get(500), Scale.Get(500))));
            if (Flags[9]["MainCharacterBoughtFakePassports"])
            {
                Flags[9]["MainCharacterIsFree"] = true;
                var bitmap = new Bitmap(Properties.Resources.Ubringston, Scale.Get(500), Scale.Get(400));
                if (Flags[9]["SonAlive"] && Flags[9]["WifeAlive"])
                    result.Add(("Вы с семьёй успешно сбежали в Убрингстон, где начали жизнь с чистого листа.", bitmap));
                else if (Flags[9]["SonAlive"])
                    result.Add(("Вы с сыном успешно сбежали в Убрингстон, где начали жизнь с чистого листа.", bitmap));
                else if (Flags[9]["WifeAlive"])
                    result.Add(("Вы с женой успешно сбежали в Убрингстон, где начали жизнь с чистого листа.", bitmap));
                else result.Add(("Вы в одиночку успешно сбежали в Убрингстон, где начали жизнь с чистого листа.", bitmap));
                if (Flags[6]["GalinaWillHelpMainCharacterFreeCharge"] || Flags[6]["MainCharacterPaidGalina"])
                    result.Add(("В Убрингстоне Вы вновь встретились с Галиной Руш. Она стала Вашим частым гостем.", new Bitmap(Properties.Resources.Galina, Scale.Get(450), Scale.Get(500))));
            }
            else
            {
                result.Add(("Вы прибыли на слушание по делу причастия гос. служащих к \"Кузнечикам\".", new Bitmap(Properties.Resources.Hummer, Scale.Get(500), Scale.Get(500))));
                if (!Flags[8]["GrasshoppersEliminated"])
                {
                    result.Add(("Против Вас найдены улики в Вашем причастии к \"Кузнечикам\". За измену государству Вам положено наказание в виде лишения свободы на 3 года.", new Bitmap(Properties.Resources.Prison, Scale.Get(600), Scale.Get(450))));
                    if (!Flags[3]["MainCharacterGaveOutAboutSecretEditorialOffice"])
                    {
                        result.Add(("Разъярённая толпа граждан из тайной редакции, существование которой Вы оставили в секрете, силой добилась Вашего освобождения.", new Bitmap(Properties.Resources.Crowd, Scale.Get(600), Scale.Get(400))));
                        Flags[9]["MainCharacterIsFree"] = true;
                    }
                }
                else
                {
                    Flags[9]["MainCharacterIsFree"] = true;
                    result.Add(("Вы абсолютно чисты, и с Вас сняты все подозрения. Государство благодарит Вас за преданность!", new Bitmap(Properties.Resources.Hummer, Scale.Get(500), Scale.Get(500))));
                }
            }
            if (Flags[2]["ArticleOnProhibitionWeaponsWasApproved"])
                result.Add(("Международная ассоциация мировой безопасности \"Апостол\" изгнала захватчиков из страны, " +
                    "передав часть Андиплантийской территории государству. Да прибудет справедливость!", new Bitmap(Properties.Resources.Town, Scale.Get(500), Scale.Get(350))));
            else
                result.Add(("Международная ассоциация мировой безопасности \"Апостол\" отказалась изгонять захватчиков из страны. " +
                    "Государство пало. Теперь эти территории присоединены к Андиплантийской коалиции.", new Bitmap(Properties.Resources.DeadTown, Scale.Get(500), Scale.Get(350))));
            if (Flags[9]["MainCharacterIsFree"])
            {
                if (Flags[9]["SonAlive"])
                {
                    if (Flags[5]["MainCharacterBoughtPresentForHisSon"])
                        result.Add(("Вы нашли с сыном общий язык. Теперь Вы проводите больше времени вместе.", null));
                    else result.Add(("Вы пытаетесь найти с сыном общий язык, но он сторонится Вас. Что не так?", null));
                }
                if (Flags[9]["WifeAlive"])
                {
                    if (Flags[5]["WifeIsFaithful"])
                        result.Add(("У Вас с женой тёплые отношения. Вы живёте в верности и согласии.", new Bitmap(Properties.Resources.Hands, Scale.Get(300), Scale.Get(500))));
                    else result.Add(("Жена отвергает Вас. Она проводит большую часть своего времени с сыном.", new Bitmap(Properties.Resources.BrokenHeart, Scale.Get(500), Scale.Get(500))));
                }
            }
            if (Flags[9]["SonAlive"])
                result.Add(("Сын поступил в высшую академию. Там он нашёл свою вторую половинку.", new Bitmap(Properties.Resources.Confederate, Scale.Get(500), Scale.Get(400))));
            if (Flags[1]["ArticleOnProhibitionWeaponsWasApproved"])
                result.Add(("Всё это время МАМБА за Вами наблюдала. Вам предложили там вакансию. " +
                    "Вы успешно прошли испытательный срок и теперь являетесь частью команды одной из самых авторитетных организаций в мире.", new Bitmap(Properties.Resources.Team, Scale.Get(500), Scale.Get(400))));
            else if (!Flags[9]["MainCharacterBoughtFakePassports"])
            {
                if (Flags[4]["MainCharacterWentToFestival"])
                    result.Add(("Вы устроились на работу технического специалиста в сфере коммунального хозяйства. " +
                        "Сделать это помог Ваш новый приятель, с которым Вы познакомились на фестивале света.", new Bitmap(Properties.Resources.CommunalService, Scale.Get(500), Scale.Get(500))));
                else result.Add(("Вы не смогли найти новую работу. Вам суждено жить в бедности до конца жизни.", new Bitmap(Properties.Resources.Trash, Scale.Get(500), Scale.Get(500))));
            }
            result.Add(("Ничего в этой жизни не даётся легко. Чтобы достичь своих целей, необходимо идти на определенные жертвы — " +
                "тратить свои силы, время, ограничивать себя в чём-либо.", new Bitmap(Properties.Resources.Landscape, Scale.Get(500), Scale.Get(400))));
            result.Add(("Иногда бывают моменты, когда хочется всё бросить и отказаться от мечты.", new Bitmap(Properties.Resources.Fox, Scale.Get(500), Scale.Get(300))));
            result.Add(("В такие моменты вспомни, как много ты получишь, если пойдёшь дальше, и как много потеряешь, если сдашься. " +
                "Цена успеха, как правило, меньше, чем цена неудачи.", new Bitmap(Properties.Resources.Bird, Scale.Get(400), Scale.Get(500))));
            result.Add(("Единственный способ жить — это жить. Говорить себе: \"Я могу это сделать\", — даже зная, что не можешь.", new Bitmap(Properties.Resources.Flower, Scale.Get(300), Scale.Get(500))));
            result.Add(("Спасибо за игру", null));
            return result.ToArray();
        }

        public void SetFlagToTrue(string flag)
        {
            if (!Flags[LevelNumber - 1].ContainsKey(flag))
                throw new Exception($"The \"{flag}\" flag doesn't exist in the collection {Flags[LevelNumber - 1]}");
            Flags[LevelNumber - 1][flag] = true;
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
                    if (!Flags[0]["ArticleAboutChampionWasApproved"])
                    {
                        Money += 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Подарок от Галины Руш:\t\t50"));
                    }
                    if (Flags[1]["MainCharacterWasOnDate"])
                    {
                        Money -= 30;
                        dayEnd.StatsTexts.Add(GetLabel($"Посещение бара \"Алый цветок\":\t\t-30"));
                    }
                    break;
                case 3:
                    if (Flags[0]["ArticleAboutChampionWasApproved"])
                    {
                        Money -= 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Штраф от Министерства социальной защиты:\t\t-50"));
                        dayEnd.InformationTexts.Add(GetLabel("Министерство социальной защиты налагает штраф за нарушение"));
                        dayEnd.InformationTexts.Add(GetLabel("неприкосновенности частной жизни гражданки Галины Руш."));
                    }
                    if (Flags[1]["SalaryIncreased"])
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
                    if (Flags[2]["ArticleOnProhibitionWeaponsWasApproved"])
                    {
                        Money -= 75;
                        dayEnd.StatsTexts.Add(GetLabel($"Вычет из зарплаты:\t\t-75"));
                        dayEnd.Expenses.Add(new Expense($"План Ларисы:\t\t25 {MonetaryCurrencyName}", 25, ExpenseType.Larisa));
                    }
                    break;
                case 5:
                    dayEnd.InformationTexts.Add(GetLabel("Становится холодно. Пора платить за отопление."));
                    dayEnd.InformationTexts.Add(GetLabel("Вы можете пойти на фестиваль света, оплатив стоимость билета."));
                    dayEnd.InformationTexts.Add(GetLabel("У Вашего сына завтра День Рождения."));
                    dayEnd.Expenses.Add(new Expense($"Фестиваль света:\t\t30 {MonetaryCurrencyName}", 30, ExpenseType.Festival));
                    if (Flags[3]["TheMainCharacterPaidLarisa"] && Flags[3]["TheMainCharacterPaidForSilence"])
                    {
                        Money += 200;
                        dayEnd.StatsTexts.Add(GetLabel($"Возврат похищенных денег:\t\t200"));
                    }
                    if (!Flags[3]["TheMainCharacterPaidForSilence"] && !Flags[3]["TheMainCharacterPaidLarisa"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Сегодня последний шанс подкупить шантажистку."));
                        dayEnd.Expenses.Add(new Expense($"Плата за молчание:\t\t250 {MonetaryCurrencyName}", 250, ExpenseType.Stranger));
                    }
                    else if (Flags[3]["TheMainCharacterPaidForSilence"] && !Flags[3]["TheMainCharacterPaidLarisa"])
                        dayEnd.InformationTexts.Add(GetLabel("Шантажистка получила Ваши деньги."));
                    break;
                case 6:
                    dayEnd.InformationTexts.Add(GetLabel("Ваша жена заразилась вирусом КРАБ."));
                    dayEnd.InformationTexts.Add(GetLabel("Сегодня День Рождения Вашего сына. Вы можете отправить ему подарок."));
                    dayEnd.Expenses.Add(new Expense($"Подарок сыну:\t\t35 {MonetaryCurrencyName}", 35, ExpenseType.Son));
                    if (Flags[3]["MainCharacterGaveOutAboutSecretEditorialOffice"])
                    {
                        Money += 120;
                        dayEnd.StatsTexts.Add(GetLabel($"Премия:\t\t120"));
                        dayEnd.InformationTexts.Add(GetLabel("Вы получили премию за раскрытие незаконной тайной редакции."));
                    }
                    if (Flags[3]["TheMainCharacterPaidForSilence"] && !Flags[3]["TheMainCharacterPaidLarisa"])
                    {
                        Money += 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Шантажистка:\t\t50"));
                        dayEnd.InformationTexts.Add(GetLabel("Шантажистка решила вернуть часть похищенных денег."));
                    }
                    if (Flags[5]["MainCharacterHelpedGrasshoppers"])
                    {
                        Money += 100;
                        dayEnd.StatsTexts.Add(GetLabel($"Привет от \"Кузнечиков\":\t\t100"));
                    }
                    break;
                case 7:
                    rent += 20;
                    dayEnd.InformationTexts.Add(GetLabel("Квартплата увеличена."));
                    dayEnd.InformationTexts.Add(GetLabel("Курс лечения Вашей жены начался. Регулярно поставляйте ей лекарство. Осталось 3 дня."));
                    if ((!Flags[0]["ArticleAboutChampionWasApproved"] || Flags[3]["AnnouncementOfDisappearanceOfGalinasHusbandWasApproved"])
                        && !Flags[6]["GalinaWillHelpMainCharacterFreeCharge"])
                        dayEnd.Expenses.Add(new Expense($"Помощь Галины Руш:\t\t110 {MonetaryCurrencyName}", 110, ExpenseType.Galina));
                    if (Flags[3]["MissingPersonNoticeWasPublished"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Женщина из плиувильской аптеки украла препарат для Вашей жены и поплатилась за это:"));
                        dayEnd.InformationTexts.Add(GetLabel("её уволили. Она больше не сможет Вам помочь. Лекарство успешно доставлено Вашей жене."));
                    }
                    else dayEnd.Expenses.Add(new Expense($"Лекарство для жены:\t\t200 {MonetaryCurrencyName}", 200, ExpenseType.Medicine));
                    if (Flags[5]["MainCharacterBoughtPresentForHisSon"])
                        dayEnd.InformationTexts.Add(GetLabel("Вашему сыну понравился подарок."));
                    else dayEnd.InformationTexts.Add(GetLabel("Ваш сын расстроился, что Вы не проявили к нему внимание в его День Рождения."));
                    if (Flags[6]["MainCharacterHelpedGrasshoppersFirstTime"] && Flags[6]["MainCharacterHelpedGrasshoppersSecondTime"])
                    {
                        Money += 150;
                        dayEnd.StatsTexts.Add(GetLabel($"Привет от \"Кузнечиков\":\t\t150"));
                    }
                    break;
                case 8:
                    if (Flags[6]["MedicineWasDeliveredToWife"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("До конца курса лечения Вашей жены осталось 2 дня."));
                        dayEnd.Expenses.Add(new Expense($"Лекарство для жены:\t\t200 {MonetaryCurrencyName}", 200, ExpenseType.Medicine));
                    }
                    if (Flags[7]["MainCharacterHelpedGrasshoppersFirstTime"]
                        && Flags[7]["MainCharacterHelpedGrasshoppersSecondTime"]
                        && Flags[7]["MainCharacterHelpedGrasshoppersThirdTime"])
                    {
                        Money += 250;
                        dayEnd.StatsTexts.Add(GetLabel($"Привет от \"Кузнечиков\":\t\t250"));
                    }
                    else if (!Flags[7]["MainCharacterHelpedGrasshoppersFirstTime"]
                        && !Flags[7]["MainCharacterHelpedGrasshoppersSecondTime"]
                        && !Flags[7]["MainCharacterHelpedGrasshoppersThirdTime"])
                    {
                        Money += 300;
                        dayEnd.InformationTexts.Add(GetLabel("Министерство цензуры и печати благодарит Вас за противодействие \"Кузнечикам\"."));
                        dayEnd.StatsTexts.Add(GetLabel($"Бонус к зарплате:\t\t300"));
                    }
                    break;
                case 9:
                    if (Flags[6]["MedicineWasDeliveredToWife"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Сегодня последний день курса лечения Вашей жены."));
                        dayEnd.Expenses.Add(new Expense($"Лекарство для жены:\t\t200 {MonetaryCurrencyName}", 200, ExpenseType.Medicine));
                    }
                    if (!Flags[6]["GalinaWillHelpMainCharacterFreeCharge"] && !Flags[6]["MainCharacterPaidGalina"])
                        dayEnd.InformationTexts.Add(GetLabel("Вашего сына забрали на войну."));
                    break;
                case 10:
                    dayEnd.InformationTexts.Add(GetLabel("Вы в шаге от конца игры."));
                    if (Flags[6]["MedicineWasDeliveredToWife"])
                        dayEnd.InformationTexts.Add(GetLabel("Ваша жена пошла на поправку."));
                    else
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Вашу жену положили в больницу. Она умирает."));
                        dayEnd.InformationTexts.Add(GetLabel("Требуется срочная хирургическая операция – последняя надежда на её спасение."));
                        dayEnd.Expenses.Add(new Expense($"Операция:\t\t400 {MonetaryCurrencyName}", 400, ExpenseType.Operation));
                    }
                    if (Flags[6]["MainCharacterHelpedGrasshoppersFirstTime"]
                                && Flags[6]["MainCharacterHelpedGrasshoppersSecondTime"]
                                && Flags[7]["MainCharacterHelpedGrasshoppersFirstTime"]
                                && Flags[7]["MainCharacterHelpedGrasshoppersSecondTime"]
                                && Flags[7]["MainCharacterHelpedGrasshoppersThirdTime"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("\"Кузнечики\" готовы достать поддельные паспорта за \"относительно низкую цену\"."));
                        dayEnd.Expenses.Add(new Expense($"Поддельные паспорта:\t\t500 {MonetaryCurrencyName}", 500, ExpenseType.Passports));
                    }
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

                if (Loyality > -10 && Loyality <= -5)
                    dayEnd.InformationTexts.Add(GetLabel("Граждане протестуют. Они недовольны, что от них скрывают правду."));
            }
        }

        public void UpdateReprimandScore()
        {
            Loyality += Level.Loyality;
            if (Level.ReprimandScore < 3)
            {
                if (Flags[LevelNumber - 1].ContainsKey("MinistryIsSatisfied"))
                {
                    if (degreeGovernmentAnger > 0) degreeGovernmentAnger--;
                    Flags[LevelNumber - 1]["MinistryIsSatisfied"] = true;
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
                    case ExpenseType.Stranger: Flags[3]["TheMainCharacterPaidForSilence"] = false; break;
                    case ExpenseType.Larisa: Flags[3]["TheMainCharacterPaidLarisa"] = false; break;
                    case ExpenseType.Festival: Flags[4]["MainCharacterWentToFestival"] = false; break;
                    case ExpenseType.Son: Flags[5]["MainCharacterBoughtPresentForHisSon"] = false; break;
                    case ExpenseType.Galina: Flags[6]["MainCharacterPaidGalina"] = false; break;
                    case ExpenseType.Medicine: Flags[6]["MedicineWasDeliveredToWife"] = false; break;
                    case ExpenseType.Operation: Flags[9]["WifesOperationPaid"] = false; break;
                    case ExpenseType.Passports: Flags[9]["MainCharacterBoughtFakePassports"] = false; break;
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

            if (Loyality <= -10)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Fired, 450, 350),
                    "Вы не справились со своими обязанностями. Вас уволили. " +
                    "Вы вернулись к своей семье, где Вам суждено жить в бедности до конца своих дней...");

            if (productsDebts >= 3)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Dead, 450, 450),
                    "Вы потеряли сознание из-за сильного голода. Последнее, что Вы помните, — перед обмороком Вы мылись в душе...");

            if (rentDebts >= 5)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Expired, 500, 270),
                    "Вас выселили из квартиры. Вам пришлось вернуться в деревню к семье, " +
                    "но Вы не можете ежедневно ходить на работу из-за дальнего расстояния. " +
                    "Вас уволили. Вы обречены жить в бедности до конца своих дней...");

            if (heatingDebts >= 3)
                return new GameOver(controls, new GraphicObject(Properties.Resources.DoctorRecommended, 450, 450),
                    "Вы тяжело заболели и не в состоянии работать. Вас уволили." +
                    "Вы вернулись к своей семье, где Вам суждено жить в бедности до конца своих дней...");

            return null;
        }

        public void SetDifficulty(Difficulties difficulty)
        {
            if (difficulty is Difficulties.Normal)
            {
                loyalityFactor = 1;
                Money = 100;
            }
            else
            {
                loyalityFactor = 2;
                Money = 200;
            }
        }
    }
}
