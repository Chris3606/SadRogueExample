using SadRogue.Integration.Components;
using SadRogue.Integration;
using System;
using SadConsole.Components;
using SadRogue.Primitives;
using SadRogueExample.Screens.MainGameStates;

namespace SadRogueExample.MapObjects.Components.Items;
internal abstract class AreaTargetConsumable : RogueLikeComponentBase<RogueLikeEntity>, IConsumable
{
    // Private for now because the player is the only one who can use items so there is no need to set it outside the class, but this could change.
    private Point? _target;

    public readonly int Radius;

    private readonly Radius? _radiusShape;

    public Radius RadiusShape => _radiusShape ?? Engine.GameScreen!.Map.DistanceMeasurement;
    public readonly bool AllowTargetNonVisible;

    protected AreaTargetConsumable(int radius, Radius? radiusShape, bool allowTargetNonVisible)
        : base(false, false, false, false)
    {
        Radius = radius;
        _radiusShape = radiusShape;
        AllowTargetNonVisible = allowTargetNonVisible;
    }

    public IComponent? GetStateHandler(RogueLikeEntity consumer)
    {
        if (_target != null) return null;

        if (Engine.GameScreen == null)
            throw new InvalidOperationException("Cannot target an item without a game screen.");

        var targetingState = new TargetAreaState(Engine.GameScreen, Radius, RadiusShape, AllowTargetNonVisible,
            positionSelected: pos =>
            {
                _target = pos.MapPosition;
                Engine.GameScreen.CurrentState = new MainMapState(Engine.GameScreen);
                PlayerActionHelper.PlayerTakeAction(entity => entity.AllComponents.GetFirst<Inventory>().Consume(Parent!));
            });
        return targetingState;
    }

    // TODO: Effects?
    public bool Consume(RogueLikeEntity consumer)
    {
        if (_target == null)
            throw new InvalidOperationException($"A {nameof(AreaTargetConsumable)} must have a target set when consumed.");


        // In any case we're going to try to use the item, so we'll remove the target in case it fails
        // and we use it again later.
        var target = _target;
        _target = null;

        return OnUse(target.Value);
    }

    protected abstract bool OnUse(Point target);
}