﻿using SadConsole.Input;
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
            var player = new RogueLikeEntity('@', false, layer: (int)GameMap.Layer.Monsters)
            {
                Name = "Player"
            };

            // Add component for controlling player movement via keyboard.
            var motionControl = new CustomPlayerKeybindingsComponent();
            motionControl.SetMotions(PlayerKeybindingsComponent.ArrowMotions);
            motionControl.SetMotions(PlayerKeybindingsComponent.NumPadAllMotions);
            motionControl.SetMotion(Keys.NumPad5, Direction.None);
            motionControl.SetMotion(Keys.OemPeriod, Direction.None);
            player.AllComponents.Add(motionControl);

            // Add component for updating map's player FOV as they move
            player.AllComponents.Add(new PlayerFOVController());
            
            // Player combatant
            var combatant = new Combatant(30, 2, 5);
            combatant.Died += Actions.PlayerDeath;
            player.AllComponents.Add(combatant);

            return player;
        }

        public static RogueLikeEntity Orc()
        {
            var enemy = new RogueLikeEntity(new Color(63, 127, 63), 'o', false, layer: (int)GameMap.Layer.Monsters)
            {
                Name = "Orc"
            };
            
            // Add AI component to bump action toward the player if the player is in view
            enemy.AllComponents.Add(new HostileAI());

            var combatant = new Combatant(10, 0, 3);
            combatant.Died += Actions.HostileDeath;
            enemy.AllComponents.Add(combatant);
            
            return enemy;
        }
        
        public static RogueLikeEntity Troll()
        {
            var enemy = new RogueLikeEntity(new Color(0, 127, 0), 'T', false, layer: (int)GameMap.Layer.Monsters)
            {
                Name = "Troll"
            };
            
            enemy.AllComponents.Add(new HostileAI());

            var combatant = new Combatant(16, 1, 4);
            combatant.Died += Actions.HostileDeath;
            enemy.AllComponents.Add(combatant);

            return enemy;
        }

        public static RogueLikeEntity Corpse(RogueLikeEntity entity)
        {
            var corpse = new RogueLikeEntity(entity.Appearance, layer: (int)GameMap.Layer.Items)
            {
                Name = $"Corpse - {entity.Name}",
                Position = entity.Position
            };
            corpse.Appearance.Glyph = '%';

            return corpse;
        }
    }
}
