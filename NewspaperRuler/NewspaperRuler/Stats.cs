using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class Stats
    {
        public static GraphicObject NoteBackground { get; set; }
        public static string MonetaryCurrencyName { get; } = "ТОКЕНОВ";

        private int degreeGovernmentAnger = 0;
        private int rentDebts = 0;
        private int productsDebts = 0;

        private DateTime date = new DateTime(1981, 9, 27);
        private int money;
        private readonly DayEnd dayEnd;

        private int rent = 120;
        private int productsCost = 40;

        public int Money
        {
            get { return money; }
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

        public Stats(int money, DayEnd dayEnd)
        {
            Money = money;
            this.dayEnd = dayEnd;
            EventFlags = new List<Dictionary<string, bool>>
            {
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["ArticleAboutChampionWasApproved"] = false
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["ArticleOnProhibitionWeaponsWasApproved"] = false,
                    ["MainCharacterWasOnDate"] = false
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
            date = date.AddDays(1);
            Level = new LevelData(CreateNotes(), ArticleConstructor.ArticlesByLevel[LevelNumber - 1]);
            var introduction = GetIntroduction();
            if (introduction != null) Level.AddIntroduction(introduction);
        }

        public void FinishLevel() => UpdateStatistics();

        private List<Note> CreateNotes()
        {
            var notes = new List<Note>();
            switch (LevelNumber)
            {
                case 1:
                    {
                        var text = "\tЗдравствуй, дорогой!" +
                            "\n\n\tЯ и наш сын Тимоша понимаем, что вступить на должность главного редактора гос. газеты – очень большая ответственность перед всей страной. " +
                            "\n\n\tМы планировали сходить в магазин за продуктами, но внеплановые обязательные налоги не позволили нам это сделать. Пожалуй, схожу на базар. Может, удастся договорится с давними знакомыми торговками." +
                            "\n\n\tУ Тимы на носу школьные выпускные экзамены, но я уверена, что он со всем справится. Он же у нас такой умничка!" +
                            "\n\n\tМы по тебе очень скучаем и сильно ждём!" +
                            "\n\n\tЦелую, твоя любимая жена";
                        notes.Add(new Note(NoteBackground, text, "ОК"));
                        text = "\tТссс... Я раньше был на твоём месте. Меня уволили из-за малешей ошибочки. Руководство не должно узнать об этой записке. Разве не видишь? Аппарат управления прогнил изнутри." +
                            "\n\n\tГосударство устанавливает неведомое количество правил, в результате которых большинство правдивых статей не проходит к публикации." +
                            "\n\n\tНо люди хотят ВСЮ правду. Иначе говоря, отклоняя статьи, доверие читателей к государству теряется. " +
                            "Но доверие читателей - то что ХОЧЕТ видеть государство. От этого зависит и твоя зарплата. То есть, государство сознательно не даёт тебе спокойно работать." +
                            "\n\n\tЯ ничего такого не имею в виду. Просто хочу, чтобы ты пересмотрел то, до чего ты докатился сейчас. Хочу, чтоб ты принимал разумные решения. Работа государственным служащим - дело грязное." +
                            "\n\n\tИ помни, ты меня не знаешь. Тссс...";
                        notes.Add(new Note(NoteBackground, text, "ОК"));
                        break;
                    }
                case 2:
                    {
                        var text = "\tПривет, красавчик!" +
                            "\n\n\tВстретимся в баре \"Алый цветок\" сегодня в 20:00";
                        notes.Add(new Note(NoteBackground, text, "Пойти", "Игнорировать", "Вы пойдёте на свидание с таинственной незнакомкой", 
                            "Вы не пойдёте на свидание с таинственной незнакомкой", "MainCharacterWasOnDate"));
                        if (EventFlags[0]["ArticleAboutChampionWasApproved"])
                            text = "\tМногоуважаемый государственный служащий," +
                                "\n\n\tНе знаю, как статья о моих намерениях завести семью попала к Вам, но из-за Вас сегодня мне отказали в поездке за рубеж, где известные спортсмены со всего мира проводят мастер-классы по олимпийским видам спорта. " +
                                "Туда не берут беременных. Никто бы не узнал, ведь срок моей беременности только начался. Я не разглашала информацию об этом." +
                                "\n\n\tПрошу Вас никоим образом не вмешиваться в мою личную жизнь. Я пожалуюсь в Министерство социальной защиты, так и знайте." +
                                "\n\n\tГалина Руш";
                        else text = "\tМногоуважаемый государственный служащий," +
                                "\n\n\tНе знаю, как статья о моих намерениях завести семью попала в Ваши руки, но я безмерно благодарна Вам, что Вы не опубликовали статью и оставили это в секрете." +
                                "\n\n\tТеперь я смогу отправиться в поездку за рубеж, где известные спортсмены со всего мира проводят мастер-классы по олимпийским видам спорта. " +
                                "Туда не берут беременных, но об этом никто не узнает, ведь срок моей беременности только начался." +
                                "\n\n\tСпасибо Вам ещё раз! Примите от меня нескромный подарок в размере 50 ТОКЕНОВ." +
                                "\n\n\tГалина Руш";
                        notes.Add(new Note(NoteBackground, text, "OK"));
                        break;
                    }
            }
            return notes;
        }

        private Article GetIntroduction()
        {
            var text = new StringBuilder();
            var title = "";
            switch (LevelNumber)
            {
                case 1:
                    text.Append("\tЗдравствуйте, редактор!" +
                        "\n\n\tВ результате перераспределения кадров в области государственной печати мы освободили рабочее место главного редактора, которое теперь занимаете Вы. Просто следуйте инструкциям, и до следующего перераспределения рабочее место останется за Вами." +
                        "\n\n\tВо время исполнения должностных обязанностей Вы освобождаетесь от срочного призыва на военные действия." +
                        "\n\n\tПосле прошедшей войны мы наблюдаем агрессивные настроения граждан: по ряду городов прошли восстания. В наших интересах показать людям, что ситуация налаживается, и Ваша сегодняшняя задача — публиковать только статьи ПОЗИТИВНОГО характера." +
                        "\n\n\tПеретаскивайте ШТАМПЫ на бумагу, чтобы сделать выбор." +
                        "\n\n\tС уважением," +
                        "\n\tМинистерство цензуры и печати");
                    title = date.ToString("D");
                    break;
                case 2:
                    text.Append("\tЗдравствуйте, редактор!");
                    if (EventFlags[0]["MinistryIsSatisfied"])
                        text.Append("\n\n\tМы выражаем восхищение Вашей способностью поднимать " +
                            "позитивные настроения граждан, однако проблем ещё предостаточно.");
                    else text.Append("\n\n\tМы недовольны Вашей работой: восстания продолжаются, " +
                        "а беженцев становится всё больше.");
                    text.Append("\n\n\tВступил в силу приказ №34.11, гласящий, что каждая статья обазана иметь ЗАГОЛОВОК. " +
                        "Отклоняйте все статьи без заголовков. Мы добавили данный приказ в ПЕРЕЧЕНЬ ДЕЙСТВУЮЩИХ ПРИКАЗОВ, " +
                        "который оставили на Вашем рабочем столе. Ознакомьтесь с ним и начинайте работу." +
                        "\n\n\tПродолжайте публиковать только статьи ПОЗИТИВНОГО характера." +
                        "\n\n\tОбращайте внимание на информацию о ПОГОДЕ: граждане любят знать точные прогнозы." +
                        "\n\n\tС уважением," +
                        "\n\tМинистерство цензуры и печати");
                    title = date.ToString("D");
                    break;
            }
            if (text.ToString() == "") return null;
            return new Article(ArticleConstructor.ArticleBackground, text.ToString(), title);
        }

        private void UpdateStatistics()
        {
            if (Level.ReprimandScore < 3)
            {
                if (EventFlags[LevelNumber - 1].ContainsKey("MinistryIsSatisfied"))
                {
                    if (degreeGovernmentAnger > 0) degreeGovernmentAnger--;
                    EventFlags[LevelNumber - 1]["MinistryIsSatisfied"] = true;
                }
            }
            else degreeGovernmentAnger++;

            Loyality += Level.Loyality;
            Money += Level.Salary - Level.GetTotalFine();

            LevelNumber++;

            dayEnd.InformationTexts.Add(GetLabel(GetWarnings()));

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
            }
            dayEnd.StatsTexts.Add(GetLabel($"Итого:\t\t{Money} {MonetaryCurrencyName}"));

            dayEnd.Expenses.Add(new Expense($"Квартплата:\t\t{rent} {MonetaryCurrencyName}", rent, ExpenseType.Rent));
            dayEnd.Expenses.Add(new Expense($"Продукты:\t\t{productsCost} {MonetaryCurrencyName}", productsCost, ExpenseType.Products));

            dayEnd.RecalculatePositions();

            Label GetLabel(string text)
            {
                return new Label
                {
                    Text = text,
                    ForeColor = Color.White,
                    Font = StringStyle.TitleFont,
                    AutoSize = true
                };
            }

            string GetWarnings()
            {
                var information = new StringBuilder();

                if (degreeGovernmentAnger == 2)
                    information.Append("Руководство недовольно Вашей работой и ищет нового кандидата на Ваше место. ");

                if (productsDebts == 1) information.Append("Вы голодны. ");
                else if (productsDebts == 2) information.Append("Вы умираете с голоду. ");

                if (rentDebts >= 2 && rentDebts <= 3) information.Append("У Вас остались неоплаченные долги по счетам. ");
                else if (rentDebts >= 4 && rentDebts <= 5) information.Append("Коммунальное хозяйство " +
                    "выселит Вас из квартиры, если Вы не закроете долги по счетам. ");

                return information.ToString();
            }
        }

        public void ApplyExpenses()
        {
            foreach (var expence in dayEnd.Expenses)
            {
                if (expence.Marked) continue;
                if (expence.Type is ExpenseType.Rent) rentDebts += 3;
                else if (expence.Type is ExpenseType.Products) productsDebts += 2;
            }
            if (rentDebts > 0) rentDebts--;
            if (productsDebts > 0) productsDebts--;
        }
    }
}
