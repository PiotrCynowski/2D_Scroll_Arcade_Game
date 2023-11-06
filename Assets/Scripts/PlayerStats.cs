using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Stats 
{
    public class PlayerStats : MonoBehaviour 
    {
        public static PlayerStats Instance;

        [SerializeField] private Text playerTextLastScore;
        [SerializeField] private Text playerTextScore;
        [SerializeField] private Text playerTextBestResult;
        [SerializeField] private Text playerTextLifes;
        [SerializeField] private Text timerText;
        [SerializeField] private GameObject panelStats;

        private bool isTimerActive;
        private bool isGameStarted;

        public delegate void OnGameOver(bool isGameStoped);
        public static event OnGameOver onGameOver;

        [Header("Player Settings")]
        [Range(15, 500)]
        public int timeForMissionInSeconds;
        [Range(1, 9)]
        public int playerStartLifes;

        #region attributes
        // TIMER
        private float _timer;

        public float Timer 
        {
            get 
            {
                if (_timer <= 0) 
                {
                    StartCoroutine(GameOver());
                }
                return _timer;
            }
            set 
            {
                _timer = value;
                string minutes = ((int)_timer / 60).ToString();
                string seconds = (_timer % 60).ToString("f2");
                timerText.text = minutes + ":" + seconds;
            }
        }

        // PLAYER STATS
        private int _playerScore;

        public int PlayerScore 
        {
            get 
            {
                return _playerScore;
            }
            set 
            {
                _playerScore = value;
                playerTextScore.text = "Score: " + _playerScore;
            }
        }

        private int _playerLastScore;

        public int PlayerLastScore 
        {
            get 
            {
                return _playerLastScore;
            }
            set 
            {
                _playerLastScore = value;
                playerTextLastScore.text = "Last Score: " + _playerLastScore;
            }
        }

        private int _playerBestResult;

        public int PlayerBestResult 
        {
            get 
            {
                return _playerBestResult;
            }
            set 
            {
                _playerBestResult = value;
                playerTextBestResult.text = "Best Score: " + _playerBestResult;
            }
        }

        private int _playerLifes;

        public int PlayerLifes 
        {
            get 
            {
                if (_playerLifes <= 1) 
                {
                    StartCoroutine(GameOver());
                }
                return _playerLifes;
            }
            set 
            {
                _playerLifes = value;
                playerTextLifes.text = "Lives: " + _playerLifes;
            }
        }
        #endregion

        public void StatsReset() 
        {
            PlayerLifes = playerStartLifes;
            Timer = timeForMissionInSeconds;

            PlayerLastScore = PlayerScore;

            PlayerScore = 0;
        }


        private void Awake() {
            if (Instance != null && Instance != this) 
            {
                Destroy(this);
            }
            else 
            {
                Instance = this;
            }
        }

        private void Start() 
        {
            if (PlayerPrefs.HasKey("LastScore")) 
            {
                PlayerLastScore = PlayerPrefs.GetInt("LastScore");
            }
            if (PlayerPrefs.HasKey("playerBestScore")) {
                PlayerBestResult = PlayerPrefs.GetInt("playerBestScore");
            }

            StatsReset();
        }

        private void FixedUpdate() 
        {
            if (isTimerActive) 
            {
                Timer -= Time.deltaTime;
            }

            if (Input.anyKey && !isGameStarted) //start game from stats menu
            {
                panelStats.SetActive(false);
                StatsReset();

                onGameOver(false);

                isTimerActive = true;
                isGameStarted = true;
            }
        }


        private IEnumerator GameOver() 
        {
            isTimerActive = false;

            onGameOver(true);

            PlayerPrefs.SetInt("LastScore", PlayerScore);

            if (PlayerScore > PlayerBestResult) 
            {
                PlayerPrefs.SetInt("playerBestScore", PlayerScore);
                PlayerBestResult = PlayerScore;
            }

            panelStats.SetActive(true);

            yield return new WaitForSeconds(2);

            isGameStarted = false;
        }
    }
}