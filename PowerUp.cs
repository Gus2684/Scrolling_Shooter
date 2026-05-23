using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    [Header("Tipo de Mejora")]
    [Tooltip("Escribir EXACTAMENTE: Multishot, Health o Ammo")]
    public string upgradeType;

    [Header("Efectos Visuales y Movimiento")]
    public float rotationSpeed = 100f;
    [Tooltip("Velocidad a la que el item cae hacia el jugador")]
    public float driftSpeed = 5f;
    public float lifetime = 10f;

    [Header("Efecto de Titileo (Blink)")]
    [Tooltip("¿Cada cuántos segundos cambia entre visible e invisible?")]
    public float blinkInterval = 0.2f;
    [Tooltip("Si está activo, titilará más rápido cuando esté a punto de desaparecer")]
    public bool blinkFasterAtEnd = true;

    private Renderer visualRenderer;
    private float blinkTimer;
    private float timeActive;

    void Start()
    {
        Destroy(gameObject, lifetime);

        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        visualRenderer = GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.back * driftSpeed * Time.deltaTime, Space.World);

        ExecuteBlinking();
    }

    void ExecuteBlinking()
    {
        if (visualRenderer == null) return;

        timeActive += Time.deltaTime;
        blinkTimer += Time.deltaTime;

        float currentInterval = blinkInterval;

        if (blinkFasterAtEnd && timeActive > (lifetime * 0.7f))
        {
            currentInterval = blinkInterval * 0.35f;
        }

        if (blinkTimer >= currentInterval)
        {
            visualRenderer.enabled = !visualRenderer.enabled;
            blinkTimer = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController playerScript = other.GetComponent<PlayerController>();

        if (playerScript == null)
        {
            playerScript = other.GetComponentInParent<PlayerController>();
        }

        if (playerScript != null)
        {
            Debug.Log("¡ÉXITO! Se entregó el item: " + upgradeType);
            playerScript.CollectUpgrade(upgradeType);
            Destroy(gameObject);
        }
    }
}