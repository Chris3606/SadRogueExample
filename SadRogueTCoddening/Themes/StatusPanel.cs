using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;

namespace SadRogueTCoddening.Themes;

internal static class StatusPanel
{
    public static readonly ProgressBarTheme HPBarTheme = GetHPBarTheme();

    private static ProgressBarTheme GetHPBarTheme()
    {
        // Color setting doesn't work right now, not sure why
        var theme = (ProgressBarTheme)Library.Default.GetControlTheme(typeof(ProgressBar));
        theme.Background.SetBackground(Color.Red);
        theme.Background.SetForeground(Color.Red);
        theme.Foreground.SetBackground(Color.Green);
        theme.Foreground.SetForeground(Color.Green);

        return theme;
    }
}