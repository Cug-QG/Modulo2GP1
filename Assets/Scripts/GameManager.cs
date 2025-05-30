using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else { instance = this; }
    }

    [SerializeField] private float gameHP;

    private float points;
    [SerializeField] private float coins;
    private bool playing;

    private void Start()
    {
        UIManager.Instance.SetHP(gameHP);
        UIManager.Instance.SetCoins(coins);
        PauseGame();
    }

    private void Update()
    {
        if (!playing) { return; }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
            UIManager.Instance.TogglePauseMenu(true);
        }
    }
    public void DecreseHP(float dmg)
    {
        gameHP -= dmg;
        UIManager.Instance.SetHP(gameHP);
        if (gameHP <= 0) { GameOver(); }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        playing = true;
    }

    public void PauseGame()
    {
        playing = false;
        Time.timeScale = 0;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GameOver() 
    {
        PauseGame();
        UIManager.Instance.SetADPointsCounter(points);
        UIManager.Instance.ToggleGameOverMenu(true);
    }

    public void RestartGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddPoints(float value)
    {
        points += value;
        UIManager.Instance.SetPointsCounter(points);
    }

    public float GetCoins() { return coins; }

    public void AddCoins(float value)
    {
        coins += value;
        UIManager.Instance.SetCoins(coins);
    }

    public void Purchase(float value) 
    { 
        coins -= value; 
        UIManager.Instance.SetCoins(coins); }

    public bool GetGameStatus() { return playing; }
}
