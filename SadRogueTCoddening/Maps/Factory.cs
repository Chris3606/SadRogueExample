using System;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace SadRogueTCoddening.Maps;

/// <summary>
/// Basic factory which produces different types of maps.
/// </summary>
/// <remarks>
/// TODO: Better integrate into GoRogue map gen, and convert to use GoRogue.Factories.
/// </remarks>
internal static class Factory
{
    public static GameMap Dungeon()
    {
        // Generate a dungeon maze map
        var generator = new Generator(100, 60)
            .ConfigAndGenerateSafe(gen =>
            {
                gen.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps(minRooms: 20, maxRooms: 30, roomMinSize: 8, roomMaxSize: 12, saveDeadEndChance: 0));
            });

        var generatedMap = generator.Context.GetFirst<ISettableGridView<bool>>("WallFloor");

        // Create actual integration library map.
        var map = new GameMap(generator.Context.Width, generator.Context.Height, null);

        // Add a component that will implement a character "memory" system.
        map.AllComponents.Add(new TerrainFOVVisibilityHandler());

        // Translate GoRogue's terrain data into actual integration library objects.  Our terrain must be of type
        // MemoryAwareRogueLikeCells because we are using the integration library's "memory-based" fov visibility
        // system.
        map.ApplyTerrainOverlay(generatedMap, (pos, val) => val ? MapObjects.Factory.Floor(pos) : MapObjects.Factory.Wall(pos));

        // Add player to map at the center of the first room we placed
        var rooms = generator.Context.GetFirst<ItemList<Rectangle>>("Rooms");
        Engine.Player.Position = rooms.Items[0].Center;
        map.AddEntity(Engine.Player);
            
        // Generate between zero and two monsters per room.  Each monster has an 80% chance of being an orc (weaker)
        // and a 20% chance of being a troll (stronger).
        foreach (var room in rooms.Items)
        {
            int enemies = GlobalRandom.DefaultRNG.NextInt(0, 3);
            for (int i = 0; i < enemies; i++)
            {
                bool isOrc = GlobalRandom.DefaultRNG.PercentageCheck(80f);
                    
                var enemy = isOrc ? MapObjects.Factory.Orc() : MapObjects.Factory.Troll();
                enemy.Position = RandomPositionFromRect(GlobalRandom.DefaultRNG, room, pos => map.WalkabilityView[pos]);
                map.AddEntity(enemy);
            }
        }

        return map;
    }

    private static Point RandomPositionFromRect(IEnhancedRandom rng, Rectangle rect, Func<Point, bool> selector)
    {
        var pos = new Point(rng.NextInt(rect.X, rect.X + rect.Width), rng.NextInt(rect.Y, rect.Y + rect.Height));
        while (!selector(pos))
            pos = new Point(rng.NextInt(rect.X, rect.X + rect.Width), rng.NextInt(rect.Y, rect.Y + rect.Height));

        return pos;
    }
}