using System.Drawing;

namespace EduSync
{
    internal static class AppColours
    {
        // Danger button
        public static readonly Color DangerButtonColor = Color.Red;
        public static readonly Color DangerButtonHover = Color.DarkRed;
        public static readonly Color DangerButtonClick = Color.Maroon;

        // Primary button
        public static readonly Color PrimaryButtonColor = Color.FromArgb(45, 156, 219);
        public static readonly Color PrimaryButtonHover = Color.FromArgb(30, 136, 200);
        public static readonly Color PrimaryButtonClick = Color.FromArgb(20, 120, 180);

        // Secondary button
        public static readonly Color SecondaryButtonColor = Color.Gray;
        public static readonly Color SecondaryButtonHover = Color.DarkGray;
        public static readonly Color SecondaryButtonClick = Color.DimGray;

        // Success button
        public static readonly Color SuccessButtonColor = Color.Green;
        public static readonly Color SuccessButtonHover = Color.DarkGreen;
        public static readonly Color SuccessButtonClick = Color.SeaGreen;

        // Button text + font
        public static readonly Color ButtonTextColor = Color.White;
        public static readonly Font ButtonFont = new Font("Segoe UI", 10, FontStyle.Bold);

        // Panel colors (soft teal theme)
        public static readonly Color PanelBackground = Color.FromArgb(178, 223, 219); // Soft teal
        public static readonly Color PanelBorder = Color.FromArgb(128, 203, 196);     // Slightly darker teal
        public static readonly Color PanelHighlight = Color.FromArgb(77, 182, 172);   // Stronger teal for accents
    }
}
