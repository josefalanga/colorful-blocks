using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace ColorfulBlocks.Scripts
{
    public class Grid
    {
        private Dictionary<Vector2Int, Block> _grid = new();
        public void Generate(Vector2Int gridSize, BlockMap map, int seed)
        {
            _grid.Clear();
            var random = new Random(seed);
            var names = map.Names;
            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var i = random.Next(0, names.Length);
                    var block = new Block { Name = names[i] };
                    block.Setup(map);
                    _grid[new Vector2Int(x, y)] = block;
                } 
            }
        }
        
        public void ForEach(Action<Vector2Int, Block> action)
        {
            foreach (var kvp in _grid)
            {
                action(kvp.Key, kvp.Value);
            }
        }
    }
}