using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorfulBlocks.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int fallSpeed = 750;
        [SerializeField] private int seed = -1;
        [SerializeField] private int initialMoves = 5;
        [SerializeField] private Vector2Int gridSize = new(5, 6);
        [SerializeField] private Vector2Int blockSize = new(120, 120);
        [SerializeField] private BlockUI blockPrefab;
        [SerializeField] private BlockMap blockMap;
        [SerializeField] private GameObject blockContainer;
        [SerializeField] private CanvasGroup canvasGroup;

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
            var playSeed = seed;
            if (playSeed < 0)
                playSeed = Random.Range(0, int.MaxValue);
            _moves = initialMoves;
            _score = 0;
            _isGameOver = false;
            _grid.Generate(gridSize, blockMap, playSeed);

            UpdateUI();
            UpdateGrid();
        }

        private void UpdateUI()
        {
            movesText.text = _moves.ToString();
            scoreText.text = _score.ToString("N", Nfi);
            gameOverPanel.SetActive(_isGameOver);
        }

        private Vector2 GetTarget(Vector2Int pos)
        {
            var target = new Vector2(pos.x * blockSize.x, pos.y * (blockSize.y - 15));
            target += new Vector2(blockSize.x / 2f, blockSize.y / 2f);
            return target;
        }

        private void UpdateGrid()
        {
            // First pass: instantiate new blocks and group them by column
            var newBlocksByColumn = new Dictionary<int, List<(Vector2Int pos, Block block)>>();

            _grid.ForEach((pos, block) =>
            {
                if (block.Instance == null)
                {
                    block.Instance = Instantiate(blockPrefab, blockContainer.transform);
                    block.Setup(blockMap);
                    block.OnClick += Clicked;

                    var rt = block.Instance.transform as RectTransform;
                    rt.sizeDelta = blockSize;
                    rt.localScale = Vector3.zero;

                    if (!newBlocksByColumn.ContainsKey(pos.x))
                        newBlocksByColumn[pos.x] = new List<(Vector2Int, Block)>();
                    newBlocksByColumn[pos.x].Add((pos, block));
                }
                else
                {
                    // Existing blocks just move to their new position
                    var rt = block.Instance.transform as RectTransform;
                    rt.sizeDelta = blockSize;
                    rt.DOAnchorPos(GetTarget(pos), .5f).SetEase(Ease.OutBounce);
                }
            });

            // Second pass: animate new blocks per column as a single falling unit
            var topY = gridSize.y * (blockSize.y - 15) + blockSize.y;

            foreach (var (col, blocks) in newBlocksByColumn)
            {
                // Find the lowest block in this column (smallest y = bottom)
                var lowestTarget = float.MaxValue;
                foreach (var (pos, _) in blocks)
                    lowestTarget = Mathf.Min(lowestTarget, GetTarget(pos).y);

                // Offset so the bottom-most block spawns just above the grid top
                var spawnOffset = topY - lowestTarget;

                foreach (var (pos, block) in blocks)
                {
                    var target = GetTarget(pos);
                    var spawnPos = new Vector2(target.x, target.y + spawnOffset);

                    var rt = block.Instance.transform as RectTransform;
                    rt.anchoredPosition = spawnPos;

                    var distance = spawnOffset; // all blocks in column travel same distance
                    var duration = distance / fallSpeed;

                    var seq = DOTween.Sequence();
                    seq.Append(rt.DOScale(1.2f, .15f).SetEase(Ease.OutQuad));
                    seq.Append(rt.DOScale(1f, .1f).SetEase(Ease.InQuad));
                    seq.Join(rt.DOAnchorPos(target, duration).SetEase(Ease.OutBounce));
                }
            }
        }

        private void Clicked(Vector2Int pos)
        {
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
            canvasGroup.blocksRaycasts = false;
            
            _grid.Remove(positions);
            UpdateGrid();
            yield return new WaitForSeconds(.5f);
            
            _grid.Compress();
            UpdateGrid();
            yield return new WaitForSeconds(.5f);
            
            _grid.Refill();
            UpdateGrid();
            
            UpdateUI();
            
            canvasGroup.blocksRaycasts = true;
        }
    }
}