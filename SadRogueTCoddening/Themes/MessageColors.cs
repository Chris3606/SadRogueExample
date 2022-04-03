using SadConsole;

namespace SadRogueTCoddening.Themes;

internal static class MessageColors
{
    public static readonly ColoredString.ColoredGlyphEffect WelcomeTextAppearance = new()
    {
        Foreground = new(0x20, 0xA0, 0xFF)
    };
    
    public static readonly ColoredString.ColoredGlyphEffect PlayerAtkAppearance = new()
    {
        Foreground = new(0xE0, 0xE0, 0xE0)
    };
    
    public static readonly ColoredString.ColoredGlyphEffect EnemyAtkAtkAppearance = new()
    {
        Foreground = new(0xFF, 0xC0, 0xC0)
    };
    
    public static readonly ColoredString.ColoredGlyphEffect EnemyDiedAppearance = new()
    {
        Foreground = new(0xFF, 0xA0, 0x30)
    };

    public static readonly ColoredString.ColoredGlyphEffect ImpossibleActionAppearance = new()
    {
        Foreground = new(0x80, 0x80, 0x80)
    };
    
    public static readonly ColoredString.ColoredGlyphEffect HealthRecoveredAppearance = new()
    {
        Foreground = new(0x0, 0xFF, 0x0)
    };
}