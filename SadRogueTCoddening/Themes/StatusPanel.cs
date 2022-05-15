using SadConsole.UI;
using SadConsole.UI.Themes;

namespace SadRogueTCoddening.Themes;

/// <summary>
/// Colors/themes related to the Screens.Surfaces.StatusPanel.
/// </summary>
internal static class StatusPanel
{

    public static readonly Colors HPBarColors = GetHPBarColors();

    private static Colors GetHPBarColors()
    {
        var colors = Library.Default.Colors.Clone();
        colors.Appearance_ControlNormal.Foreground = new (0x0, 0x60, 0x0);
        colors.Appearance_ControlNormal.Background = new (0x40, 0x10, 0x10);

        return colors;
    }
}