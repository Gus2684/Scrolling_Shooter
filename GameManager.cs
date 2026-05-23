using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;

    [Header("Pantallas Finales")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    private bool gameOver = false;

    [Header("Límites de Pantalla")]
    public float spawnDistanceZ = 160f;
    public float spawnRangeX = 85f;

    [Header("Zonas de Combate")]
    [Tooltip("El punto más bajo donde pueden frenar a pelear (Ej: 60)")]
    public float battleZoneMinZ = 60f;
    [Tooltip("El punto más alto donde pueden frenar a pelear (Ej: 90)")]
    public float battleZoneMaxZ = 90f;

    [Header("Reloj del Jefe (Timer)")]
    public float timeToBoss = 60f;

    private float gameTimer = 0f;
    private bool bossSpawned = false;
    private bool waitingForClear = false;

    void Start()
    {
        Time.timeScale = 1f;

        SpawnSquads(2);
        waitingForClear = true;
    }

    void Update()
    {
        if (gameOver) return;

        if (player == null)
        {
            TriggerDefeat();
            return;
        }

        if (bossSpawned)
        {
            if (GameObject.FindGameObjectWithTag("Boss") == null)
            {
                TriggerVictory();
            }

            return;
        }

        gameTimer += Time.deltaTime;

        if (gameTimer >= timeToBoss)
        {
            SpawnBoss();
            return;
        }

        if (waitingForClear)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length == 0)
            {
                waitingForClear = false;
                int squadsToSpawn = Random.Range(2, 5);
                SpawnSquads(squadsToSpawn);
                waitingForClear = true;
            }
        }
    }

    void SpawnBoss()
    {
        bossSpawned = true;
        GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in remainingEnemies) Destroy(enemy);

        if (bossPrefab != null)
        {
            float targetY = (player != null) ? player.position.y : 0f;
            Vector3 bossSpawnPos = new Vector3(0f, targetY, spawnDistanceZ + 10f);
            Instantiate(bossPrefab, bossSpawnPos, Quaternion.identity);
        }
    }

    void SpawnSquads(int numberOfSquads)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        for (int s = 0; s < numberOfSquads; s++)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            int enemiesInWave = 5;
            int entryDirection = Random.Range(0, 3);

            float sAnchorX = 0f;
            float sAnchorZ = 0f;

            float bAnchorX = Random.Range(-spawnRangeX + 10f, spawnRangeX - 10f);
            float bAnchorZ = Random.Range(battleZoneMinZ, battleZoneMaxZ);

            if (entryDirection == 0) { sAnchorX = bAnchorX; sAnchorZ = spawnDistanceZ; }
            else if (entryDirection == 1) { sAnchorX = -spawnRangeX - 20f; sAnchorZ = bAnchorZ; }
            else { sAnchorX = spawnRangeX + 20f; sAnchorZ = bAnchorZ; }

            for (int i = 0; i < enemiesInWave; i++)
            {
                float targetY = (player != null) ? player.position.y : 0f;
                Vector3 spawnPos = new Vector3(sAnchorX, targetY, sAnchorZ);

                GameObject enemyObj = Instantiate(enemyPrefabs[randomIndex], spawnPos, Quaternion.identity);
                enemyObj.tag = "Enemy";

                EnemySquad squadScript = enemyObj.GetComponent<EnemySquad>();
                if (squadScript != null)
                {
                    squadScript.memberIndex = i;
                    squadScript.spawnAnchorX = sAnchorX;
                    squadScript.spawnAnchorZ = sAnchorZ;
                    squadScript.battleAnchorX = bAnchorX;
                    squadScript.battleAnchorZ = bAnchorZ;
                }
            }
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TriggerDefeat()
    {
        if (gameOver) return;
        gameOver = true;

        if (defeatPanel != null) defeatPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TriggerVictory()
    {
        if (gameOver) return;
        gameOver = true;

        if (victoryPanel != null) victoryPanel.SetActive(true);
    }
}