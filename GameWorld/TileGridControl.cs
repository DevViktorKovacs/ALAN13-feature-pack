using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.GameWorld
{
    public class TileGridControl
    {
		public TileCell SelectedCell { get; set; }

		public CustomAStar AStar2D { get; set; }

		public TileMap[] Layers => new TileMap[] { ground, shadows, objects };
		public int ID => id;

		public static int LAYER_INDEX_GROUND = 0;

		public static int LAYER_INDEX_SHADOWS = 1;

		public static int LAYER_INDEX_OBJECTS = 2;

		public Random RNG { get; set; }

		public static float VoidOffset = 400f;

		public int MaxShift { get { return maxShift; } set { maxShift = value; } }

		private int maxShift = 800;

		readonly TileCell[,] cells;

		readonly TileMap ground;

		readonly TileMap shadows;

		readonly TileMap objects;

		readonly StructureFactory factory;

		readonly int offsetX;

		readonly int offsetY;

		Sprite tileCellSelection;

		int id;

		public static Vector2[] ConnectedNeigbourVectors = new Vector2[]
		{
				new Vector2(1,0),
				new Vector2(-1,0),
				new Vector2(0,-1),
				new Vector2(0,1)
		};

		public TileGridControl(TileMap ground, TileMap shadows, TileMap objects)
		{
			id = InputProcessor.gridIds++;

			this.ground = ground;

			this.shadows = shadows;

			this.objects = objects;

			var gridSize = new Vector2(objects.CellQuadrantSize, objects.CellQuadrantSize);

			cells = new TileCell[(int)gridSize.x, (int)gridSize.y];

			offsetX = (int)gridSize.x / 2;

			offsetY = (int)gridSize.y / 2;

			RNG = new Random();

			factory = new StructureFactory(this);

			InitializeCells();

			InstantiateCellSelection(GetCellAt(0, 0));
		}

		/// <summary>
		/// Gets cell on the given grid based coordinates
		/// </summary>
		/// <param name="gridPosition"></param>
		/// <returns></returns>
		public TileCell GetCellAt(Vector2 gridPosition)
		{
			var x = (int)gridPosition.x + offsetX;

			var y = (int)gridPosition.y + offsetY;

			if (x > cells.GetLength(0) - 1 || y > cells.GetLength(1) - 1 || x < 0 || y < 0)
			{
				return cells[0, 0];
			}

			return cells[x, y];
		}

		public TileCell GetCellAt(int x, int y)
		{
			return cells[x + offsetX, y + offsetY];
		}

		public Vector2 GetCellCenterAtGridPosition(Vector2 gridPosition)
		{
			return objects.GetCellCenterWorldPosition(gridPosition);
		}

		private void InitializeCells()
		{
			DebugHelper.Print("Initializing TileMap...");

			IterateThroughCells((coords) =>
			{
				CreateCell(coords);
			});

			cells[0, 0].Occupant = Occupant.DefaultTile;


			DebugHelper.Print("TileMap initialization finished!");
		}

		private void CreateCell(IntVector2 coords)
		{
			int x = coords.x - offsetX;

			int y = coords.y - offsetY;

			var gridPosition = new Vector2(x, y);

			var newCell = new TileCell(gridPosition, objects.CellSize, ID);

			newCell.WorldPositionOfCenter = objects.GetCellCenterPosition(objects.MapToWorld(gridPosition));

			Occupant occupant = GetOccupantFromLayer(ground, x, y);

			if (occupant == Occupant.Walkable)
			{
				occupant = GetOccupantFromLayer(objects, x, y);

			}

			newCell.Occupant = occupant;

			var objectLayerTile = GetTileName(objects, x, y);

			if (StaticData.StructureDataSet.ContainsKey(objectLayerTile))
			{
				objects.SetCell(x, y, -1);

				factory.GetStructure(StaticData.StructureDataSet[objectLayerTile], newCell);
			}

			if (IsCellEmpty(newCell))
			{
				newCell.Occupant = Occupant.Void;
			}
			else
			{
				var tileName = GetTileName(ground, (int)newCell.GridPosition.x, (int)newCell.GridPosition.y);

				GroundType groundType;

				StaticData.GroundTypes.TryGetValue(tileName, out groundType);

				newCell.GroundType = groundType;
			}

			newCell.AStarId = coords.x * 100 + coords.y;

			cells[coords.x, coords.y] = newCell;
		}

		private void IterateThroughCells(Action<IntVector2> action)
		{
			for (int i = 0; i < cells.GetLength(0); i++)
			{
				for (int j = 0; j < cells.GetLength(1); j++)
				{
					action(new IntVector2() { x = i, y = j });
				}
			}
		}

		private Occupant GetOccupantFromLayer(TileMap layer, int x, int y)
		{
			Occupant occupant;

			var tileName = GetTileName(layer, x, y);

			StaticData.TileTypes.TryGetValue(tileName, out occupant);

			return occupant;
		}

		private string GetTileName(TileMap layer, int x, int y)
		{
			var tileName = string.Empty;

			var layerIdx = layer.GetCell((int)x, (int)y);

			if (layer.TileSet.GetTilesIds().Contains(layerIdx))
			{
				tileName = layer.TileSet.TileGetName(layerIdx);
			}

			return tileName;
		}

		public bool IsCellEmpty(TileCell cell)
		{
			var tileName = GetTileName(ground, (int)cell.GridPosition.x, (int)cell.GridPosition.y);

			if (tileName == string.Empty) return true;

			return false;
		}

		public void ShiftMap(Vector2 shift)
		{

			ground.Position += shift;

			float x = ground.Position.x;

			float y = ground.Position.y;

			if (ground.Position.x < -1 * maxShift) x = -1 * maxShift;
			if (ground.Position.x > maxShift) x = maxShift;
			if (ground.Position.y < -1 * maxShift) y = -1 * maxShift;
			if (ground.Position.y > maxShift) y = maxShift;

			ground.Position = new Vector2(x, y);

			shadows.Position = ground.Position;
			objects.Position = ground.Position;
		}

		private bool IsShiftOutOfArea(Vector2 shift)
		{

			var offsetAfterShift = ground.Position + shift;

			return offsetAfterShift.x < (-1) * MaxShift || offsetAfterShift.x > MaxShift || offsetAfterShift.y < (-1) * MaxShift || offsetAfterShift.y > MaxShift;
		}

		public void SetMapPosition(Vector2 offset)
		{
			ground.Position = offset;
			shadows.Position = offset;
			objects.Position = offset;
		}

		public void Scale(Vector2 scale)
		{
			ground.Scale = scale;
			shadows.Scale = scale;
			objects.Scale = scale;
		}

		private void InstantiateCellSelection(TileCell cell)
		{
			var resource = (PackedScene)ResourceLoader.Load("res://GameWorld/CellSelection.tscn"); 

			var instance = resource.Instance();

			var sprite = instance.GetChild<Sprite>();

			var position = objects.GetCellCenterWorldPosition(cell.GridPosition);

			sprite.Position = position;

			sprite.ZIndex++;

			instance.RemoveChild(sprite);

			instance.Free();

			shadows.AddChild(sprite);

			tileCellSelection = sprite;
		}

		public TileCell SelectCell()
		{
			var worldPosition = shadows.GetLocalMousePosition();

			var gridposition = shadows.WorldToMap(worldPosition);

			var cell = GetCellAt(gridposition);

			tileCellSelection.Position = objects.GetCellCenterWorldPosition(cell.GridPosition);

			if (IsCellEmpty(cell)) tileCellSelection.Visible = false;

			else tileCellSelection.Visible = true;

			SelectedCell = cell;

			return cell;
		}

		public TileCell GetCellFromLocalPosition(Vector2 localPosition)
		{
			var gridposition = objects.WorldToMap(localPosition);

			return GetCellAt(gridposition);
		}

		public TileCell GetCellAtMousePosition()
		{
			return GetCellFromLocalPosition(GetLocalMousePosition());
		}

		public Vector2 GetLocalMousePosition()
		{
			return shadows.GetLocalMousePosition();
		}
	}
}
