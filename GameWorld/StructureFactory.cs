using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.GameWorld
{
    public class StructureFactory
    {
        private TileMap shadows;

        private TileMap objects;

        private TileGridControl parentTileGrid;
        public StructureFactory(TileGridControl tileGrid)
        {
            parentTileGrid = tileGrid;

            shadows = tileGrid.Layers[1];

            objects = tileGrid.Layers[2];
        }

		public Structure GetStructure(StructureData data, TileCell cell)
		{

			return new Structure();
		}
	}
}
