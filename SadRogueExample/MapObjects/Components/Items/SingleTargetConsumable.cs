using System;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogueExample.Screens;
using SadRogueExample.Themes;

namespace SadRogueExample.MapObjects.Components.Items;
internal abstract class SingleTargetConsumable : RogueLikeComponentBase<RogueLikeEntity>, IConsumable
{
    enum State
    {
        Targeting, // Awaiting target selection.
        Consuming, // Consuming the item.
    }

    private readonly State _originalState;
    private State _currentState;
    private RogueLikeEntity? _target;
    private readonly bool _allowTargetingSelf;
    private readonly bool _allowTargetingNonVisible;

    public SingleTargetConsumable(bool allowTargetSelf, bool allowTargetingNonVisible)
        : base(false, false, false, false)
    {
        _currentState = _originalState = State.Targeting;
        _allowTargetingSelf = allowTargetSelf;
        _allowTargetingNonVisible = allowTargetingNonVisible;
    }

    public SingleTargetConsumable(RogueLikeEntity target)
        : base(false, false, false, false)
    {
        _target = target;
        _currentState = _originalState = State.Consuming;
    }

    // TODO: Refactor this to have some sort of Activate function that differs from Consume?  Use Effects instead?  More flexible targeting.
    public bool Consume(RogueLikeEntity consumer)
    {
        if (consumer != Engine.Player && _currentState == State.Targeting)
            throw new InvalidOperationException("A {nameof(SingleTargetConsumable)} being used by a non-player _must_ have a target set when consumed.");

        if (Engine.GameScreen == null)
            throw new InvalidOperationException("Cannot consume a targeted item without a game screen.");

        switch (_currentState)
        {
            case State.Targeting: // We know it is the player using the item
                _currentState = State.Consuming;
                // TODO: Color
                Engine.GameScreen.MessageLog.AddMessage(new("Select an enemy to target.", MessageColors.EnemyAtkAppearance));

                // Create the targeting component and set the state of the game screen so we use it.  The component will call this item's Consume()
                // again once a target is selected.
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
                    _target = null;
                    Engine.GameScreen.SetState(MainGame.State.MainMap);
                    _currentState = State.Targeting;
                };

                Engine.GameScreen.SetState(MainGame.State.TargetEntityMode, targeter);
                return false; // We didn't finish consuming, so we'll return false here.
            case State.Consuming:
                // In any case we're going to try to use the item, so we'll set the state back to Targeting in case it fails
                // and we use it again later.
                var target = _target;
                if (_originalState == State.Targeting)
                {
                    _currentState = State.Targeting;
                    _target = null;
                }

                // TODO: This validation really could go elsewhere; the targeter shouldn't have accepted invalid targets.
                if (target == null)
                {
                    if (_originalState == State.Targeting)
                    {
                        Engine.GameScreen.MessageLog.AddMessage(
                            new("You must select an enemy to target.", MessageColors.ImpossibleActionAppearance));
                        return false;
                    }
                    
                    throw new InvalidOperationException($"A {nameof(SingleTargetConsumable)} being used by a non-player _must_ have a target set when consumed.");
                }

                if (!_allowTargetingSelf && target == consumer)
                {
                    if (_originalState == State.Targeting)
                    {
                        Engine.GameScreen.MessageLog.AddMessage(
                            new("You cannot target yourself.", MessageColors.ImpossibleActionAppearance));
                        return false;
                    }

                    throw new InvalidOperationException($"A {nameof(SingleTargetConsumable)} was consumed by a non-player, and tried to illegally target itself.");
                }

                if (!_allowTargetingNonVisible && !Engine.GameScreen.Map.PlayerFOV.BooleanResultView[target.Position])
                {
                    if (_originalState == State.Targeting)
                    {
                        Engine.GameScreen.MessageLog.AddMessage(
                            new("You cannot target an area that you cannot see.", MessageColors.ImpossibleActionAppearance));
                        return false;
                    }

                    throw new InvalidOperationException($"A {nameof(SingleTargetConsumable)} was consumed by a non-player, and tried to illegally target a non-visible area.");
                }

                return OnUse(target);
        }

        throw new InvalidOperationException("Invalid state.");
    }

    protected abstract bool OnUse(RogueLikeEntity target);

}
