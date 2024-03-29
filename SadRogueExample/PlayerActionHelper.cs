﻿using System;
using SadRogue.Integration;
using SadRogueExample.MapObjects.Components;
using SadRogueExample.Maps;

namespace SadRogueExample;

/// <summary>
/// Helper functions for having the player take arbitrary actions, which will end the player's turn if they succeed.
/// </summary>
/// <remarks>
/// Since this game is relatively simple, it is no need for a complex turn manager; the functions in this class more or less stand in for that role.
/// </remarks>
internal static class PlayerActionHelper
{
    /// <summary>
    /// Causes the player to attempt to take the given action as a turn.  If the action succeeds, the player's turn will end and enemy's turns will
    /// take place.
    /// </summary>
    /// <param name="action">Action to be performed.  Takes the entity performing the action (the player in this case) as a parameter.</param>
    public static void PlayerTakeAction(Func<RogueLikeEntity, bool> action) => PlayerTakeAction((entity, _) => action(entity), false);

    /// <summary>
    /// Causes the player to attempt to take the given action as a turn.  If the action succeeds, the player's turn will end and enemy's turns will
    /// take place.
    /// </summary>
    /// <typeparam name="T">Type of parameters for the action function.</typeparam>
    /// <param name="action">Action to be performed.  Takes the entity performing the action (the player in this case) and an arbitrary parameters object as parameters.</param>
    /// <param name="performParams">Parameters to pass to the action function.</param>
    public static void PlayerTakeAction<T>(Func<RogueLikeEntity, T, bool> action, T performParams)
    {
        var player = Engine.Player;

        // If the action failed, then we won't take up a turn.
        if (!action(player, performParams))
            return;

        // The player completed their turn by successfully taking an action; but if they somehow died in the process, we'll just return
        // because the main game screen's death handlers will have already dealt with the situation.
        if (player.AllComponents.GetFirst<Combatant>().HP <= 0) return;

        // Otherwise, have the enemies take their turns.
        (player.CurrentMap as GameMap)!.TakeEnemyTurns();
    }
}