using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;

namespace NewspaperRuler
{
    public static class AuxiliaryMethods
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; } =
            new JsonSerializerOptions { WriteIndented = true };

        public static bool IsClickedOnArea(Rectangle area) =>
            Cursor.Position.X >= area.X && Cursor.Position.X <= area.X + area.Width
            && Cursor.Position.Y >= area.Y && Cursor.Position.Y <= area.Y + area.Height;

        public static SavedData GetSave()
        {
            if (!SaveExists()) return null;
            var jsonText = File.ReadAllText(SavedData.name);
            var savedData = JsonSerializer.Deserialize<SavedData>(jsonText, JsonSerializerOptions);
            return savedData;
        }

        public static bool SaveExists() => File.Exists(SavedData.name);
    }
}
