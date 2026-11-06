using System;
using System.IO;

namespace YMH219_FlappyBird
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver
    }

    public class GameManager
    {
        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public GameState State { get; private set; }

        private readonly string highScoreFilePath = "highscore.txt";

        public GameManager()
        {
            State = GameState.WaitingToStart;
            Score = 0;
            LoadHighScore();
        }

        public void StartGame()
        {
            Score = 0;
            State = GameState.Playing;
        }

        public void AddScore()
        {
            Score++;
            if (Score > HighScore)
            {
                HighScore = Score;
                SaveHighScore();
            }
        }

        public void EndGame()
        {
            State = GameState.GameOver;
            if (Score > HighScore)
            {
                HighScore = Score;
                SaveHighScore();
            }
        }

        public void ResetGame()
        {
            Score = 0;
            State = GameState.WaitingToStart;
        }

        private void LoadHighScore()
        {
            try
            {
                if (File.Exists(highScoreFilePath))
                {
                    string text = File.ReadAllText(highScoreFilePath).Trim();
                    if (int.TryParse(text, out int val))
                    {
                        HighScore = val;
                    }
                }
            }
            catch
            {
                HighScore = 0;
            }
        }

        private void SaveHighScore()
        {
            try
            {
                File.WriteAllText(highScoreFilePath, HighScore.ToString());
            }
            catch
            {
                
            }
        }
    }
}