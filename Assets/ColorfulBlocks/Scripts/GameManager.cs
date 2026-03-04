using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorfulBlocks.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Button moveButton;
        [SerializeField] private TMP_Text movesText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private int initialMoves = 5;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button replayButton;

        private int _moves;
        private int _score;
        private bool _isGameOver;
        
        private static readonly NumberFormatInfo Nfi = new()
        {
            NumberGroupSeparator = ".",
            NumberDecimalDigits = 0
        };

        private void Start()
        {
            moveButton.onClick.AddListener(FakeMove);
            replayButton.onClick.AddListener(Reset);
            Reset();
        }

        private void FakeMove()
        {
            _moves--;
            _score+=10;
            if (_moves <= 0)
            {
                _isGameOver = true;
            }
            UpdateUI();
        }

        private void Reset()
        {
            _moves = initialMoves;
            _score = 0;
            _isGameOver = false;

            UpdateUI();
        }

        private void UpdateUI()
        {
            movesText.text = _moves.ToString();
            scoreText.text = _score.ToString("N", Nfi);

            gameOverPanel.SetActive(_isGameOver);
        }
    }
}
