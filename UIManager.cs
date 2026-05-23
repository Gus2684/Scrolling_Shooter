using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public PlayerController player;
    public Slider playerHealthSlider;
    public Slider playerAmmoSlider;

    [Header("Referencias del Jefe")]
    public Slider bossHealthSlider;
    private EnemyBase bossRef;

    [Header("Paneles de la Interfaz (UI)")]
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject victoryPanel;

    private bool isPaused = false;
    private bool gameEnded = false;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (bossHealthSlider != null) bossHealthSlider.gameObject.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!gameEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused) return;

        if (player == null || player.currentHealth <= 0)
        {
            if (!gameEnded) TriggerGameOver();
            HandleInputEndGame();
            return;
        }

        if (gameEnded)
        {
            HandleInputEndGame();
            return;
        }

        UpdatePlayerUI();
        UpdateBossUI();
    }

    void UpdatePlayerUI()
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = player.maxHealth;
            playerHealthSlider.value = player.currentHealth;
        }

        if (playerAmmoSlider != null)
        {
            playerAmmoSlider.maxValue = player.maxAmmo;
            playerAmmoSlider.value = player.currentAmmo;
        }
    }

    void UpdateBossUI()
    {
        if (bossRef == null)
        {
            GameObject bossObj = GameObject.FindGameObjectWithTag("Boss");
            if (bossObj != null)
            {
                bossRef = bossObj.GetComponent<EnemyBase>();
                bossHealthSlider.gameObject.SetActive(true);
            }
        }
        else
        {
            bossHealthSlider.maxValue = bossRef.maxHealth;
            bossHealthSlider.value = bossRef.currentHealth;

            if (bossRef.currentHealth <= 0)
            {
                bossHealthSlider.gameObject.SetActive(false);
                TriggerVictory();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
        }
    }

    void TriggerGameOver()
    {
        gameEnded = true;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    void TriggerVictory()
    {
        gameEnded = true;
        Time.timeScale = 0.2f;
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    void HandleInputEndGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
