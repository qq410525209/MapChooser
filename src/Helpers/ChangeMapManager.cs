using MapChooser.Models;
using MapChooser.Dependencies;
using MapChooser.Helpers;
using SwiftlyS2.Shared;

namespace MapChooser.Helpers;

public class ChangeMapManager
{
    private readonly ISwiftlyCore _core;
    private readonly PluginState _state;
    private readonly MapLister _mapLister;

    public ChangeMapManager(ISwiftlyCore core, PluginState state, MapLister mapLister)
    {
        _core = core;
        _state = state;
        _mapLister = mapLister;
    }

    public void ScheduleMapChange(string mapName, bool changeImmediately = false)
    {
        _state.NextMap = mapName;
        _state.MapChangeScheduled = true;
        _state.ChangeMapImmediately = changeImmediately;

        if (changeImmediately)
        {
            ChangeMap();
        }
        else
        {
            _core.PlayerManager.SendChat(_core.Localizer["map_chooser.prefix"] + " " + _core.Localizer["map_chooser.next_map_announced", mapName]);
        }
    }

    public void ChangeMap()
    {
        if (string.IsNullOrEmpty(_state.NextMap)) return;

        _state.MapChangeScheduled = false;

        var map = _mapLister.Maps.FirstOrDefault(m => m.Name.Equals(_state.NextMap, StringComparison.OrdinalIgnoreCase));
        if (map == null) return;

        _core.PlayerManager.SendChat(_core.Localizer["map_chooser.prefix"] + " " + _core.Localizer["map_chooser.changing_map", map.Name]);

        _core.Scheduler.DelayBySeconds(3, () => {
            if (!string.IsNullOrEmpty(map.Id) && (map.Id.StartsWith("ws:") || long.TryParse(map.Id, out _)))
            {
                string workshopId = map.Id.StartsWith("ws:") ? map.Id.Substring(3) : map.Id;
                _core.Engine.ExecuteCommandWithBuffer($"nextlevel {map.Name}", _ => { });
                _core.Engine.ExecuteCommandWithBuffer($"host_workshop_map {workshopId}", _ => { });
            }
            else
            {
                _core.Engine.ExecuteCommandWithBuffer($"nextlevel {map.Name}", _ => { });
                _core.Engine.ExecuteCommandWithBuffer($"changelevel {map.Id ?? map.Name}", _ => { });
            }
        });
    }
}
