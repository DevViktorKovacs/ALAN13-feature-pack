using ALAN13featurepack.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.GameWorld
{
    public class TileCell
    {
        private Vector2 cellSize;

        private int tileGridId;

        public int TileGridId => tileGridId;

        public float ActiveOffset { get; set; }

        public Vector2 GridPosition { get; set; }

        public Vector2 WorldPositionOfCenter { get; set; }

        public Structure Structure { get; set; }

        public GroundType GroundType { get; set; }

        public int AStarId { get; set; }

        private Occupant occupant;

        public Occupant Occupant
        {

            get { return occupant; }
            set
            {
                occupant = value;
            }
        }

        public TileCell(Vector2 gridPosition, Vector2 cellSize, int tileGridID)
        {
            this.cellSize = cellSize;

            GridPosition = gridPosition;

            ActiveOffset = 2.1f;

            tileGridId = tileGridID;
        }


        public Vector2 TargetPositionWithOffset(Vector2 direction)
        {
            var isoDirection = ConvertDirectionToIsometricDirection(direction);

            var edgeOffset = new Vector2(cellSize.x / ActiveOffset, cellSize.y / ActiveOffset);

            var edgeOffsetVector = new Vector2(isoDirection.x * (-1) * edgeOffset.x * 0.5f, isoDirection.y * (-1) * edgeOffset.y * 0.5f);

            return WorldPositionOfCenter + edgeOffsetVector;
        }

        private Vector2 ConvertDirectionToIsometricDirection(Vector2 direction)
        {
            var result = new Vector2();

            if (direction.x != 0)
            {
                result.x = direction.x;
                result.y = direction.x / 2;
            }

            else
            {
                result.x = direction.y * (-1);
                result.y = direction.y / 2;
            }

            return result;
        }

        public Vector2 GetBorder(WorldOrientation orientation)
        {
            var direction = StaticData.OrientationData[orientation];

            var cartesianDirection = ConvertDirectionToIsometricDirection(direction);

            var offset = cartesianDirection * (cellSize.x / 4);

            return WorldPositionOfCenter + offset;
        }

        public Vector2 GetBorderWithOffset(WorldOrientation orientation, float borderOffset)
        {
            var direction = StaticData.OrientationData[orientation];

            var cartesianDirection = ConvertDirectionToIsometricDirection(direction);

            var offset = cartesianDirection * (borderOffset / 4);

            return WorldPositionOfCenter + offset;
        }

        public override string ToString()
        {
            string result = $"GPS:{GridPosition} Type:{GroundType} {Occupant} A*id:{AStarId}, TileGridId:{tileGridId}";

            if (Structure != null) result = result + System.Environment.NewLine + $" {Structure.ToString()}";

            return result;
        }

        public bool IsNeighbour(Vector2 targetPosition)
        {
            if (Math.Abs(targetPosition.x - GridPosition.x) == 1 && targetPosition.y == GridPosition.y) return true;

            if (Math.Abs(targetPosition.y - GridPosition.y) == 1 && targetPosition.x == GridPosition.x) return true;

            return false;
        }

        public List<TileCell> GetNeighboursWhere(TileGridControl grid, Predicate<TileCell> predicate)
        {
            var result = new List<TileCell>();

            for (int i = 0; i < TileGridControl.ConnectedNeigbourVectors.Length; i++)
            {
                var neigbourGridCoords = GridPosition + TileGridControl.ConnectedNeigbourVectors[i];

                var neighbour = grid.GetCellAt(neigbourGridCoords);

                if (predicate(neighbour)) result.Add(neighbour);
            }

            return result;
        }

        public WorldOrientation GetNeigbourDirection(Vector2 targetGridPosition)
        {
            if (GridPosition.x < targetGridPosition.x) return WorldOrientation.SouthEast;

            if (GridPosition.x > targetGridPosition.x) return WorldOrientation.NorthWest;

            if (GridPosition.y < targetGridPosition.y) return WorldOrientation.SouthWest;

            if (GridPosition.y > targetGridPosition.y) return WorldOrientation.NorthEast;

            return WorldOrientation.SouthEast;
        }

        public Vector2 GetOffset(Vector2 position)
        {
            return WorldPositionOfCenter - position;
        }
    }


}
