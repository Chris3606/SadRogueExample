using SadConsole;
using SadRogue.Integration.Components;

namespace SadRogueExample.Screens.MainGameStates;

/// <summary>
/// Component representing a state of the main game screen.
/// </summary>
internal class StateBase : RogueLikeComponentBase<IScreenSurface>
{
    protected readonly MainGame GameScreen;

    public StateBase(MainGame gameScreen, bool isUpdate, bool isRender, bool isMouse, bool isKeyboard)
        : base(isUpdate, isRender, isMouse, isKeyboard)
    {
        GameScreen = gameScreen;
    }
}
