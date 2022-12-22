using ALAN13featurepack.GameWorld;
using ALAN13featurepack.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.Tween;

namespace ALAN13featurepack.Utility
{
    public static class ExtensionMethods
    {
        public static Node GetMainScene(this Node node)
        {
            return node.GetTree().Root;
        }

        public static Vector2 Clone(this Vector2 vector2)
        {
            return new Vector2(vector2.x, vector2.y);
        }

        public static int Modulo(this int baseInt, int operand)
        {
            if (operand == 0) return 0;

            return (baseInt % operand + operand) % operand;
        }

        /// <summary>
        /// Enables/Disables processing on the given node, including it's children.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="enable"></param>
        public static void SetProcessingInputResursive(this Node node, bool enable)
        {
            node.SetPhysicsProcess(enable);
            node.SetProcess(enable);
            node.SetProcessUnhandledInput(enable);
            node.SetProcessInput(enable);

            foreach (var child in node.GetChildren())
            {
                var item = child as Node;
                if (item != null)
                {
                    item.SetProcessingInputResursive(enable);
                }
            }
        }

        public static T GetChild<T>(this Node node, bool withPolymorphism = false) where T : Node
        {
            foreach (var child in node.GetChildren())
            {
                var typeOfChild = child.GetType();

                if (!withPolymorphism && typeof(T).Equals(typeOfChild))
                {
                    return (T)child;
                }

                if (withPolymorphism && (typeof(T).Equals(typeOfChild) || (typeof(T).IsAssignableFrom(typeOfChild))))
                {
                    return (T)child;
                }
            }

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="withPolymorphism"> if true, returns any instance which can be inherited from the given type </param>
        /// <returns></returns>
        public static IEnumerable<T> GetChildren<T>(this Node node, bool withPolymorphism = true) //where T : Node
        {
            List<T> result = new List<T>();

            foreach (var child in node.GetChildren())
            {
                var typeOfChild = child.GetType();

                if (!withPolymorphism && typeof(T).Equals(typeOfChild))
                {
                    result.Add((T)child);
                }

                if (withPolymorphism && (typeof(T).Equals(typeOfChild) || (typeof(T).IsAssignableFrom(typeOfChild))))
                {
                    result.Add((T)child);
                }
            }

            return result;
        }

        public static T GetChild<T>(this Node node, string name) where T : Node
        {
            foreach (var child in node.GetChildren())
            {
                if ((child is Node) && (child as Node).Name == name)
                    return (T)child;
            }

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="withPolymorphism"> if true, returns any instance which can be inherited from the given type</param>
        /// <returns></returns>
        public static IEnumerable<T> GetChildrenRecursive<T>(this Node node, bool withPolymorphism = true) //where T : Node
        {
            List<T> result = new List<T>();

            return GetDescendants(node, withPolymorphism, result);
        }

        public static Node GetChildByName(this Node node, string name)
        {
            Node result = null;

            foreach (var child in node.GetChildren())
            {
                var item = child as Node;
                if (item != null)
                {
                    if (item.Name.Equals(name)) result = item;
                }

            }
            return result;
        }

        public static Node GetChildByNameRecursive(this Node node, string name)
        {
            var children = node.GetChildren();

            foreach (var child in children)
            {
                var item = child as Node;

                if (item != null)
                {
                    if (item.Name.Equals(name)) return item;
                }
            }

            foreach (var child in children)
            {
                var grandChild = (child as Node).GetChildByNameRecursive(name);

                if (grandChild != default) return grandChild;
            }

            return default;
        }

        private static IEnumerable<T> GetDescendants<T>(Node node, bool withPolymorphism, List<T> result)
        {
            var children = node.GetChildren();

            foreach (var child in children)
            {
                var typeOfChild = child.GetType();

                if (!withPolymorphism && typeof(T).Equals(typeOfChild))
                {
                    result.Add((T)child);
                }

                if (withPolymorphism && (typeof(T).Equals(typeOfChild) || (typeof(T).IsAssignableFrom(typeOfChild))))
                {
                    result.Add((T)child);
                }
            }

            foreach (var child in children)
            {
                GetDescendants((Node)child, withPolymorphism, result);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="withPolymorphism"> if true, returns any instance which can be inherited from the given type</param>
        /// <returns></returns>
        public static T GetChildRecursive<T>(this Node node, bool withPolymorphism = true) where T : class
        {
            var children = node.GetChildren();

            foreach (var child in children)
            {
                var typeOfChild = child.GetType();

                if (!withPolymorphism && typeof(T).Equals(typeOfChild))
                {
                    return (T)child;
                }

                if (withPolymorphism && (typeof(T).Equals(typeOfChild) || (typeof(T).IsAssignableFrom(typeOfChild))))
                {
                    return (T)child;
                }
            }

            foreach (var child in children)
            {
                var grandChild = (child as Node).GetChildRecursive<T>(withPolymorphism);

                if (grandChild != default) return grandChild;
            }

            return default;
        }

        public static T GetAncestor<T>(this Node node, bool withPolymorphism = true) where T : Node
        {
            var parent = node.GetParent();

            if (parent == default) return default;

            var typeOfParent = parent.GetType();

            if (!withPolymorphism && typeof(T).Equals(typeOfParent))
            {
                return (T)parent;
            }

            if (withPolymorphism && (typeof(T).Equals(typeOfParent) || (typeof(T).IsAssignableFrom(typeOfParent))))
            {
                return (T)parent;
            }

            return parent.GetAncestor<T>();
        }

        public static InputProcessor InputProcessor(this Node node)
        {
            return (InputProcessor)node.GetNode("/root/InputProcessor");
        }


        public static float ToRadians(this float value)
        {
            return (3.1416f / 180f) * value;
        }

        public static TileGridControl GetTileGrid(this Node node, int tileGridId = 0)
        {
            var results = node.GetTree().Root.GetChildrenRecursive<ILevel>(true).ToList();

            var grid = results.Where(l => l.TileGridControl.ID == tileGridId).FirstOrDefault();

            return grid.TileGridControl;
        }


        public static T InstantiateNewChild<T>(this Node node, string resourcePath) where T : Node
        {
            var spriteResource = (PackedScene)ResourceLoader.Load(resourcePath);

            var instance = (T)spriteResource.Instance();

            node.AddChild(instance);

            return instance;
        }

        public static Vector2 GetCellCenterPosition(this TileMap tilemap, Vector2 cellPosition)
        {
            return new Vector2(cellPosition.x, cellPosition.y + (tilemap.CellSize.y / 2));
        }

        public static Vector2 GetCellCenterWorldPosition(this TileMap tilemap, Vector2 gridPosition)
        {
            return tilemap.GetCellCenterPosition(tilemap.MapToWorld(gridPosition));
        }

        public static bool IsInsideTriangle(this Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
            var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

            if ((s < 0) != (t < 0))
                return false;

            var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;

            return A < 0 ?
                    (s <= 0 && s + t >= A) :
                    (s >= 0 && s + t <= A);
        }

        public static bool IsInsideRectangle(this Vector2 p, Vector2 p0, Vector2 p1)
        {
            return p.x > p0.x && p.x < p1.x && p.y > p0.y && p.y < p1.y;
        }

        public static bool IsLeftClick(this InputEvent @event)
        {
            return @event is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == (int)ButtonList.Left;
        }

        public static bool IsMouseEvent(this InputEvent @event)
        {
            return (@event is InputEventMouse);
        }

        public static void CallAction(this Node node, string action)
        {
            node.Call(action, null);
        }

        public static void SkipAll(this Tween tween)
        {
            tween.StopAll();
            tween.RemoveAll();
        }

        public static IEnumerable<T> Flatten<T>(this T[,] map)
        {
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    yield return map[row, col];
                }
            }
        }

        public static bool TryToGetValue<T>(this T[,] array, int x, int y, out T result)
        {
            result = default;

            if (array.GetLength(0) <= x || array.GetLength(1) <= y || x < 0 || y < 0) return false;

            result = array[x, y];

            return true;
        }

        public static bool TryToGetValue<T>(this T[] array, int i, out T result)
        {
            result = default;

            if (i < 0 || array.Length <= i) return false;

            result = array[i];

            return true;
        }

        public static bool TryToAssignValue<T>(this T[] array, int i, T value)
        {
            if (i < 0 || array.Length <= i) return false;

            array[i] = value;

            return true;
        }

        public static bool TryToGetValue<T>(this List<T> list, int index, out T result)
        {
            result = default;

            if (list == null || list.Count <= index || index < 0) return false;

            result = list[index];

            return true;
        }

        public static byte[] GetBytes(this int[] array)
        {
            return array.Select(i => (byte)i).ToArray();
        }

        public static int[] ToIntArray(this byte[] bytes)
        {
            return bytes.Select(i => (int)i).ToArray();
        }

        public static int RandomNext(this IEnumerable<int> range, Random seed)
        {
            var list = range.ToList();

            if (list.Count > 0)

                return list[seed.Next(0, list.Count)];

            return 0;
        }

        public static int IndexOf<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            var list = dictionary.ToList();

            int result = -1;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Key.Equals(key))
                {
                    result = i;

                    break;
                }
            }

