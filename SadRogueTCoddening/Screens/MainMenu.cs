using System;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogueTCoddening.MapObjects.Components;

namespace SadRogueTCoddening.Screens;

/// <summary>
/// The main menu screen.
/// </summary>
public class MainMenu : ControlsConsole
{
    public MainMenu()
        : base(15, 3)
    {
        // Position controls console
        Position = (Constants.ScreenWidth / 2 - Width / 2, Constants.ScreenHeight / 2 - Height / 2);
        
        // Add buttons
        var newGame = new Button(Width)
        {
            Name = "NewGameBtn",
            Text = "New Game",
            Position = (0, 0)
        };
        newGame.Click += NewGameOnClick;
        Controls.Add(newGame);
        
        var loadGame = new Button(Width)
        {
            Name = "LoadGameBtn",
            Text = "Load Game",
            Position = (0, 1)
        };
        loadGame.Click += LoadGameOnClick;
        Controls.Add(newGame);

        var exit = new Button(Width)
        {
            Name = "ExitBtn",
            Text = "Exit",
            Position = (0, 2)
        };
        exit.Click += ExitOnClick;
        Controls.Add(exit);
    }

    private void NewGameOnClick(object? sender, EventArgs e)
    {
        // Create player entity
        Engine.Player = MapObjects.Factory.Player();
        
        // Generate a dungeon map, and spawn the player/enemies
        var map = Maps.Factory.Dungeon();
        
        // Calculate initial FOV for player
        Engine.Player.AllComponents.GetFirst<PlayerFOVController>().CalculateFOV();

        // Create a MapScreen and set it as the active screen so that it processes input and renders itself.
        Engine.GameScreen = new MainGame(map);
        GameHost.Instance.Screen = Engine.GameScreen;
    }
    
    private void LoadGameOnClick(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
    
    private void ExitOnClick(object? sender, EventArgs e)
    {
        Game.Instance.MonoGameInstance.Exit();
    }
}