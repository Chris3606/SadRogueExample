using SadConsole;
using SadConsole.Input;
using SadRogueExample.MapObjects.Components;
using SadRogueExample.Screens.Components;
using SadRogueExample.Screens.MainGameMenus;

namespace SadRogueExample.Screens.MainGameStates
{
    /// <summary>
    /// "Main" state where the map is displayed and the player can move around.
    /// </summary>
    internal class MainMapState : StateBase
    {
        private readonly MainMapKeybindingsComponent _keybindings;

        public MainMapState(MainGame gameScreen)
            : base(gameScreen, false, false, false, false)
        {
            _keybindings = new MainMapKeybindingsComponent();
            // Add controls for picking up items and getting to inventory screen.
            _keybindings.SetAction(Keys.G, () => PlayerActionHelper.PlayerTakeAction(e => e.AllComponents.GetFirst<Inventory>().PickUp()));

            // Controls for menus
            _keybindings.SetAction(Keys.C, () => GameScreen.Children.Add(new ConsumableSelect()));
            _keybindings.SetAction(Keys.M, () => GameScreen.Children.Add(new MessageLogMenu(50, 24, 1000)));

            // "Look" functionality Keybinding
            _keybindings.SetAction(Keys.OemQuestion, () => GameScreen.CurrentState = new SelectMapLocationState(GameScreen));
        }

        public override void OnAdded(IScreenObject host)
        {
            base.OnAdded(host);
            host.SadComponents.Add(_keybindings);
        }

        public override void OnRemoved(IScreenObject host)
        {
            base.OnRemoved(host);
            host.SadComponents.Remove(_keybindings);
        }
    }
}