            return result;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        public static List<string> WrapLines(this string value, int lineLength)
        {
            if (value.Length < lineLength) return new List<string>() { value };

            var words = value.Split(" ");

            List<string> result = new List<string>();

            string currentLine = words[0];

            for (int i = 1; i < words.Length; i++)
            {
                var concated = $"{currentLine} {words[i]}";

                if (concated.Length > lineLength)
                {
                    result.Add(currentLine);

                    currentLine = words[i];

                    continue;
                }

                currentLine = concated;
            }

            result.Add(currentLine);

            return result;
        }

        public static void Shuffle<T>(this IList<T> list, Random seed)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = seed.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool InterpolateProperty(this Tween tween, InterpolateParams interpolateParams)
        {
            return tween.InterpolateProperty(interpolateParams.Subject, interpolateParams.Property, interpolateParams.InitialValue, interpolateParams.FinalValue, interpolateParams.Duration, interpolateParams.TransitionType, interpolateParams.EaseType, interpolateParams.Delay);
        }
    }

    public struct IntVector2 { public int x, y; }

    public struct InterpolateParams
    {
        public Godot.Object Subject;

        public NodePath Property;

        public object InitialValue;

        public object FinalValue;

        public float Duration;

        public TransitionType TransitionType;

        public EaseType EaseType;

        public float Delay;
    }
}
