using System;
using UnityEngine;

namespace ColorfulBlocks.Scripts
{
    public class Block
    {
        public string Name;
        private Sprite Sprite { get; set; }
        public BlockUI Instance { get; set; }
        public Vector2Int Position { get; set; }

        public event Action<Vector2Int> OnClick;

        public void Setup(BlockMap map)
        {
            Sprite = map.GetSprite(Name);
            Instance.Setup(Sprite);
            Instance.OnClick += ()=> OnClick?.Invoke(Position);
        }
    }
}