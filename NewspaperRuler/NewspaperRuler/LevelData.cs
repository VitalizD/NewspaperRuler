using System;
using System.Collections.Generic;
using System.Linq;

namespace NewspaperRuler
{
    public class LevelData
    {
        private static int fixedSalaryAmount = 100;
        private int totalFine = 0;

        public static int FixedSalaryAmount
        {
            get => fixedSalaryAmount;
            set
            {
                if (value < 0) throw new ArgumentException("The value can't be less than zero");
                fixedSalaryAmount = value;
            }
        }

        public int Loyality { get; private set; } = 0;

        public int ReprimandScore { get; private set; } = 0;

        public int Salary
        {
            get 
            {
                if (Loyality <= 0) return FixedSalaryAmount;
                return FixedSalaryAmount + (int)(Loyality * MoneyFactor);
            }
        }

        private double MoneyFactor
        {
            get
            {
                var result = 37 - Loyality * 1.5;
                if (result < 10) return 10;
                return result;
            }
        }

        public int CurrentFine { get; private set; } = 0;
        public bool FirstFine { get; private set; } = true;

        public Queue<object> Events { get; private set; }
        private readonly List<object> preEvents = new List<object>();

        public LevelData(List<Note> notes, List<Article> articles)
        {
            foreach (var note in notes) preEvents.Add(note);
            foreach (var article in articles) preEvents.Add(article);

            var maxIndex = 0;

            foreach (var note in notes)
                if (note.NumberInQueue >= 0)
                {
                    preEvents.Remove(note);
                    preEvents.Insert(note.NumberInQueue, note);
                    if (note.NumberInQueue > maxIndex) maxIndex = note.NumberInQueue;
                }
            foreach (var article in articles)
                if (article.NumberInQueue >= 0)
                {
                    preEvents.Remove(article);
                    preEvents.Insert(article.NumberInQueue, article);
                    if (article.NumberInQueue > maxIndex) maxIndex = article.NumberInQueue;
                }
            Mix(maxIndex);
        }

        /// <summary>
        /// Перемешивает игровые события.
        /// </summary>
        /// <param name="index">Отсчитываемый от нуля индекс, от которого и до конца коллекции перемешиваются элементы</param>
        private void Mix(int index)
        {
            var random = new Random();
            for (int i = preEvents.Count - 1; i > index; i--)
            {
                int j = random.Next(index + 1, i + 1);
                var temp = preEvents[j];
                preEvents[j] = preEvents[i];
                preEvents[i] = temp;
            }
        }

        public void Insert(int index, Article value) => preEvents.Insert(index, value);

        public void Insert(int index, Note value) => preEvents.Insert(index, value);

        public void BuildEventQueue() => Events = new Queue<object>(preEvents);

        public void IncreaseLoyality(int value) => Loyality += value;

        public void IncreaseReprimandScore(int value) => ReprimandScore += value;

        public void IncreaseFine()
        {
            if (FirstFine) FirstFine = false;
            else
            {
                CurrentFine += 50;
                totalFine += CurrentFine;
            }
        }

        public int GetTotalFine() => totalFine;
    }
}
