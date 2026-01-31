using MapChooser.Models;
using SwiftlyS2.Shared;

namespace MapChooser.Helpers;

public class MapCooldown
{
    private List<string> _mapsOnCooldown = new();
    private readonly MapChooserConfig _config;

    public MapCooldown(MapChooserConfig config)
    {
        _config = config;
    }

    public void OnMapStart(string mapName)
    {
        if (_config.MapsInCooldown <= 0)
        {
            _mapsOnCooldown.Clear();
            return;
        }

        string map = mapName.Trim().ToLower();
        
        if (_mapsOnCooldown.Contains(map))
            return;

        _mapsOnCooldown.Add(map);

        while (_mapsOnCooldown.Count > _config.MapsInCooldown)
        {
            _mapsOnCooldown.RemoveAt(0);
        }
    }

    public bool IsMapInCooldown(string mapName)
    {
        return _mapsOnCooldown.Contains(mapName.Trim().ToLower());
    }

    public void AddMapToCooldown(string mapName)
    {
        string map = mapName.Trim().ToLower();
        if (!_mapsOnCooldown.Contains(map))
        {
            _mapsOnCooldown.Add(map);
            while (_mapsOnCooldown.Count > _config.MapsInCooldown && _config.MapsInCooldown > 0)
            {
                _mapsOnCooldown.RemoveAt(0);
            }
        }
    }
}
