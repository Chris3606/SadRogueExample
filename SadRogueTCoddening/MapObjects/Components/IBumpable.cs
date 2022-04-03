using SadRogue.Integration;

namespace SadRogueTCoddening.MapObjects.Components;

/// <summary>
/// Interface implemented by anything that reacts to bumps.
/// </summary>
public interface IBumpable
{
    /// <summary>
    /// Does whatever bump action is needed, using the given entity as the source.  Returns true if a bump action was
    /// taken, false otherwise.
    /// </summary>
    bool OnBumped(RogueLikeEntity source);
}