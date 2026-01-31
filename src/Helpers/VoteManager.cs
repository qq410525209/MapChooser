using MapChooser.Models;
using MapChooser.Dependencies;
using MapChooser.Helpers;

namespace MapChooser.Helpers;

public class VoteManager
{
    private readonly HashSet<int> _voters = new();
    private readonly int _requiredPercentage;

    public int VoteCount => _voters.Count;

    public VoteManager(int requiredPercentage = 60)
    {
        _requiredPercentage = requiredPercentage;
    }

    public bool AddVote(int playerSlot)
    {
        return _voters.Add(playerSlot);
    }

    public bool RemoveVote(int playerSlot)
    {
        return _voters.Remove(playerSlot);
    }

    public void Clear()
    {
        _voters.Clear();
    }

    public bool HasReached(int totalPlayers)
    {
        if (totalPlayers == 0) return false;
        int required = (int)Math.Ceiling(totalPlayers * (_requiredPercentage / 100.0));
        return VoteCount >= required;
    }

    public int GetRequiredVotes(int totalPlayers)
    {
        return (int)Math.Ceiling(totalPlayers * (_requiredPercentage / 100.0));
    }
}
