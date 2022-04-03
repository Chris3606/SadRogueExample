using SadRogue.Integration.FieldOfView.Memory;
using SadRogueTCoddening.MapObjects;

namespace SadRogueTCoddening.Maps;

internal class TerrainFOVVisibilityHandler : MemoryFieldOfViewHandlerBase
{
    protected override void ApplyMemoryAppearance(MemoryAwareRogueLikeCell terrain)
    {
        terrain.LastSeenAppearance.CopyAppearanceFrom(((Terrain)terrain).DarkAppearance);
    }
}