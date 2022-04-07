using System;
using System.Linq;
using GoRogue.GameFramework;
using SadConsole;
using SadRogue.Integration;
using SadRogue.Primitives;
using SadRogueTCoddening.MapObjects.Components;
using SadRogueTCoddening.MapObjects.Components.Items;
using SadRogueTCoddening.Maps;
using SadRogueTCoddening.Screens;
using SadRogueTCoddening.Themes;

namespace SadRogueTCoddening;

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

    public static void PickUpItem(RogueLikeEntity entity)
    {
        if (entity.CurrentMap == null)
            throw new ArgumentException("Entity must be part of a map to pick up items.", nameof(entity));
        
        var inventory = entity.AllComponents.GetFirst<Inventory>();
        foreach (var item in entity.CurrentMap.GetEntitiesAt<RogueLikeEntity>(entity.Position))
        {
            if (!item.GoRogueComponents.Contains<ICarryable>()) continue;

            if (inventory.Items.Count >= inventory.Capacity)
            {
                // TODO: Color
                Engine.GameScreen?.MessageLog.AddMessage(new("Your inventory is full."));
                return;
            }
            
            item.CurrentMap!.RemoveEntity(item);
            inventory.Items.Add(item);
            
            // TODO: Pick a color
            Engine.GameScreen?.MessageLog.AddMessage(new($"You picked up the {item.Name}."));
            TakeEnemyTurns(Engine.Player.CurrentMap!);
            return;
        }
        
        Engine.GameScreen?.MessageLog.AddMessage(new("There is nothing here to pick up.", MessageColors.ImpossibleActionAppearance));
    }

    public static void PlayerDeath(object? s, EventArgs e)
    {
        // Go back to main menu for now
        Game.Instance.Screen = new MainMenu();
        
    }

    // Creates a corpse of the given object
    public static void HostileDeath(object? s, EventArgs e)
    {
        var hostile = ((Combatant)s!).Parent!;
        Engine.GameScreen?.MessageLog.AddMessage(new ColoredString($"The {hostile.Name} dies!", MessageColors.EnemyDiedAppearance));
        
        // Switch entity for corpse
        var map = hostile.CurrentMap!;
        map.RemoveEntity(hostile);
        map.AddEntity(MapObjects.Factory.Corpse(hostile));
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