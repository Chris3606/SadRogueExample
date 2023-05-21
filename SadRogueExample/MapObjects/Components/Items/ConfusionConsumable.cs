using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;
using SadRogueExample.MapObjects.Components.AI;
using SadRogueExample.Screens;
using SadRogueExample.Themes;

namespace SadRogueExample.MapObjects.Components.Items;
internal class ConfusionConsumable : RogueLikeComponentBase<RogueLikeEntity>, IConsumable
{
    enum State
    {
        Targeting, // Awaiting target selection.
        Consuming, // Consuming the item.
    }

    public readonly int NumberOfTurns;
    private State _currentState;
    private RogueLikeEntity? _target;

    public ConfusionConsumable(int numberOfTurns)
        : base(false, false, false, false)
    {
        NumberOfTurns = numberOfTurns;
        _currentState = State.Targeting;
    }

    public bool Consume(RogueLikeEntity consumer)
    {
        if (Engine.GameScreen == null)
            throw new InvalidOperationException("Cannot consume an item without a game screen.");
        if (consumer != Engine.Player)
            throw new InvalidOperationException("Only the player can use confusion effects.");

        // For now, we'll just assume the player is the only one who can use this.
        switch (_currentState)
        {
            case State.Targeting:
                _currentState = State.Consuming;
                // TODO: Color
                Engine.GameScreen.MessageLog.AddMessage(new("Select a target for your confusion spell.", MessageColors.EnemyAtkAppearance));

                var targeter = new TargetEntityComponent(() => (0, 0), Engine.GameScreen.Map,
                    Engine.GameScreen.StatusPanel.LookInfo);
                targeter.PositionSelected += (_, _) =>
                {
                    _target = targeter.TargetEntity;
                    Engine.GameScreen.SetState(MainGame.State.MainMap);
                    Consume(consumer);
                };
                targeter.SelectionCancelled += (_, _) =>
                {
                    Engine.GameScreen.SetState(MainGame.State.MainMap);
                    _currentState = State.Targeting;
                };

                Engine.GameScreen.SetState(MainGame.State.TargetEntityMode, targeter);
                return false; // We didn't finish consuming it.
            case State.Consuming:
                // TODO: This validation logic should go elsewhere.
                if (_target == null)
                {
                    Engine.GameScreen.MessageLog.AddMessage(
                        new("You must select an enemy to target.", MessageColors.ImpossibleActionAppearance));
                    return false;
                }

                var target = _target;
                _target = null;

                if (!Engine.GameScreen.Map.PlayerFOV.BooleanResultView[target.Position])
                {
                    Engine.GameScreen.MessageLog.AddMessage(
                        new("You cannot target an area that you cannot see.",
                            MessageColors.ImpossibleActionAppearance));
                    return false;
                }

                if (target.Position == consumer.Position)
                {
                    Engine.GameScreen.MessageLog.AddMessage(
                        new("You cannot confuse yourself!",
                            MessageColors.ImpossibleActionAppearance));
                    return false;
                }

                var currentAI = target.AllComponents.GetFirstOrDefault<AIBase>();
                if (currentAI == null)
                {
                    Engine.GameScreen.MessageLog.AddMessage(
                                               new("You cannot confuse an inanimate object.",
                                                                              MessageColors.ImpossibleActionAppearance));
                    return false;
                }
                target.AllComponents.Remove(currentAI);
                // TODO: Color
                Engine.GameScreen!.MessageLog.AddMessage(new($"The eyes of the {target.Name} look vacant, as it starts to stumble around!", MessageColors.EnemyAtkAppearance));
                target.AllComponents.Add(new ConfusedAI(NumberOfTurns, currentAI));
                
                return true;
        }

        throw new InvalidOperationException("Invalid state.");
    }
}
