using System;
using System.Linq;
using GoRogue.GameFramework;
using SadConsole;
using SadRogue.Integration;
using SadRogue.Primitives;
using SadRogueTCoddening.MapObjects.Components;
using SadRogueTCoddening.Maps;
using SadRogueTCoddening.Screens;
using SadRogueTCoddening.Themes;
using Factory = SadRogueTCoddening.MapObjects.Factory;

namespace SadRogueTCoddening;

/// <summary>
/// Main class containing the program's entry point, which runs the game's core loop and implements core activities/rules such as how to handle
/// player/hostile death.
/// </summary>
internal static class Engine
{
    public static MainGame? GameScreen;
        
    // Null override because it's initialized vid new-game/load game
    public static RogueLikeEntity Player = null!;

    private static void Main()
    {
        Game.Create(Constants.ScreenWidth, Constants.ScreenHeight, "Fonts/Cheepicus12.font");
        Game.Instance.OnStart = Init;
        Game.Instance.Run();
        Game.Instance.Dispose();
    }

    private static void Init()
    {
        // Main menu
        GameHost.Instance.Screen = new MainMenu();

        // Destroy the default starting console that SadConsole created automatically because we're not using it.
        GameHost.Instance.DestroyDefaultStartingConsole();
    }

    /// <summary>
    /// Causes the entity specified to move in the specified direction if it can, or generate a "bump" action in that direction if a move is not possible.
    /// </summary>
    /// <remarks>
    /// If the entity cannot move, any map objects which have components implementing IBumpable will get their "Bump" function called, with the given
    /// entity being used as the source.  All components on all entities at the position being bumped will have their "Bump" function called
    /// in sequence, until one of them returns "true".
    /// </remarks>
    /// <param name="entity">The entity performing the bump.</param>
    /// <param name="direction">The direction of the adjacent square the specified entity is moving/bumping in.</param>
    /// <returns>True if either the entity moves, or some component's "Bump" function returned true; false otherwise.</returns>
    public static bool MoveOrBump(RogueLikeEntity entity, Direction direction)
    {
        if (entity.CurrentMap == null) return false;

        // Move if nothing blocks
        if (entity.CanMoveIn(direction))
        {
            entity.Position += direction;
            return true;
        }

        // Bump anything bumpable
        var newPosition = entity.Position + direction;
        foreach (var obj in entity.CurrentMap.GetObjectsAt(newPosition))
            foreach (var bumpable in obj.GoRogueComponents.GetAll<IBumpable>())
                if (bumpable.OnBumped(entity))
                    return true;

        if (entity == Player)
            GameScreen?.MessageLog.AddMessage(new("That way is blocked.", MessageColors.ImpossibleActionAppearance));

        return false;
    }

    /// <summary>
    /// Causes all objects with a HostileAI component to take their turns.  In this simple example, we don't really need a full turn system, so this
    /// is sufficient.
    /// </summary>
    /// <param name="map">Map to search for enemies on.</param>
    public static void TakeEnemyTurns(Map map)
    {
        var enemies = map.Entities.GetLayer((int)GameMap.Layer.Monsters).Items.ToArray();
        foreach (var enemy in enemies)
        {
            var ai = enemy.GoRogueComponents.GetFirstOrDefault<HostileAI>();
            ai?.TakeTurn();
        }
    }

    /// <summary>
    /// Called when the player dies.
    /// </summary>
    public static void PlayerDeath(object? s, EventArgs e)
    {
        // Go back to main menu for now
        Game.Instance.Screen = new MainMenu();

    }

    /// <summary>
    /// Called when enemies die.
    /// </summary>
    public static void HostileDeath(object? s, EventArgs e)
    {
        var hostile = ((Combatant)s!).Parent!;
        GameScreen?.MessageLog.AddMessage(new ColoredString($"The {hostile.Name} dies!", MessageColors.EnemyDiedAppearance));

        // Switch entity for corpse
        var map = hostile.CurrentMap!;
        map.RemoveEntity(hostile);
        map.AddEntity(Factory.Corpse(hostile));
    }
}