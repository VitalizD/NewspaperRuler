using System.Collections.Generic;
using System;
using System.IO;
using System.Text.Json;

namespace NewspaperRuler
{
    [Serializable]
    public class SavedData
    {
        public const string name = "Save.json";

        public Flags[] Flags { get; set; }
        public int DegreeGovernmentAnger { get; set; }
        public int RentDebts { get; set; }
        public int ProductsDebts { get; set; }
        public int HeatingDebts { get; set; }
        public int Rent { get; set; }
        public int ProductsCost { get; set; }
        public int HeatingCost { get; set; }
        public int Money { get; set; }
        public int LoyalityFactor { get; set; }
        public int Year { get; set; }
        public int Mouth { get; set; }
        public int Day { get; set; }
        public int LevelNumber { get; set; }
        public int Loyality { get; set; }
        public Difficulties Difficulty { get; set; }
        public bool DecreesAreVisible { get; set; }
        public bool TrendsAreVisible { get; set; }
        public int MinistrySatisfactionsCount { get; set; }
        public int RequiredLoyality { get; set; }

        public void ToJson()
        {
            var jsonText = JsonSerializer.Serialize(this, AuxiliaryMethods.JsonSerializerOptions);
            File.WriteAllText(name, jsonText);
        }

        public List<Dictionary<string, bool>> GetFlags()
        {
            var result = new List<Dictionary<string, bool>>();
            foreach (var flag in Flags)
            {
                var dict = new Dictionary<string, bool>();
                for (var i = 0; i < flag.Keys.Length; i++)
                    dict.Add(flag.Keys[i], flag.Values[i]);
                result.Add(dict);
            }
            return result;
        }
    }

    [Serializable]
    public class Flags
    {
        public string[] Keys { get; set; }
        public bool[] Values { get; set; }

        public Flags(string[] keys, bool[] values)
        {
            Keys = keys;
            Values = values;
        }
    }
}
