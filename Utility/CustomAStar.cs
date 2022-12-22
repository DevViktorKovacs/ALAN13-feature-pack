using ALAN13featurepack.Interfaces;
using Godot;
using System;

namespace ALAN13featurepack.Utility
{
    public class CustomAStar : AStar2D, IPathFinder
    {
        public override float _ComputeCost(int fromId, int toId)
        {
            var from = GetPointPosition(fromId);

            var to = GetPointPosition(toId);

            return ManhattanDistance(from, to);
        }

        public override float _EstimateCost(int fromId, int toId)
        {
            var from = GetPointPosition(fromId);

            var to = GetPointPosition(toId);

            return ManhattanDistance(from, to);
        }

        private float ManhattanDistance(Vector2 from, Vector2 to)
        {
            var dx = Math.Abs(to.x - from.x);

            var dy = Math.Abs(to.y - from.y);

            return dx + dy;
        }
    }
}
