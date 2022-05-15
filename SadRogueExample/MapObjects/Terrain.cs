using System;
using GoRogue.Components;
using SadConsole;
using SadRogue.Integration.FieldOfView.Memory;
using SadRogue.Primitives;

namespace SadRogueExample.MapObjects;

/// <summary>
/// A MemoryAwareRogueLikeCell class used to represent all terrain objects.  Implements fields necessary for our FOV darkening method.
/// </summary>
internal class Terrain : MemoryAwareRogueLikeCell
{
    public ColoredGlyph DarkAppearance { get; }

    public Terrain(TerrainAppearanceDefinition appearance, int layer, bool walkable = true,
                   bool transparent = true, Func<uint>? idGenerator = null,
                   IComponentCollection? customComponentContainer = null)
        : base(appearance.Light, layer, walkable, transparent, idGenerator, customComponentContainer)
    {
        DarkAppearance = new ColoredGlyph();
        DarkAppearance.CopyAppearanceFrom(appearance.Dark);
    }

    public Terrain(Point position, TerrainAppearanceDefinition appearance, int layer,
                   bool walkable = true,
                   bool transparent = true, Func<uint>? idGenerator = null,
                   IComponentCollection? customComponentContainer = null)
        : base(position, appearance.Light, layer, walkable, transparent, idGenerator, customComponentContainer)
    {
        DarkAppearance = new ColoredGlyph();
        DarkAppearance.CopyAppearanceFrom(appearance.Dark);
    }
}