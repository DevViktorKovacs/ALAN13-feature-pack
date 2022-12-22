using ALAN13featurepack.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALAN13featurepack.Utility
{
    public class AlanStar : IPathFinder
    {
        Dictionary<int, StarCell> allCells;

        List<StarCell> openCells;

        List<StarCell> closedCells;

        public bool Found { get; set; }

        public AlanStar()
        {
            allCells = new Dictionary<int, StarCell>();

            openCells = new List<StarCell>();

            closedCells = new List<StarCell>();
        }

        public void AddPoint(int id, Vector2 position, float weightScale = 1)
        {
            var newCell = new StarCell()
            {
                Id = id,

                Position = position,

                Neigbours = new List<int>()
            };

            newCell.Reset();

            allCells.Add(id, newCell);
        }

        public bool ArePointsConnected(int id, int toId)
        {
            StarCell cell;

            var success = allCells.TryGetValue(id, out cell);

            if (success)
            {
                return cell.Neigbours.Any(n => n == toId);
            }

            return false;
        }

        public void ConnectPoints(int id, int toId, bool bidirectional = true)
        {
            StarCell fromCell;

            StarCell toCell;

            var successFrom = allCells.TryGetValue(id, out fromCell);

            var successTo = allCells.TryGetValue(toId, out toCell);


            if (successFrom && successTo)
            {
                fromCell.Neigbours.Add(toId);

                toCell.Neigbours.Add(id);
            }
        }

        public int[] GetIdPath(int fromId, int toId)
        {
            StarCell fromCell;

            StarCell toCell;

            var successFrom = allCells.TryGetValue(fromId, out fromCell);

            var successTo = allCells.TryGetValue(toId, out toCell);

            InitCollections(fromCell, toCell);

            while (!Found && openCells.Count > 0)
            {
                openCells.Sort(StarCellComparator);

                var first = openCells.First();

                CloseCell(first, toCell);
            }

            if (!Found)
            {
                ResetCollections();

                return new int[] { };
            }

            List<int> result = new List<int>();

            result.Add(toCell.Id);

            var parent = toCell.Parent;

            toCell.Reset();

            while (parent != default)
            {
                result.Add(parent.Id);

                parent = parent.Parent;
            }

            result.Reverse();

            ResetCollections();

            return result.ToArray();
        }

        private int StarCellComparator(StarCell x, StarCell y)
        {
            if (x.FCost == y.FCost)
            {
                return x.HCost.CompareTo(y.HCost);
            }

            return x.FCost.CompareTo(y.FCost);
        }

        private void InitCollections(StarCell fromCell, StarCell toCell)
        {
            Found = false;

            fromCell.Reset();

            toCell.Reset();

            ResetCollections();

            OpenStartingCell(fromCell, toCell);
        }

        private void ResetCollections()
        {
            foreach (var cell in openCells)
            {
                cell.Reset();
            }

            foreach (var cell in closedCells)
            {
                cell.Reset();
            }

            openCells.Clear();

            closedCells.Clear();
        }

        private bool CloseCell(StarCell closingCell, StarCell targetCell)
        {
            openCells.Remove(closingCell);

            closingCell.OnOpenList = false;

            closingCell.OnClosedList = true;

            foreach (var neigbourId in closingCell.Neigbours)
            {
                var neighbourCell = allCells[neigbourId];

                if (neighbourCell == targetCell)
                {
                    closedCells.Add(closingCell);

                    targetCell.Parent = closingCell;

                    Found = true;

                    return true;
                }

                if (neighbourCell.OnClosedList) continue;

                if (neighbourCell.OnOpenList)
                {
                    if (neighbourCell.EstimateFCost(closingCell) < neighbourCell.FCost)
                    {
                        neighbourCell.UpdateFCost(closingCell);
                    }

                    continue;
                }

                OpenCell(neighbourCell, closingCell, targetCell);
            }

            closedCells.Add(closingCell);

            return false;
        }

        private void OpenCell(StarCell currentCell, StarCell closingCell, StarCell targetCell)
        {
            currentCell.OnOpenList = true;

            currentCell.Parent = closingCell;

            SetCosts(currentCell, closingCell, targetCell);

            openCells.Add(currentCell);
        }

        private void OpenStartingCell(StarCell startingCell, StarCell targetCell)
        {
            startingCell.OnOpenList = true;

            openCells.Add(startingCell);

            startingCell.Gcost = 0;

            startingCell.HCost = (int)ManhattanDistance(startingCell.Position, targetCell.Position);

            startingCell.EvaluateFCost();
        }

        private void SetCosts(StarCell currentCell, StarCell closingCell, StarCell targetCell)
        {
            currentCell.Parent = closingCell;

            currentCell.Gcost = (int)ManhattanDistance(currentCell.Position, closingCell.Position) + closingCell.Gcost;

            currentCell.HCost = (int)ManhattanDistance(currentCell.Position, targetCell.Position);

            currentCell.FCost = 0;

            var grandParent = closingCell.Parent;

            if (grandParent != default && grandParent.Position.x != currentCell.Position.x && grandParent.Position.y != currentCell.Position.y)
            {
                currentCell.Gcost++;
            }

            currentCell.EvaluateFCost();
        }

        private static float ManhattanDistance(Vector2 from, Vector2 to)
        {
            var dx = Math.Abs(to.x - from.x);

            var dy = Math.Abs(to.y - from.y);

            return dx + dy;
        }

        class StarCell
        {
            public int Id { get; set; }
            public List<int> Neigbours { get; set; }

            public StarCell Parent { get; set; }

            public Vector2 Position { get; set; }

            public int Gcost { get; set; }

            public int HCost { get; set; }

            public int FCost { get; set; }

            public bool OnOpenList { get; set; }

            public bool OnClosedList { get; set; }

            public void Reset()
            {
                Gcost = 100000;

                HCost = 100000;

                FCost = 100000;

                Parent = default;

                OnClosedList = false;

                OnOpenList = false;
            }

            public void EvaluateFCost()
            {
                FCost = Gcost + HCost;
            }

            public int EstimateFCost(StarCell closingCell)
            {
                var newGCost = (int)ManhattanDistance(Position, closingCell.Position) + closingCell.Gcost;

                return HCost + newGCost;
            }

            public void UpdateFCost(StarCell closingCell)
            {
                Gcost = (int)ManhattanDistance(Position, closingCell.Position) + closingCell.Gcost;

                EvaluateFCost();

                Parent = closingCell;
            }
        }
    }
}
