using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Vida y Estado")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isControllable = false;
    public bool isDodging = false;

    [Header("Efectos de Daño")]
    [Tooltip("Arrastrá acá el modelo visual de la nave (el objeto hijo que tiene el Mesh/Material)")]
    public GameObject visualModel;
    public float invincibilityDuration = 1.5f;
    public float flashFrequency = 0.1f;
    private bool isInvincible = false;

    [Header("Munición y Mejoras")]
    public int maxAmmo = 100;
    public int currentAmmo = 100;
    [Tooltip("Nivel 1 = Disparo simple. Sube al agarrar power-ups.")]
    [Range(1, 5)] public int weaponLevel = 1;

    [Header("Movimiento")]
    public float speed = 15f;
    public float limiteX = 8f;
    public float limiteZAtras = -10f;
    public float limiteZAdelante = 10f;

    [Header("Animación")]
    public float tiltAmount = 25f;

    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform[] firePoints;
    public float fireRate = 0.15f;
    private float nextFireTime = 0f;

    private Rigidbody rb;
    private Vector3 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        currentAmmo = maxAmmo;

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        isControllable = false;

        Vector3 startPos = new Vector3(0, transform.position.y, limiteZAtras - 15f);
        Vector3 endPos = new Vector3(0, transform.position.y, limiteZAtras + 3f);

        transform.position = startPos;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isControllable = true;
    }

    void Update()
    {
        if (!isControllable || currentHealth <= 0) return;

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.z = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        if (!isControllable || currentHealth <= 0) return;

        if (!isDodging)
        {
            rb.linearVelocity = movementInput.normalized * speed;
        }

        Vector3 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -limiteX, limiteX);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, limiteZAtras, limiteZAdelante);
        rb.MovePosition(clampedPosition);

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, movementInput.x * -tiltAmount);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoints == null || currentAmmo <= 0) return;

        currentAmmo--;

        int cannonsToUse = Mathf.Min(weaponLevel, firePoints.Length);

        for (int i = 0; i < cannonsToUse; i++)
        {
            Instantiate(bulletPrefab, firePoints[i].position, firePoints[i].rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.TriggerShake(0.2f, 0.15f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFlashRoutine());
        }
    }

    private IEnumerator InvincibilityFlashRoutine()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibilityDuration)
        {
            if (visualModel != null)
            {
                visualModel.SetActive(!visualModel.activeSelf);
            }

            yield return new WaitForSeconds(flashFrequency);
            timer += flashFrequency;
        }

        if (visualModel != null)
        {
            visualModel.SetActive(true);
        }

        isInvincible = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public void CollectUpgrade(string upgradeType)
    {
        switch (upgradeType)
        {
            case "Health":
                currentHealth += 30;
                if (currentHealth > maxHealth) currentHealth = maxHealth;
                Debug.Log("¡Power-Up: Vida recuperada!");
                break;

            case "Ammo":
                currentAmmo += 50;
                if (currentAmmo > maxAmmo) currentAmmo = maxAmmo;
                Debug.Log("¡Power-Up: Munición recargada!");
                break;

            case "Multishot":
                if (weaponLevel < firePoints.Length)
                {
                    weaponLevel++;
                    Debug.Log("¡Power-Up: Armas mejoradas al Nivel " + weaponLevel + "!");
                }
                else
                {
                    Debug.Log("¡Armas al máximo!");
                }
                break;
        }
    }
}
