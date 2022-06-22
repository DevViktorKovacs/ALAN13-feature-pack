using ALAN13featurepack.GameWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.Interfaces
{
    public interface ILevel
    {
        TileGridControl TileGridControl { get; }
    }
}
