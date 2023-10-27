using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// https://youtu.be/3Dw5d7PlcTM



// MinHeap
// Current task: Refactor to use the `indices` Dictionary!
public class AStarHeap<T>
{
    class AStarNode : IComparable<AStarNode>
    {
        public T Item;
        public int Gcost;
        public int Fcost;
        public AStarNode(T item, int gCost, int hCost)
        {
            Item = item;
            Gcost = gCost;
            Fcost = gCost + hCost;
        }
        public int CompareTo(AStarNode nodeToCompare)
        {
            int comparison = Fcost.CompareTo(nodeToCompare.Gcost);
            if (comparison == 0)
            {
                // I use Gcost here because we already store it. The video
                // used Hcost instead, idk why.
                comparison = Gcost.CompareTo(nodeToCompare.Fcost);
            }
            return comparison;
        }
    }

    // This is the list representation of the heap.
    List<AStarNode> _nodes;
    Dictionary<T, int> _indices;


    public AStarHeap()
    {
        _nodes = new List<AStarNode>();
        _indices = new Dictionary<T, int>();
    }

    public void Insert(T item, int gCost, int hCost)
    {
        UnityEngine.Debug.LogFormat("[Heap] Called Insert(item, {0}, {1})", gCost, hCost);
        // Add it to the end of the array, then "sort up".
        _indices[item] = _indices.Count;

        var node = new AStarNode(item, gCost, hCost);
        _nodes.Add(node); // TODO: This might be the problem?
        // Now we need to compare the item with its parent.
        SortUp(node);
    }

    // We may need to update items during the A*/Dijkstra algorithm.
    public void UpdateItem(T item, int g_cost)
    {
        if (!_indices.ContainsKey(item) || _nodes[_indices[item]].Gcost <= g_cost)
        {
            return;
        }

        _nodes[_indices[item]].Gcost = g_cost;

        // We only every increase the priority (i.e. we lower the cost
        // if we find shorter path to this item/node), so we can just
        // call SortUp.
        SortUp(_nodes[_indices[item]]);
    }

    public int Count
    {
        get
        {
            return _indices.Count;
        }

    }

    public bool Empty()
    {
        return Count == 0;
    }

    public bool Contains(T node)
    {
        return _indices.ContainsKey(node);
    }

    public (T, int) Pop()
    {
        if (Empty()) {
            return (default(T), 0);
        }

        // Classes are reference types! This just makes `firstItem` 
        // point to the same object as items[0] does.
        AStarNode firstNode = _nodes[0];
        if (_nodes.Count == 1)
        {
            _nodes.Clear();
            _indices.Clear();
        }
        else
        {
            // Swap the first and last item, then sort down (reheap).
            Swap(_nodes[0], _nodes[_nodes.Count - 1]);
            _nodes.RemoveAt(_nodes.Count - 1);
            _indices.Remove(firstNode.Item);
            SortDown(_nodes[0]);
        }

        return (firstNode.Item, firstNode.Gcost);
    }

    // Assumes both items are already inserted.
    void Swap(AStarNode a, AStarNode b)
    {
        _nodes[_indices[a.Item]] = a;
        _nodes[_indices[b.Item]] = b;
        int itemAOldIndex = _indices[a.Item];
        _indices[a.Item] = _indices[b.Item];
        _indices[b.Item] = itemAOldIndex;
    }

    bool LeftChildHasPriority(AStarNode node)
    {
        int leftChildIndex = _indices[node.Item] * 2 + 1;
        return leftChildIndex < Count && node.CompareTo(_nodes[leftChildIndex]) < 0;
    }
    bool RightChildHasPriority(AStarNode node)
    {
        int rightChildIndex = _indices[node.Item] * 2 + 2;
        return rightChildIndex < Count && node.CompareTo(_nodes[rightChildIndex]) < 0;
    }

    // Returns true if a child had higher priority and swapped places with the
    // input item.
    bool SwapWithHighestPriorityChild(AStarNode node)
    {
        int leftChildIndex = _indices[node.Item] * 2 + 1;
        int rightChildIndex = leftChildIndex + 1;
        if (leftChildIndex < Count)
        {
            int swapIndex = leftChildIndex;
            // If both children exist, see which one has a lower value ("higher priority")
            if (rightChildIndex < Count)
            {
                // Prioritize the left child if they are equal (ensures the
                // right child only exists if the left one does).
                if (_nodes[leftChildIndex].CompareTo(_nodes[rightChildIndex]) < 0)
                {
                    swapIndex = rightChildIndex;
                }
            }
            if (node.CompareTo(_nodes[swapIndex]) < 0)
            {
                Swap(node, _nodes[swapIndex]);
                return true;
            }
        }
        return false;
    }

    void SortDown(AStarNode node)
    {
        bool try_sorting_down = true;
        while (try_sorting_down)
        {
            try_sorting_down = SwapWithHighestPriorityChild(node);
        }
    }

    void SortUp(AStarNode node)
    {
        UnityEngine.Debug.Log("[Heap] In SortUp!");

        if (_indices[node.Item] == 0)
        {
            UnityEngine.Debug.Log("[Heap] Returning early cause index is 0 (it's the first element)!");
            return;
        }

        int parentIndex = (_indices[node.Item] - 1) / 2;
        UnityEngine.Debug.Log("[Heap] _indices[node.Item] = " + _indices[node.Item] + ", parentIndex = " + parentIndex);
        // This stuff below was an issue when we click to go on another
        // tile, but it was fixed after adding the zero check above.
        // Move it up until we find its home :)
        while (_indices[node.Item] > 0 && node.CompareTo(_nodes[parentIndex]) > 0)
        {
            Swap(node, _nodes[parentIndex]);
            parentIndex = (_indices[node.Item] - 1) / 2;
            /*UnityEngine.Debug.Log("Returning early! _indices[node.Item] = " + _indices[node.Item] + ", parentIndex = " + parentIndex);
            return;*/
        }
        UnityEngine.Debug.Log("[Heap] Exiting SortUp!");
    }
}