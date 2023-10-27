using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public delegate int HeuristicDelegate<T>(T a, T b);

// IEquatable is included for so we can hash T.
public interface INode<T> : IEquatable<T>
{
    int HeuristicDistance(T destination);

    List<T> GetNeighbors();
}

/*
Example heuristic:
int EuclideanDistance((int x, int y) source, (int x, int y) destination)
{
    return Mathf.Abs(source.x - destination.x) + Mathf.Abs(source.y - destination.y);
}
*/

// A* is only garunteed to give an optimal solution if the heuristic
// never overestimates!
// TODO: Try making this (and the method) static
public static class AStar 
{
    public static int Distance<T>(T source, T destination) where T : INode<T>
    {
        var heuristic_distance = source.HeuristicDistance(destination);
        UnityEngine.Debug.Log("[AStar] In AStar.Distance! HeuristicDistance is " + heuristic_distance);

        if (heuristic_distance <= 1)
        {
            UnityEngine.Debug.Log("[AStar] Returning heuristic_distance since it's <= 1.");
            return heuristic_distance;
        }

        // "Open tiles" are tiles that are available for us to step into.
        AStarHeap<T> open_nodes = new();
        open_nodes.Insert(source, 0, source.HeuristicDistance(destination));

        // "Closed tiles" are tiles we've already considered - tiles that were "open"
        // and then selected. We don't need to revisit them because the first time
        // is when they were cheapest.
        HashSet<T> closed_tiles = new();



        UnityEngine.Debug.Log("[AStar] Going into 'while (!open_nodes.Empty())'");
        int loops = 0;
        // while (!open_nodes.Empty())
        while (loops == 0)
        {
            var (current, gCost) = open_nodes.Pop();
            if (current.Equals(destination))
            {
                UnityEngine.Debug.Log("[AStar] Returning gcost " + gCost);
                return gCost;
            }
            closed_tiles.Add(current);

            // TODO: Only consider traversable neighbors!
            var neighbors = current.GetNeighbors();
            UnityEngine.Debug.LogFormat("[AStar] Got {0} neighbors!", neighbors.Count);
            foreach (var neighbor in neighbors)
            {
                if (closed_tiles.Contains(neighbor))
                {
                    continue;
                }

                if (open_nodes.Contains(neighbor))
                {
                    // This internally checks if the new gCost is better (lower).
                    open_nodes.UpdateItem(neighbor, gCost + 1);
                }
                else
                {
                    open_nodes.Insert(neighbor, gCost + 1, neighbor.HeuristicDistance(destination));
                }

            }
            loops += 1;
        }
        return -1;
    }
}

/*public class AStar<T> : MonoBehaviour where T : IAStarNode<T>, IEquatable<T>
{
    int FindPath(T source, T destination)
    {
        AStarNode<T> source_node = new(source, destination, 0);

        // "Open tiles" are tiles that are available for us to step into.
        Heap<AStarNode<T>> open_nodes = new();

        // "Closed tiles" are tiles we've already considered - tiles that were "open"
        // and then selected. We don't need to revisit them because the first time
        // is when they were cheapest.
        HashSet<T> closed_tiles = new();

        open_nodes.Insert(source_node);

        while (!open_nodes.Empty())
        {
            AStarNode<T> current = open_nodes.Pop();
            if (current.item.Equals(destination))
            {
                return current.gCost;
            }
            closed_tiles.Add(current.item);

            // TODO: Only consider traversable neighbors!
            foreach (var neighbor in current.item.GetNeighbors())
            {
                if (closed_tiles.Contains(neighbor))
                {
                    continue;
                }
                // TODO: Making this object just to check if it exists is
                // inefficient but ok for now :(
                // Check if neighbor is in open
                AStarNode<T> neighbor_node = new(neighbor, destination, current.gCost + 1);
                if (open_nodes.Contains(neighbor_node))
                {
                    // Here!
                    // Update the stored one if the current one is faster.
                } else
                {
                    open_nodes.Insert(neighbor_node);
                }

            }

        }

        return -1;
    }

}*/