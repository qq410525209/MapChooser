using MapChooser.Models;
using MapChooser.Dependencies;
using MapChooser.Helpers;
using MapChooser.Menu;
using SwiftlyS2.Core.Menus.OptionsBase;

using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using System.Threading.Tasks;

namespace MapChooser.Helpers;

public class EndOfMapVoteManager
{
    private readonly ISwiftlyCore _core;
    private readonly PluginState _state;
    private readonly VoteManager _voteManager;
    private readonly MapLister _mapLister;
    private readonly MapCooldown _mapCooldown;
    private readonly ChangeMapManager _changeMapManager;
    private readonly MapChooserConfig _config;

    private Dictionary<string, int> _votes = new();
    private Dictionary<int, string> _playerVotes = new();
    private List<string> _mapsInVote = new();
    private bool _voteActive = false;
    private bool _changeImmediately = false;
    private DateTime _voteEndTime;
    private readonly HashSet<int> _playersReceivedMenu = new();

    public EndOfMapVoteManager(ISwiftlyCore core, PluginState state, VoteManager voteManager, MapLister mapLister, MapCooldown mapCooldown, ChangeMapManager changeMapManager, MapChooserConfig config)
    {
        _core = core;
        _state = state;
        _voteManager = voteManager;
        _mapLister = mapLister;
        _mapCooldown = mapCooldown;
        _changeMapManager = changeMapManager;
        _config = config;
    }

    public void StartVote(int voteDuration, int mapsToShow, bool changeImmediately = false)
    {
        if (_voteActive) return;

        _voteActive = true;
        _changeImmediately = changeImmediately;
        _state.EofVoteHappening = true;
        _votes.Clear();
        _playerVotes.Clear();
        _playersReceivedMenu.Clear();

        // Select maps for vote
        var allMaps = _mapLister.Maps.Select(m => m.Name).ToList();
        var random = new Random();
        _mapsInVote = allMaps.OrderBy(x => random.Next()).Take(mapsToShow).ToList();

        foreach (var map in _mapsInVote)
            _votes[map] = 0;

        _core.PlayerManager.SendChat(_core.Localizer["map_chooser.prefix"] + " " + _core.Localizer["map_chooser.vote.started"]);

        _voteEndTime = DateTime.Now.AddSeconds(voteDuration);

        // Show menu to all players and start timer
        RunVoteTimer();
        RefreshVoteMenu(true);
    }

    private void RunVoteTimer()
    {
        if (!_voteActive) return;

        int timeRemaining = (int)Math.Max(0, Math.Ceiling((_voteEndTime - DateTime.Now).TotalSeconds));
        
        if (timeRemaining <= 0)
        {
            EndVote();
            return;
        }

        RefreshVoteMenu();

        _core.Scheduler.DelayBySeconds(1, RunVoteTimer);
    }

    private void RefreshVoteMenu(bool forceOpen = false)
    {
        if (!_voteActive) return;

        int timeRemaining = (int)Math.Max(0, Math.Ceiling((_voteEndTime - DateTime.Now).TotalSeconds));
        foreach (var player in _core.PlayerManager.GetAllPlayers().Where(p => p.IsValid))
        {
            var currentMenu = _core.MenusAPI.GetCurrentMenu(player);
            bool hasEofMenuOpen = currentMenu?.Tag?.ToString() == "EofVoteMenu";

            if (_playerVotes.ContainsKey(player.Slot))
            {
                // If they already voted, only refresh if they still have the menu open
                if (hasEofMenuOpen)
                {
                    OpenVoteMenu(player, timeRemaining);
                }
                continue;
            }

            if (forceOpen || !_playersReceivedMenu.Contains(player.Slot))
            {
                // First time opening for this player or forced open
                OpenVoteMenu(player, timeRemaining);
                _playersReceivedMenu.Add(player.Slot);
            }
            else if (hasEofMenuOpen)
            {
                // If they haven't voted but have the menu open, refresh it
                OpenVoteMenu(player, timeRemaining);
            }
        }
    }
    
    // Kept for backward compatibility if needed, but not used internally now
    public void OpenVoteMenu(IPlayer player)
    {
        if (!_voteActive) return;
        _playersReceivedMenu.Add(player.Slot); // Mark as received so it starts refreshing
        int timeRemaining = (int)Math.Max(0, Math.Ceiling((_voteEndTime - DateTime.Now).TotalSeconds));
        OpenVoteMenu(player, timeRemaining);
    }

    public void OpenVoteMenu(IPlayer player, int timeRemaining)
    {
        if (!_voteActive) return;
        var menu = new EndOfMapVoteMenu(_core, _mapCooldown);
        menu.Show(player, _mapsInVote, _votes, timeRemaining, RegisterVote);
    }

    private void RegisterVote(IPlayer player, string map)
    {
        if (!_voteActive) return;

        int slot = player.Slot;
        if (_playerVotes.ContainsKey(slot))
        {
            _votes[_playerVotes[slot]]--;
        }

        _playerVotes[slot] = map;
        _votes[map]++;

        var localizer = _core.Translation.GetPlayerLocalizer(player);
        player.SendChat(localizer["map_chooser.prefix"] + " " + localizer["map_chooser.vote.you_voted", map]);
        
        // Refresh menu for everyone to show new counts
        RefreshVoteMenu();
    }

    private void EndVote()
    {
        if (!_voteActive) return;
        _voteActive = false;
        _state.EofVoteHappening = false;

        // Check and close menus for players
        foreach (var player in _core.PlayerManager.GetAllPlayers().Where(p => p.IsValid))
        {
            var menu = _core.MenusAPI.GetCurrentMenu(player);
            if (menu?.Tag?.ToString() == "EofVoteMenu")
            {
                _core.MenusAPI.CloseMenuForPlayer(player, menu);
            }
        }

        if (_votes.Count == 0)
        {
            return;
        }

        string winner = _votes.OrderByDescending(x => x.Value).FirstOrDefault().Key;
        if (string.IsNullOrEmpty(winner))
        {
            // Fallback to a random map from the vote list if somehow FirstOrDefault failed
            winner = _mapsInVote.OrderBy(_ => Guid.NewGuid()).FirstOrDefault() ?? "";
        }

        if (string.IsNullOrEmpty(winner))
        {
            return;
        }

        _core.PlayerManager.SendChat(_core.Localizer["map_chooser.prefix"] + " " + _core.Localizer["map_chooser.vote.ended", winner, _votes.GetValueOrDefault(winner, 0)]);

        _changeMapManager.ScheduleMapChange(winner, _changeImmediately);
    }
}
