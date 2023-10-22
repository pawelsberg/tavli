using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pawelsberg.Tavli.Model.Common;

public abstract record PlayerBase
{
    public abstract TurnPlayBase ChooseTurnPlay(GameBase game, TurnRoll roll);
}