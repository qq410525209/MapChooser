using MapChooser.Models;
using MapChooser.Dependencies;
using MapChooser.Helpers;
using SwiftlyS2.Core.Menus.OptionsBase;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using System.Threading.Tasks;

namespace MapChooser.Menu;

public class EndOfMapVoteMenu
{
    private readonly ISwiftlyCore _core;
    private readonly MapCooldown _mapCooldown;

    public EndOfMapVoteMenu(ISwiftlyCore core, MapCooldown mapCooldown)
    {
        _core = core;
        _mapCooldown = mapCooldown;
    }

    public void Show(IPlayer player, List<string> mapsInVote, Dictionary<string, int> votes, int timeRemaining, Action<IPlayer, string> onVote)
    {
        // specific logic to preserve cursor position
        var oldMenu = _core.MenusAPI.GetCurrentMenu(player);
        int? indexToRestore = null;
        if (oldMenu?.Tag?.ToString() == "EofVoteMenu")
        {
            indexToRestore = oldMenu.GetCurrentOptionIndex(player);
        }

        var localizer = _core.Translation.GetPlayerLocalizer(player);
        var builder = _core.MenusAPI.CreateBuilder();
        builder.Design.SetMenuTitle((localizer["map_chooser.vote.title"] ?? "Vote for the next map:") + $" <font color='red'>({timeRemaining}s)</font>");
        foreach (var map in mapsInVote)
        {
            int count = votes.ContainsKey(map) ? votes[map] : 0;
            string displayName = map;
            bool isExtend = map == "map_chooser.extend_option";
            if (isExtend)
            {
                displayName = localizer["map_chooser.extend_option"];
            }

            var option = new ButtonMenuOption($"{(isExtend ? "<font color='orange'>" : "<font color='lightgreen'>")}{displayName}</font> <font color='red'>[{count}]</font>");
            option.Enabled = isExtend || !_mapCooldown.IsMapInCooldown(map);
            option.Click += (sender, args) =>
            {
                _core.Scheduler.NextTick(() => {
                    onVote(args.Player, map);
                });
                return ValueTask.CompletedTask;
            };

            builder.AddOption(option);
        }

        var menu = builder.Build();
        menu.Tag = "EofVoteMenu";
        _core.MenusAPI.OpenMenuForPlayer(player, menu);

        if (indexToRestore.HasValue && indexToRestore.Value != -1)
        {
            // Restore functionality on next tick to ensure client processes the new menu first
            _core.Scheduler.NextTick(() => menu.MoveToOptionIndex(player, indexToRestore.Value));
        }
    }
}
