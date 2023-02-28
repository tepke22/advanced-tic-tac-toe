using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTicTacToe
{
    static public class ResLibrary
    {

        // Colors
        public static Color darkBlue = Color.FromArgb(7, 23, 49);
        public static Color lightBlue = Color.FromArgb(76, 153, 203);
        public static Color red = Color.FromArgb(224, 60, 67);
        public static Color white = Color.White;

        // Labels
        public static string humanLbl = "Čovek";
        public static string computerLbl = "Računar";

        // Draw
        public static string DrawBracketsLbl = "{ Nerešeno }";
        public static string DrawLbl = "Nerešeno";
        public static string DrawModalMessage = "Nerešeno\n\nRestartovati igru?";
    }
}
