using UnityEngine;
using UnityEngine.UI;

namespace ColorfulBlocks.Scripts
{
    public class BlockUI : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image image;

        public void Setup(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}