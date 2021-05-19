using System;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public static class Scl
    {
        /// <summary>
        /// Масшатбный коэффициент
        /// </summary>
        public static double Factor { get; } = Math.Min(
            SystemInformation.PrimaryMonitorMaximizedWindowSize.Width / 1552.0,
            SystemInformation.PrimaryMonitorMaximizedWindowSize.Height / 840.0) + 0.08;

        public static Size Resolution { get; } = SystemInformation.PrimaryMonitorMaximizedWindowSize;

        /// <summary>
        /// Возвращает число, умноженное на мастабшный коэффициент
        /// </summary>
        public static int Get(int number) => (int)(Factor * number);
    }
}
