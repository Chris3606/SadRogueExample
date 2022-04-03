using SadConsole;
using SadRogue.Primitives;

namespace SadRogueTCoddening;

/// <summary>
/// A place for default constants (for now)
/// </summary>
internal static class Constants
{
    // Window width/height
    public const int ScreenWidth = 80;
    public const int ScreenHeight = 25;

    public static readonly Color WelcomeTextColor = new(0x20, 0xA0, 0xFF);

    public static readonly Color PlayerAtkTextColor = new(0xE0, 0xE0, 0xE0);
    public static readonly Color EnemyAtkTextColor = new(0xFF, 0xC0, 0xC0);
    
    public static readonly Color EnemyDiedTextColor = new(0xFF, 0xA0, 0x30);
    
}