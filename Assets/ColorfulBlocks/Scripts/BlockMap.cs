using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorfulBlocks.Scripts
{
    [CreateAssetMenu(fileName = "Blocks", menuName = "Colorful Blocks", order = 0)]
    public class BlockMap : ScriptableObject
    {
        [SerializeField] private List<StringSpritePair> sprites = new ();
        private Dictionary<string, Sprite> _spriteDictionary = new ();
        public string[] Names { get; private set; } = Array.Empty<string>();
        
        public void Initialize()
        {
            _spriteDictionary = sprites.ToDictionary(p => p.name, p => p.sprite);
            Names = _spriteDictionary.Keys.ToArray();
        }

        public Sprite GetSprite(string spriteName)
        {
            return _spriteDictionary.GetValueOrDefault(spriteName);
        }
    }
    
    [System.Serializable]
    public class StringSpritePair
    {
        public string name;
        public Sprite sprite;
    }
}