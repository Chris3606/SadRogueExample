using SadConsole;
using SadConsole.UI.Controls;
using SadRogueExample.MapObjects.Components;
using System;

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
            var keybindings = Parent.GetSadComponent<CustomKeybindingsComponent>();
            if (keybindings != null)
                Parent.SadComponents.Remove(keybindings);

            // Don't allow the player to close the menu
            CloseOnEscKey = false;

            // Print text
            PrintTextAtCenter("You have died.", y: 2);

            // Place buttons for going to the main menu or exiting the game
            var mainMenuButton = new Button(13, 1)
            {
                Text = "Main Menu",
                Position = (2, 4),
            };
            mainMenuButton.Click += MainMenuOnClick;

            var exitButton = new Button(11, 1)
            {
                Text = "Exit",
                Position = (16, 4),
            };
            exitButton.Click += ExitOnClick;

            Controls.Add(mainMenuButton);
            Controls.Add(exitButton);
        }

        private void MainMenuOnClick(object? sender, EventArgs e)
        {
            Hide();

            Engine.GameScreen = null;
            
            GameHost.Instance.Screen = new MainMenu();
        }

        private void ExitOnClick(object? sender, EventArgs e)
        {
            Game.Instance.MonoGameInstance.Exit();
        }
    }
}
