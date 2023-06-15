using SadConsole;

namespace SadRogueExample.Themes;

/// <summary>
/// Static class which defines <see cref="ColoredString.ColoredGlyphEffect"/> instances which define the colors used for
/// different types of messages.
/// </summary>
internal static class MessageColors
{
    /// <summary>
    /// Initial welcome text printed on dungeon entrance.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect WelcomeTextAppearance = new()
    {
        Foreground = new(0x20, 0xA0, 0xFF)
    };

    /// <summary>
    /// Text indicating the player attacked something.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect PlayerAtkAppearance = new()
    {
        Foreground = new(0xE0, 0xE0, 0xE0)
    };

    /// <summary>
    /// Text indicating an enemy attacked the player.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect EnemyAtkAppearance = new()
    {
        Foreground = new(0xFF, 0xC0, 0xC0)
    };

    /// <summary>
    /// Text indicating the player died.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect PlayerDiedAppearance = new()
    {
        Foreground = new(0xFF, 0x30, 0x30)
    };

    /// <summary>
    /// Text indicating an enemy died.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect EnemyDiedAppearance = new()
    {
        Foreground = new(0xFF, 0xA0, 0x30)
    };

    /// <summary>
    /// Text indicating the player tried to take an action which is not possible (ie. moving into a wall).
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect ImpossibleActionAppearance = new()
    {
        Foreground = new(0x80, 0x80, 0x80)
    };

    /// <summary>
    /// Text indicating the player picked up an item.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect ItemPickedUpAppearance = new()
    {
        Foreground = new(0xFF, 0xFF, 0xFF)
    };

    /// <summary>
    /// Text indicating the player dropped an item.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect ItemDroppedAppearance = new()
    {
        Foreground = new(0xFF, 0xFF, 0xFF)
    };

    /// <summary>
    /// Text indicating the player recovered health.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect HealthRecoveredAppearance = new()
    {
        Foreground = new(0x0, 0xFF, 0x0)
    };

    /// <summary>
    /// Text indicating that a used item needs a target.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect NeedsTargetAppearance = new()
    {
        Foreground = new(0x3F, 0xFF, 0xFF)
    };

    /// <summary>
    /// Text indicating that a status effect was applied.
    /// </summary>
    public static readonly ColoredString.ColoredGlyphEffect StatusEffectAppliedAppearance = new()
    {
        Foreground = new(0x3F, 0xFF, 0xFF)
    };
}