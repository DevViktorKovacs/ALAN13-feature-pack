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

		private int currentIndex = 0;

		public StructureFactory(TileGridControl tileGrid)
        {
            parentTileGrid = tileGrid;

            shadows = tileGrid.Layers[1];

            objects = tileGrid.Layers[2];
        }

		public Structure GetStructure(StructureData data, TileCell cell)
		{
			var result = InstantiateStructureObject(data, cell);

			PositionStructure(result, cell);

			return result;
		}

		private Structure InstantiateStructureObject(StructureData data, TileCell cell)
		{
			var result = new Structure();

			var resource = (PackedScene)ResourceLoader.Load(data.Scene);

			if (resource == default)
			{
				DebugHelper.PrintError($"Resource not found: [{data.Scene}]!");

				return default;
			}

			var instance = resource.Instance();

			var position = objects.GetCellCenterWorldPosition(cell.GridPosition);

			var structureSprite = instance.GetChild<AnimatedSprite>();

			AudioStreamPlayer2D audioStreamPlayer = instance.GetChild<AudioStreamPlayer2D>();

			if (audioStreamPlayer != default)
			{
				result.AudioStreamPlayer = audioStreamPlayer;

				instance.RemoveChild(audioStreamPlayer);
			}

			structureSprite.Position = position;

			result.StructureSprites = structureSprite;

			result.StructureType = data.StructureType;

			result.Orientation = data.Orientation;

			var shadowsAndOrnaments = instance.GetChildren<Sprite>();

			result.Shadow = shadowsAndOrnaments.FirstOrDefault();

			if (shadowsAndOrnaments.Count() > 1)
			{
				result.Ornament = shadowsAndOrnaments.LastOrDefault();
			}

			foreach (var sprite in shadowsAndOrnaments)
			{
				instance.RemoveChild(sprite);
			}

			var area = instance.GetChild<Area2D>();

			if (area != null)
			{
				instance.RemoveChild(area);

				result.Area = area;
			}

			var content = instance.GetChild<Node2D>();

			if (content != null)
			{
				instance.RemoveChild(content);

				result.Content = content;
			}

			instance.RemoveChild(structureSprite);

			instance.Free();

			cell.Structure = result;

			cell.Occupant = Interfaces.Occupant.Obstacle;

			result.GridPosition = cell.GridPosition;

			result.TileCell = cell;

			result.ID = currentIndex++;

			return result;
		}

		private Vector2 PositionStructure(Structure structure, TileCell cell, bool placeOnShadowLayer = false)
		{
			var position = objects.GetCellCenterWorldPosition(cell.GridPosition);

			structure.StructureSprites.Position = position;

			if (structure.Shadow != default)
			{
				structure.Shadow.Position = position;

				shadows.AddChild(structure.Shadow);
			}


			objects.AddChild(structure.StructureSprites);



			return position;
		}
	}
}
