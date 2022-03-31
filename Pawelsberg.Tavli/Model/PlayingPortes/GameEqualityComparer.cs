using Pawelsberg.Tavli.Model.Common;

namespace Pawelsberg.Tavli.Model.PlayingPortes;

public class GameEqualityComparer : IEqualityComparer<Game>
{
    public GameEqualityComparer() { }
    public bool Equals(Game x, Game y)
    {

        if (!x.Board.IsSameAs(y.Board))
            return false;

        if (x.State != y.State)
            return false;

        if (x.StatePlayer != y.StatePlayer)
            return false;

        foreach (PlayerColour playerColour in Enum.GetValues(typeof(PlayerColour)))
        {
            if (x.BearedOffCheckers[playerColour].Count != y.BearedOffCheckers[playerColour].Count)
                return false;

#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (x.BearedOffCheckers[playerColour].Select((boc, i) => y.BearedOffCheckers[playerColour][i] == boc).Any(same => !same))
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                return false;

            if (x.ToBeBoardedCheckers[playerColour].Count != y.ToBeBoardedCheckers[playerColour].Count)
                return false;

#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            if (x.ToBeBoardedCheckers[playerColour].Select((boc, i) => y.ToBeBoardedCheckers[playerColour][i] == boc).Any(same => !same))
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
                return false;
        }

        return true;
    }

    public int GetHashCode(Game game)
    {
        return game.Board.GenerateHashCode()
            ^ game.State.GetHashCode()
            ^ (int)(game.StatePlayer ?? 0)
            ^ game.BearedOffCheckers[PlayerColour.White].Aggregate(0, (acc, c) => acc ^ c.GetHashCode())
            ^ game.BearedOffCheckers[PlayerColour.Black].Aggregate(0, (acc, c) => acc ^ c.GetHashCode())
            ^ game.ToBeBoardedCheckers[PlayerColour.White].Aggregate(0, (acc, c) => acc ^ c.GetHashCode())
            ^ game.ToBeBoardedCheckers[PlayerColour.Black].Aggregate(0, (acc, c) => acc ^ c.GetHashCode())
        ;
    }
}

