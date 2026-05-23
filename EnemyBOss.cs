using UnityEngine;

public class FinalBoss : EnemyBase
{
    [Header("Configuración de Movimiento")]
    public float entrySpeed = 6f;
    public float hoverSpeed = 2f;
    [Tooltip("Qué tan a los costados se mueve. BAJALO si se sale de la pantalla")]
    public float hoverWidth = 3f;
    [Tooltip("El punto exacto donde el jefe frena y se queda flotando (Ej: 6 o 7)")]
    public float targetZPosition = 6f;

    [Header("Mecánica de Fases")]
    public float phase2Threshold = 0.5f;
    public float phase2FireRate = 0.4f;
    private bool isPhase2 = false;

    protected override void Start()
    {
        base.Start();

        isEntering = true;

        if (rb != null) rb.isKinematic = true;
    }

    protected override void MovePattern()
    {
        if (isEntering)
        {
            Vector3 targetPos = new Vector3(0f, transform.position.y, targetZPosition);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, entrySpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                isEntering = false;
            }
        }
        else
        {
            float newX = Mathf.Sin(Time.time * hoverSpeed) * hoverWidth;

            newX = Mathf.Clamp(newX, -limiteX, limiteX);

            transform.position = new Vector3(newX, transform.position.y, targetZPosition);
        }
    }

    protected override void HandleShooting()
    {
        if (!CanShoot()) return;

        if (!isPhase2 && currentHealth <= maxHealth * phase2Threshold)
        {
            isPhase2 = true;
            fireRate = phase2FireRate;
            Debug.Log("¡JEFE EN FASE 2!");
        }

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
}
