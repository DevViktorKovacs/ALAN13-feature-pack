using Godot;

namespace ALAN13featurepack.Interfaces
{
    public interface IPathFinder
    {
        int[] GetIdPath(int fromId, int toId);

        void AddPoint(int id, Vector2 position, float weightScale = 1);

        void ConnectPoints(int id, int toId, bool bidirectional = true);

        bool ArePointsConnected(int id, int toId);
    }
}
