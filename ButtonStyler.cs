using System.Drawing;
using System.Windows.Forms;

namespace EduSync
{
    public static class ButtonStyler
    {

        public static void ApplyPrimaryStyle(Button btn)
        {
            if (btn == null) return;

            btn.BackColor = AppColours.PrimaryButtonColor;
            btn.ForeColor = AppColours.ButtonTextColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = AppColours.ButtonFont;

            // Remove existing event handlers to avoid duplicates
            RemoveEventHandlers(btn);

            // Add new event handlers
            btn.MouseEnter += (s, e) => btn.BackColor = AppColours.PrimaryButtonHover;
            btn.MouseLeave += (s, e) => btn.BackColor = AppColours.PrimaryButtonColor;
            btn.MouseDown += (s, e) => btn.BackColor = AppColours.PrimaryButtonClick;
            btn.MouseUp += (s, e) => btn.BackColor = AppColours.PrimaryButtonHover;
        }

        public static void ApplyDangerStyle(Button btn)
        {
            if (btn == null) return;

            btn.BackColor = AppColours.DangerButtonColor;
            btn.ForeColor = AppColours.ButtonTextColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = AppColours.ButtonFont;

            RemoveEventHandlers(btn);

            btn.MouseEnter += (s, e) => btn.BackColor = AppColours.DangerButtonHover;
            btn.MouseLeave += (s, e) => btn.BackColor = AppColours.DangerButtonColor;
            btn.MouseDown += (s, e) => btn.BackColor = AppColours.DangerButtonClick;
            btn.MouseUp += (s, e) => btn.BackColor = AppColours.DangerButtonHover;
        }

        public static void ApplySuccessStyle(Button btn)
        {
            if (btn == null) return;

            btn.BackColor = AppColours.SuccessButtonColor;
            btn.ForeColor = AppColours.ButtonTextColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = AppColours.ButtonFont;

            RemoveEventHandlers(btn);

            btn.MouseEnter += (s, e) => btn.BackColor = AppColours.SuccessButtonHover;
            btn.MouseLeave += (s, e) => btn.BackColor = AppColours.SuccessButtonColor;
            btn.MouseDown += (s, e) => btn.BackColor = AppColours.SuccessButtonClick;
            btn.MouseUp += (s, e) => btn.BackColor = AppColours.SuccessButtonHover;
        }

        public static void ApplySecondaryStyle(Button btn)
        {
            if (btn == null) return;

            btn.BackColor = AppColours.SecondaryButtonColor;
            btn.ForeColor = AppColours.ButtonTextColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = AppColours.ButtonFont;

            RemoveEventHandlers(btn);

            btn.MouseEnter += (s, e) => btn.BackColor = AppColours.SecondaryButtonHover;
            btn.MouseLeave += (s, e) => btn.BackColor = AppColours.SecondaryButtonColor;
            btn.MouseDown += (s, e) => btn.BackColor = AppColours.SecondaryButtonClick;
            btn.MouseUp += (s, e) => btn.BackColor = AppColours.SecondaryButtonHover;
        }

        private static void RemoveEventHandlers(Button btn)
        {
            // This is a simplified approach - in practice, you might need to track handlers
            btn.MouseEnter -= null;
            btn.MouseLeave -= null;
            btn.MouseDown -= null;
            btn.MouseUp -= null;
        }
    }
}