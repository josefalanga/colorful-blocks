using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace ColorfulBlocks.Scripts
{
    public class Grid
    {
        private Dictionary<Vector2Int, Block> _grid = new();
        private Vector2Int _gridSize;
        private BlockMap _map;

        public void Generate(Vector2Int gridSize, BlockMap map, int seed)
        {
            _gridSize = gridSize;
            _map = map;
            foreach (var kvp in _grid)
            {
               kvp.Value.Dispose(); 
            }
            _grid.Clear();

            var random = new Random(seed);
            var names = map.Names;

            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    var i = random.Next(0, names.Length);
                    _grid[pos] = new Block { Name = names[i], Position = pos };
                }
            }
        }

        public void ForEach(Action<Vector2Int, Block> action)
        {
            foreach (var kvp in _grid)
                action(kvp.Key, kvp.Value);
        }

        public (Vector2Int, Block)[] GetMatchingNeighbors(Vector2Int pos)
        {
            if (!_grid.TryGetValue(pos, out var origin)) return Array.Empty<(Vector2Int, Block)>();

            var directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            var results = new List<(Vector2Int, Block)>();

            queue.Enqueue(pos);
            visited.Add(pos);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                results.Add((current, _grid[current]));

                foreach (var dir in directions)
                {
                    var neighbor = current + dir;
                    if (!visited.Contains(neighbor) &&
                        _grid.TryGetValue(neighbor, out var block) &&
                        block.Name == origin.Name)
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return results.ToArray();
        }

        public void Remove(List<Vector2Int> positions)
        {
            foreach (var pos in positions)
            {
                if (_grid.TryGetValue(pos, out var block))
                {
                    block.Dispose();
                    _grid.Remove(pos);
                }
            }
        }

        public void Compress()
        {
            var columns = new Dictionary<int, List<Block>>();

            foreach (var block in _grid.Values)
            {
                if (!columns.ContainsKey(block.Position.x))
                    columns[block.Position.x] = new List<Block>();
                columns[block.Position.x].Add(block);
            }

            _grid.Clear();

            foreach (var (x, blocks) in columns)
            {
                blocks.Sort((a, b) => a.Position.y.CompareTo(b.Position.y));

                for (var y = 0; y < blocks.Count; y++)
                {
                    blocks[y].Position = new Vector2Int(x, y);
                    _grid[blocks[y].Position] = blocks[y];
                }
            }
        }

        public void Refill(int seed)
        {
            var random = new Random(seed);
            var names = _map.Names;

            for (var x = 0; x < _gridSize.x; x++)
            {
                for (var y = 0; y < _gridSize.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (!_grid.ContainsKey(pos))
                    {
                        var i = random.Next(0, names.Length);
                        _grid[pos] = new Block { Name = names[i], Position = pos };
                    }
                }
            }
        }
    }
}