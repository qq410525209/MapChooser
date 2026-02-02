using MapChooser.Models;
using SwiftlyS2.Shared;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MapChooser.Helpers;

public class MapCooldown
{
    private List<string> _mapsOnCooldown = new();
    private readonly MapChooserConfig _config;
    private readonly ISwiftlyCore _core;
    private string? _savePath;

    public MapCooldown(ISwiftlyCore core, MapChooserConfig config)
    {
        _core = core;
        _config = config;
        
        _savePath = Path.Combine(_core.PluginDataDirectory, "map_history.json");
        LoadHistory();
    }

    public void OnMapStart(string mapName, string? workshopId = null)
    {
        if (_config.MapsInCooldown <= 0)
        {
            _mapsOnCooldown.Clear();
            SaveHistory();
            return;
        }

        string identity = (!string.IsNullOrEmpty(workshopId) ? workshopId : mapName).Trim().ToLower();
        
        if (_mapsOnCooldown.Contains(identity))
            _mapsOnCooldown.Remove(identity);

        _mapsOnCooldown.Add(identity);

        int limit = _config.MapsInCooldown + 1;
        while (_mapsOnCooldown.Count > limit)
        {
            _mapsOnCooldown.RemoveAt(0);
        }
        
        SaveHistory();
    }

    public bool IsMapInCooldown(string mapIdentity)
    {
        string identity = mapIdentity.Trim().ToLower();
        
        // Find if this identity exists in history
        if (!_mapsOnCooldown.Contains(identity)) return false;

        // Verify it's not the current map
        if (_core.Engine == null) return false;

        var currentMapName = _core.Engine.GlobalVars.MapName.ToString().ToLower();
        var currentWorkshopId = _core.Engine.WorkshopId.ToLower();

        if (identity == currentMapName || (!string.IsNullOrEmpty(currentWorkshopId) && identity == currentWorkshopId))
            return false;

        return true;
    }

    public bool IsMapInCooldown(Map map)
    {
        if (map.Id != null && IsMapInCooldown(map.Id)) return true;
        if (IsMapInCooldown(map.Name)) return true;
        return false;
    }

    public void AddMapToCooldown(string mapIdentity)
    {
        string identity = mapIdentity.Trim().ToLower();
        if (!_mapsOnCooldown.Contains(identity))
        {
            _mapsOnCooldown.Add(identity);
            int limit = _config.MapsInCooldown + 1;
            while (_mapsOnCooldown.Count > limit && limit > 0)
            {
                _mapsOnCooldown.RemoveAt(0);
            }
            SaveHistory();
        }
    }

    private void LoadHistory()
    {
        if (string.IsNullOrEmpty(_savePath) || !File.Exists(_savePath)) return;

        try
        {
            string json = File.ReadAllText(_savePath);
            _mapsOnCooldown = JsonSerializer.Deserialize<List<string>>(json) ?? new();
        }
        catch (Exception ex)
        {
            _core.Logger.LogError(ex, "Failed to load map history.");
        }
    }

    private void SaveHistory()
    {
        if (string.IsNullOrEmpty(_savePath)) return;

        try
        {
            string json = JsonSerializer.Serialize(_mapsOnCooldown);
            File.WriteAllText(_savePath, json);
        }
        catch (Exception ex)
        {
            _core.Logger.LogError(ex, "Failed to save map history.");
        }
    }
}

