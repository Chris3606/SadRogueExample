using SadConsole;
using SadConsole.UI.Controls;
using SadRogueExample.MapObjects.Components;

namespace SadRogueExample.Screens.MainGameMenus
{
    /// <summary>
    /// Menu that is displayed when the player dies.
    /// </summary>
    internal class GameOver : MainGameMenu
    {
        public GameOver()
            : base(29, 6)
        {
            Title = "Game Over!";

            // Remove the player's keyboard handler to ensure they can't take actions
            var keybindings = Engine.Player.AllComponents.GetFirstOrDefault<CustomPlayerKeybindingsComponent>();
            if (keybindings != null)
                Engine.Player.AllComponents.Remove(keybindings);

            // Don't allow the player to close the menu
            CloseOnEscKey = false;

            // Print text
            PrintTextAtCenter("You have died.", y: 2);

            // Place buttons for going to the main menu or exiting the game
            var mainMenuButton = new Button(11, 1)
            {
                Text = "Main Menu",
                Position = (2, 4),
            };
            mainMenuButton.Click += (_, _) =>
            {
                Engine.GameScreen = null;
                GameHost.Instance.Screen = new MainMenu();
            };

            var exitButton = new Button(11, 1)
            {
                Text = "Exit",
                Position = (16, 4),
            };
            exitButton.Click += (_, _) => Game.Instance.MonoGameInstance.Exit();

            Controls.Add(mainMenuButton);
            Controls.Add(exitButton);
        }
    }
}
