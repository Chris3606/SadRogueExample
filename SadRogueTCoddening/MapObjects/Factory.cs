using SadRogue.Integration;
using SadRogue.Integration.FieldOfView.Memory;
using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;
using SadRogueTCoddening.MapObjects.Components;
using SadRogueTCoddening.Maps;

namespace SadRogueTCoddening.MapObjects
{
    /// <summary>
    /// Simple class with some static functions for creating map objects.
    /// </summary>
    /// <remarks>
    /// TODO: Port to GoRogue's factory system.
    /// </remarks>
    internal static class Factory
    {
        public static MemoryAwareRogueLikeCell Floor(Point position)
            => new(position, Color.White, Color.Black, '.', (int)GameMap.Layer.Terrain);

        public static MemoryAwareRogueLikeCell Wall(Point position)
            => new(position, Color.White, Color.Black, '#', (int)GameMap.Layer.Terrain, false);

        public static RogueLikeEntity Player()
        {
            // Create entity with appropriate attributes
            var player = new RogueLikeEntity('@', false, layer: (int)GameMap.Layer.Monsters);

            // Add component for controlling player movement via keyboard.  Other (non-movement) keybindings can be
            // added as well
            var motionControl = new CustomPlayerKeybindingsComponent();
            motionControl.SetMotions(PlayerKeybindingsComponent.ArrowMotions);
            motionControl.SetMotions(PlayerKeybindingsComponent.NumPadAllMotions);
            player.AllComponents.Add(motionControl);

            // Add component for updating map's player FOV as they move
            player.AllComponents.Add(new PlayerFOVController());

            return player;
        }

        public static RogueLikeEntity Enemy()
        {
            var enemy = new RogueLikeEntity(Color.Red, 'g', false, layer: (int)GameMap.Layer.Monsters);

            // Add AI component to path toward player when in view
            enemy.AllComponents.Add(new DemoEnemyAI());

            return enemy;
        }

    }
}
