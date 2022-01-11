using System.Collections.Generic;

namespace NewspaperRuler
{
    public static class Translation
    {
        public static Dictionary<Difficulties, string> ruDifficulties = new Dictionary<Difficulties, string>
        {
            [Difficulties.Easy] = "лёгкая",
            [Difficulties.Normal] = "обычная"
        };
    }
}
