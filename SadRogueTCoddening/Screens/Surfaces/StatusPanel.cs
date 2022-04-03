using System;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using SadRogueTCoddening.MapObjects.Components;

namespace SadRogueTCoddening.Screens.Surfaces;

internal class StatusPanel : ControlsConsole
{
    public readonly ProgressBar HPBar;

    public StatusPanel(int width, int height)
        : base(width, height)
    {
        HPBar = new ProgressBar(Width, 1, HorizontalAlignment.Left)
        {
            DisplayTextColor = Color.White,
            Theme = Themes.StatusPanel.HPBarTheme
        };
        Controls.Add(HPBar);
        Engine.Player.AllComponents.GetFirst<Combatant>().HPChanged += OnPlayerHPChanged;
        UpdateHPBar();
    }

    private void OnPlayerHPChanged(object? sender, EventArgs e)
    {
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        var combatant = Engine.Player.AllComponents.GetFirst<Combatant>();
        HPBar.DisplayText = $"HP: {combatant.HP} / {combatant.MaxHP}";
        HPBar.Progress = (float)combatant.HP / combatant.MaxHP;
    }
}