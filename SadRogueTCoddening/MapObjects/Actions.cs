using System;
using System.Linq;
using GoRogue.GameFramework;
using SadRogue.Integration;
using SadRogue.Primitives;
using SadRogueTCoddening.MapObjects.Components;
using SadRogueTCoddening.Maps;
using SadRogueTCoddening.Screens;

namespace SadRogueTCoddening.MapObjects;

internal static class Actions
{
    public static bool Bump(RogueLikeEntity entity, Direction direction)
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


        return false;
    }

    public static void PlayerDeath(object? s, EventArgs e)
    {
        // Go back to main menu for now
        SadConsole.Game.Instance.Screen = new MainMenu();
        
    }

    // Creates a corpse of the given object
    public static void HostileDeath(object? s, EventArgs e)
    {
        var hostile = ((Combatant)s!).Parent!;
        Engine.GameScreen?.MessageLog.AddMessage($"The {hostile.Name} dies!");
        
        // Switch entity for corpse
        var map = hostile.CurrentMap!;
        map.RemoveEntity(hostile);
        map.AddEntity(Factory.Corpse(hostile));
    }

    public static void TakeEnemyTurns(Map map)
    {
        var enemies = map.Entities.GetLayer((int)GameMap.Layer.Monsters).Items.ToArray();
        foreach (var enemy in enemies)
        {
            var ai = enemy.GoRogueComponents.GetFirstOrDefault<HostileAI>();
            ai?.TakeTurn();
        }
    }
}