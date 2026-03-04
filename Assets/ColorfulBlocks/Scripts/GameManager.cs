using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorfulBlocks.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int seed = -1;
        [SerializeField] private int initialMoves = 5;
        [SerializeField] private Vector2Int gridSize = new(5, 6);
        [SerializeField] private Vector2Int blockSize = new(120, 120);
        [SerializeField] private BlockUI blockPrefab;
        [SerializeField] private BlockMap blockMap;
        [SerializeField] private GameObject blockContainer;

        [SerializeField] private TMP_Text movesText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button replayButton;

        private int _moves;
        private int _score;
        private bool _isGameOver;
        private Grid _grid;

        private static readonly NumberFormatInfo Nfi = new()
        {
            NumberGroupSeparator = ".",
            NumberDecimalDigits = 0
        };

        private void Start()
        {
            _grid = new Grid();
            blockMap.Initialize();
            replayButton.onClick.AddListener(Reset);
            Reset();
        }

        private void Reset()
        {
            if (seed < 0)
                seed = Random.Range(0, int.MaxValue);
            _moves = initialMoves;
            _score = 0;
            _isGameOver = false;
            _grid.Generate(gridSize, blockMap, seed);

            UpdateUI();
        }

        private void UpdateUI()
        {
            UpdateGrid();

            movesText.text = _moves.ToString();
            scoreText.text = _score.ToString("N", Nfi);

            gameOverPanel.SetActive(_isGameOver);
        }

        private void UpdateGrid()
        {
            _grid.ForEach((pos, block) =>
            {
                if (block.Instance == null)
                {
                    block.Instance = Instantiate(blockPrefab, blockContainer.transform);
                    block.Setup(blockMap);
                    block.OnClick += Clicked;
                }
                
                block.Position = pos;
                var rt = block.Instance.transform as RectTransform;
                rt.anchoredPosition = new Vector2(pos.x * blockSize.x, pos.y * (blockSize.y - 15));
                rt.anchoredPosition += new Vector2(blockSize.x / 2f, blockSize.y / 2f);
                rt.sizeDelta = blockSize;
            });
        }

        private void Clicked(Vector2Int pos)
        {
            //TODO: disable UI to prevent interaction while wait happens
            _moves--;
            if (_moves <= 0)
            {
                _isGameOver = true;
            }
            var neighbors = _grid.GetMatchingNeighbors(pos);
            var positions = new List<Vector2Int>();
            _score += neighbors.Length;
            
            foreach (var neighbor in neighbors)
            {
                positions.Add(neighbor.Item1);
            }
            
            StartCoroutine(ClickedCoroutine(positions));
            
        }

        private IEnumerator ClickedCoroutine(List<Vector2Int> positions)
        {
            _grid.Remove(positions);
            UpdateGrid();
            yield return new WaitForSeconds(1);
            
            _grid.Compress();
            UpdateGrid();
            yield return new WaitForSeconds(1);
            
            _grid.Refill(seed);
            UpdateGrid();
            
            yield return new WaitForSeconds(1);
            UpdateUI();
        }
    }
}