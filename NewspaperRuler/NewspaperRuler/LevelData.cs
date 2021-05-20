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
            get { return fixedSalaryAmount; }
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
                return FixedSalaryAmount + Loyality * 50;
            }
        }

        public int BonusMoney { get; private set; } = 0;

        public int CurrentFine { get; private set; } = 0;
        public bool FirstFine { get; private set; } = true;

        public List<object> Events { get; } = new List<object>();

        public LevelData(List<Note> notes, List<Article> articles)
        {
            foreach (var note in notes) Events.Add(note);
            foreach (var article in articles) Events.Add(article);
            FixedSalaryAmount = 100;
        }

        public void AddIntroduction(Article introduction) => Events.Insert(0, introduction);

        public void IncreaseLoyality(int value) => Loyality += value;

        public void IncreaseReprimandScore(int value) => ReprimandScore += value;

        public void IncreaseFine()
        {
            if (FirstFine) FirstFine = false;
            else
            {
                CurrentFine += 100;
                totalFine += CurrentFine;
            }
        }

        public void IncreaseBonusMoney(int value) => BonusMoney += value;

        public int GetTotalFine() => totalFine;
    }
}
