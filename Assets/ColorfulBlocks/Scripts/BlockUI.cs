using System;
using UnityEngine;
using UnityEngine.UI;

namespace ColorfulBlocks.Scripts
{
    public class BlockUI : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image image;
        public event Action OnClick;

        public void Setup(Sprite sprite)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(()=> OnClick?.Invoke());
            image.sprite = sprite;
        }
    }
}