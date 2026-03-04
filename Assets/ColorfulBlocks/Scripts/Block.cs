using UnityEngine;

namespace ColorfulBlocks.Scripts
{
    public class Block
    {
        public string Name;
        public Sprite Sprite { get; private set; }
        public BlockUI Instance { get; set; }

        public void Setup(BlockMap map)
        {
            Sprite = map.GetSprite(Name);
        }
    }
}