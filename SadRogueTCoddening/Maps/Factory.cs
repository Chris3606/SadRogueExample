using GoRogue.MapGeneration;
using GoRogue.Random;
using SadRogue.Integration.FieldOfView.Memory;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace SadRogueTCoddening.Maps
{
    /// <summary>
    /// Basic factory which produces different types of maps.
    /// </summary>
    /// <remarks>
    /// TODO: Better integrate into GoRogue map gen, and convert to use GoRogue.Factories.
    /// </remarks>
    internal static class Factory
    {
        public static GameMap Dungeon(int width, int height)
        {
            // Generate a rectangular map for the sake of testing with GoRogue's map generation system.
            var generator = new Generator(width, height)
                .ConfigAndGenerateSafe(gen =>
                {
                    gen.AddSteps(DefaultAlgorithms.RectangleMapSteps());
                });

            var generatedMap = generator.Context.GetFirst<ISettableGridView<bool>>("WallFloor");

            // Create actual integration library map.
            var map = new GameMap(generator.Context.Width, generator.Context.Height, null);

            // Add a component that will implement a character "memory" system.
            map.AllComponents.Add(new DimmingMemoryFieldOfViewHandler(0.6f));

            // Translate GoRogue's terrain data into actual integration library objects.  Our terrain must be of type
            // MemoryAwareRogueLikeCells because we are using the integration library's "memory-based" fov visibility
            // system.
            map.ApplyTerrainOverlay(generatedMap, (pos, val) => val ? MapObjects.Factory.Floor(pos) : MapObjects.Factory.Wall(pos));

            // Add player to map at a random walkable position
            Engine.Player.Position = GlobalRandom.DefaultRNG.RandomPosition(map.WalkabilityView, true);
            map.AddEntity(Engine.Player);
            
            // Generate 10 enemies, placing them in random walkable locations for demo purposes.
            for (int i = 0; i < 10; i++)
            {
                var enemy = MapObjects.Factory.Enemy();
                enemy.Position = GlobalRandom.DefaultRNG.RandomPosition(map.WalkabilityView, true);
                map.AddEntity(enemy);
            }

            return map;
        }
    }
}
