using MapChooser.Models;

namespace MapChooser.Helpers;

public class MapLister
{
    private List<Map> _maps = new();

    public IReadOnlyList<Map> Maps => _maps;

    public void UpdateMaps(List<Map> maps)
    {
        _maps = maps ?? new List<Map>();
    }
}
