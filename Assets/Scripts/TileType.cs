using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zs.Assets.Scripts
{
    public enum TileType
    {
        Outside,
        Open,
        Blocked,


        // Only available during generation
        FenceN,
        FenceNW,
        FenceW,
        FenceSW,
        FenceS,
        FenceSE,
        FenceE,
        FenceNE,
        Rock,
        StartTile,
        SheepTile
    }
}
