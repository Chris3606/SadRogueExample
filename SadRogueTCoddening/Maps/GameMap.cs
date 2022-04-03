using System;
using SadRogue.Integration.Maps;
using SadRogue.Primitives;

namespace SadRogueTCoddening.Maps
{
    /// <summary>
    /// Basic game map which determines layers based on a Layer enumeration.
    /// </summary>
    internal class GameMap : RogueLikeMap
    {
        /// <summary>
        /// Map layers for rendering/collision.
        /// </summary>
        public enum Layer
        {
            Terrain = 0,
            Items,
            Monsters
        }
        
        public GameMap(int width, int height, DefaultRendererParams? defaultRendererParams)
            : base(width, height, defaultRendererParams, Enum.GetValues<Layer>().Length - 1, Distance.Chebyshev)
        { }
    }
}
