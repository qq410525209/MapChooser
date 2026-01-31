using MapChooser.Models;
using MapChooser.Helpers;
using MapChooser.Dependencies;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.Players;

namespace MapChooser.Commands;

public class RevoteCommand
{
    private readonly ISwiftlyCore _core;
    private readonly PluginState _state;
    private readonly EndOfMapVoteManager _eofManager;

    public RevoteCommand(ISwiftlyCore core, PluginState state, EndOfMapVoteManager eofManager)
    {
        _core = core;
        _state = state;
        _eofManager = eofManager;
    }

    public void Execute(ICommandContext context)
    {
        if (!context.IsSentByPlayer) return;

        var player = context.Sender!;
        var localizer = _core.Translation.GetPlayerLocalizer(player);

        if (!_state.EofVoteHappening)
        {
            player.SendChat(localizer["map_chooser.prefix"] + " " + localizer["map_chooser.revote.no_vote_active"]);
            return;
        }

        _eofManager.OpenVoteMenu(player);
    }
}
