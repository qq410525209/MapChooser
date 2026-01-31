using MapChooser.Models;
using MapChooser.Dependencies;
using MapChooser.Helpers;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.Players;

namespace MapChooser.Commands;

public class NextmapCommand
{
    private readonly ISwiftlyCore _core;
    private readonly PluginState _state;

    public NextmapCommand(ISwiftlyCore core, PluginState state)
    {
        _core = core;
        _state = state;
    }

    public void Execute(ICommandContext context)
    {
        if (!context.IsSentByPlayer) return;
        var player = context.Sender!;
        var localizer = _core.Translation.GetPlayerLocalizer(player);

        if (string.IsNullOrEmpty(_state.NextMap))
        {
            player.SendChat(localizer["map_chooser.prefix"] + " " + localizer["map_chooser.next_map_not_decided"]);
        }
        else
        {
            player.SendChat(localizer["map_chooser.prefix"] + " " + localizer["map_chooser.next_map_decided", _state.NextMap]);
        }
    }
}
