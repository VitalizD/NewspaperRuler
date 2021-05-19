using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NewspaperRuler
{
    public class Stats
    {
        public static GraphicObject NoteBackground { get; set; }

        private int degreeGovernmentAnger = 0;
        private DateTime date = new DateTime(1981, 9, 27);

        public int Money { get; private set; }

        public int LevelNumber { get; private set; } = 0;

        public int Loyality { get; private set; }
        
        public LevelData Level { get; set; }

        public List<Dictionary<string, bool>> EventFlags { get; }

        public Stats(int money)
        {
            Money = money;
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
            LevelNumber++;
            date = date.AddDays(1);
            Level = new LevelData(CreateNotes(), ArticleConstructor.ArticlesByLevel[LevelNumber - 1]);
            var introduction = GetIntroduction();
            if (introduction != null) Level.AddIntroduction(introduction);
        }

        public void FinishLevel()
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
            Money += Level.Salary;
            GoToNextLevel();
        }

        private List<Note> CreateNotes()
        {
            var notes = new List<Note>();
            switch (LevelNumber)
            {
                case 1:
                    {
                        var text = "\tЗдравствуй, дорогой!" +
                            "\n\n\tЯ и наш сын Тимоша понимаем, что вступить на должность главного редактора гос.газеты – очень большая ответственность перед всей страной. " +
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
                                "\n\n\tСпасибо Вам ещё раз! Примите от меня нескромный подарок в размере 50 ТОКЕНОВ" +
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
                        "\n\n\tВ ходе глобальных структурных преобразований Министерство цензуры и печати выделило 1 рабочее место в государственной газете, которое теперь занимаете Вы.Просто следуйте инструкциям, и мы закрепим за Вами рабочее место насовсем." +
                        "\n\n\tВо время исполнения должностных обязанностей Вы освобождаетесь от срочного призыва на военные действия." +
                        "\n\n\tПосле прошедшей войны мы наблюдаем агрессивные настроения граждан: по ряду городов прошли восстания. Ваша задача – повысить лояльность и доверие народа к государству, а для этого условимся публиковать только ПОЗИТИВНЫЕ статьи." +
                        "\n\n\tПеретаскивайте ШТАМПЫ на бумагу, чтобы сделать выбор. Попробуйте сейчас!");
                    title = date.ToString("D");
                    break;
                case 2:
                    text.Append("\tЗдравствуйте, редактор!");
                    if (EventFlags[0]["MinistryIsSatisfied"])
                        text.Append("\n\n\tМинистерство цензуры и печати выражает восхищение Вашей способностью поднимать " +
                            "позитивные настроения граждан, однако проблем ещё предостаточно.");
                    else text.Append("\n\n\tМинистерство цензуры и печати недовольно Вашей работой: восстания продолжаются, " +
                        "а беженцев становится всё больше.");
                    text.Append("\n\n\tВступил в силу приказ №34.11, гласящий, что каждая статья обазана иметь ЗАГОЛОВОК. " +
                        "Отклоняйте все статьи без заголовков. Мы добавили данный приказ в ПЕРЕЧЕНЬ ДЕЙСТВУЮЩИХ ПРИКАЗОВ, " +
                        "который оставили на Вашем рабочем столе. Ознакомьтесь с ним и начинайте работу." +
                        "\n\n\tПродолжайте публиковать только ПОЗИТИВНЫЕ статьи." +
                        "\n\n\tОбращайте внимание на информацию о ПОГОДЕ: граждане любят знать точные прогнозы.");
                    title = date.ToString("D");
                    break;
            }
            if (text.ToString() == "") return null;
            return new Article(ArticleConstructor.ArticleBackground, text.ToString(), title);
        }
    }
}
