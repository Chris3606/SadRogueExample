using System;
using System.Collections.Generic;
using SadConsole;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using SadRogueExample.Themes;

namespace SadRogueExample.Screens.MainGameStates;
internal class TargetAreaState : SelectMapLocationState
{
    private readonly bool _allowTargetNonVisible;

    public TargetAreaState(MainGame gameScreen, int radius, Radius radiusShape, bool allowTargetNonVisible,
        Action<LookMarkerPosition>? positionChanged = null,
        Action<LookMarkerPosition>? positionSelected = null,
        Func<Point>? getLookMarkerSurfaceStartingLocation = null)
        : base(gameScreen, radius, radiusShape, positionChanged, positionSelected, getLookMarkerSurfaceStartingLocation)
    {
        _allowTargetNonVisible = allowTargetNonVisible;
    }

    protected override bool ValidateSelectedPosition()
    {
        if (!_allowTargetNonVisible && !GameScreen.Map.PlayerFOV.BooleanResultView[LookMarkerPosition.MapPosition])
        {
            GameScreen.MessageLog.AddMessage(
                               new("You cannot target an area that you cannot see.", MessageColors.ImpossibleActionAppearance));
            return false;
        }

        return true;
    }

    public IEnumerable<Point> PositionsSelected() //// TODO: Rad + 1?
        => RadiusShape.PositionsInRadius(LookMarkerPosition.MapPosition, Radius, GameScreen.Map.Bounds());

    public override void OnAdded(IScreenObject host)
    {
        base.OnAdded(host);

        // TODO: Color
        GameScreen.MessageLog.AddMessage(new("Select an area to target.", MessageColors.EnemyAtkAppearance));
    }
}
