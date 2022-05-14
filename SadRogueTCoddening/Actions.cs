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

/// <summary>
/// Static class with functions which has functions for various "actions" and event handlers related to state management.
/// </summary>
internal static class Actions
{
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


        return false;
    }

    /// <summary>
    /// Causes the given entity to pick up any items at its current position.  The given entity must have an inventory.
    /// </summary>
    /// <param name="entity">Entity which will pick up items.</param>
    /// <exception cref="ArgumentException"></exception>
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
                Engine.GameScreen?.MessageLog.AddMessage(new("Your inventory is full.", MessageColors.ImpossibleActionAppearance));
                return;
            }
            
            item.CurrentMap!.RemoveEntity(item);
            inventory.Items.Add(item);
            
            Engine.GameScreen?.MessageLog.AddMessage(new($"You picked up the {item.Name}.", MessageColors.ItemPickedUpAppearance));
            // TODO: Not great place for this
            TakeEnemyTurns(Engine.Player.CurrentMap!);
            return;
        }
        
        Engine.GameScreen?.MessageLog.AddMessage(new("There is nothing here to pick up.", MessageColors.ImpossibleActionAppearance));
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