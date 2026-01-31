namespace MapChooser.Models;

public class Map
{
    public string Name { get; set; } = string.Empty;
    public string? Id { get; set; }

    public Map() { }

    public Map(string name, string? id = null)
    {
        Name = name.Trim();
        Id = id?.Trim();
    }
}
