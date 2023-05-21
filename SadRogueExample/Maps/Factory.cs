using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using GoRogue.Random;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace SadRogueExample.Maps;

/// <summary>
/// Configuration object specifying the parameters for dungeon generation.
/// </summary>
/// <param name="Width">The width of the generated dungeon map.</param>
/// <param name="Height">The height of the generated dungeon map.</param>
/// <param name="MinRooms">The minimum number of rooms that the generated dungeon map will have.</param>
/// <param name="MaxRooms">The maximum number of rooms that the generated dungeon map will have.</param>
/// <param name="RoomMinSize">The minimum size of a room.</param>
/// <param name="RoomMaxSize">The maximum size of a room.</param>
/// <param name="MaxMonstersPerRoom">The maximum number of monsters that can spawn in a single room.</param>
/// <param name="MaxItemsPerRoom">The maximum number of items that can spawn in a single room.</param>
public readonly record struct DungeonGenConfig(int Width, int Height, int MinRooms, int MaxRooms, int RoomMinSize,
    int RoomMaxSize, int MaxMonstersPerRoom, int MaxItemsPerRoom);

/// <summary>
/// Basic factory which produces different types of maps.
/// </summary>
/// <remarks>
/// The map gen below won't use GoRogue's map generation system for the custom parts, although we could; it simply isn't
/// necessary for the relatively simple, single type of map we have below.
/// </remarks>
internal static class Factory
{
    public static GameMap Dungeon(DungeonGenConfig config)
    {
        // Generate a dungeon maze map
        var generator = new Generator(config.Width, config.Height)
            .ConfigAndGenerateSafe(gen =>
            {
                gen.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps(minRooms: config.MinRooms, maxRooms: config.MaxRooms,
                    roomMinSize: config.RoomMinSize, roomMaxSize: config.RoomMaxSize, saveDeadEndChance: 0));
            });

        // Extract components from the map GoRogue generated which hold basic information about the map
        var generatedMap = generator.Context.GetFirst<ISettableGridView<bool>>("WallFloor");
        var rooms = generator.Context.GetFirst<ItemList<Rectangle>>("Rooms");

        // Create actual integration library map with a proper component for the character "memory" system.
        var map = new GameMap(generator.Context.Width, generator.Context.Height, null);
        map.AllComponents.Add(new TerrainFOVVisibilityHandler());

        // Translate GoRogue's terrain data into actual integration library objects.
        map.ApplyTerrainOverlay(generatedMap, (pos, val) => val ? MapObjects.Factory.Floor(pos) : MapObjects.Factory.Wall(pos));

        // Spawn player
        SpawnPlayer(map, rooms);

        // Spawn enemies/items/etc
        SpawnMonsters(map, rooms, config.MaxMonstersPerRoom);
        SpawnItems(map, rooms, config.MaxItemsPerRoom);

        return map;
    }

    private static void SpawnPlayer(GameMap map, ItemList<Rectangle> rooms)
    {
        // Add player to map at the center of the first room we placed
        Engine.Player.Position = rooms.Items[0].Center;
        map.AddEntity(Engine.Player);
    }

    private static void SpawnMonsters(GameMap map, ItemList<Rectangle> rooms, int maxMonstersPerRoom)
    {
        // Generate between zero and the max monsters per room.  Each monster has an 80% chance of being an orc (weaker)
        // and a 20% chance of being a troll (stronger).
        foreach (var room in rooms.Items)
        {
            int enemies = GlobalRandom.DefaultRNG.NextInt(0, maxMonstersPerRoom + 1);
            for (int i = 0; i < enemies; i++)
            {
                bool isOrc = GlobalRandom.DefaultRNG.PercentageCheck(80f);

                var enemy = isOrc ? MapObjects.Factory.Orc() : MapObjects.Factory.Troll();
                enemy.Position = GlobalRandom.DefaultRNG.RandomPosition(room, pos => map.WalkabilityView[pos]);
                map.AddEntity(enemy);
            }
        }
    }

    private static void SpawnItems(GameMap map, ItemList<Rectangle> rooms, int maxItemsPerRoom)
    {
        // Generate between zero and the max items per room.
        foreach (var room in rooms.Items)
        {
            int items = GlobalRandom.DefaultRNG.NextInt(0, maxItemsPerRoom + 1);
            for (int i = 0; i < items; i++)
            {
                var isPotion = GlobalRandom.DefaultRNG.PercentageCheck(10f);
                var item = isPotion ? MapObjects.Items.Factory.HealthPotion() : MapObjects.Items.Factory.ConfusionScroll();
                item.Position = GlobalRandom.DefaultRNG.RandomPosition(room, pos => map.WalkabilityView[pos]);
                map.AddEntity(item);
            }
        }
    }
}