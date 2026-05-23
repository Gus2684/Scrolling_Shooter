using UnityEngine;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [Header("Estadísticas Base")]
    public int maxHealth = 30;
    public int currentHealth;
    public float baseSpeed = 5f;
    [Tooltip("Límite lateral máximo para la barrera")]
    public float limiteX = 4.5f;

    [Header("Interfaz (UI) Local")]
    [Tooltip("Arrastra aquí el Slider hijo de este enemigo (Dejar vacío en el Jefe)")]
    public Slider localHealthBar;

    [Header("Ataque")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    protected float nextFireTime;

    [Header("Recompensas (Power Ups)")]
    public GameObject[] powerUpPrefabs;
    [Range(0f, 1f)] public float dropChance = 0.2f;

    [HideInInspector] public bool isEntering = true;

    protected Transform player;
    protected Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;

        nextFireTime = Time.time + Random.Range(0.1f, 0.4f);

        if (localHealthBar != null)
        {
            localHealthBar.maxValue = maxHealth;
            localHealthBar.value = currentHealth;
        }
    }

    protected virtual void Update()
    {
        if (player != null && firePoint != null) firePoint.LookAt(player.position);
        HandleShooting();
    }

    protected virtual void FixedUpdate()
    {
        MovePattern();
    }

    protected virtual void MovePattern() { }

    protected virtual bool CanShoot()
    {
        if (player == null) return false;
        PlayerController pScript = player.GetComponent<PlayerController>();
        return pScript != null && pScript.isControllable && !isEntering;
    }

    protected virtual void HandleShooting()
    {
        if (!CanShoot()) return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    protected virtual void Shoot()
    {
        if (enemyBulletPrefab != null && firePoint != null)
        {
            Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (isEntering) return;

        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (localHealthBar != null)
        {
            localHealthBar.value = currentHealth;
        }

        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        if (powerUpPrefabs != null && powerUpPrefabs.Length > 0 && Random.value <= dropChance)
        {
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            Instantiate(powerUpPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
