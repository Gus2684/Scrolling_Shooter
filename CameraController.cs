using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform player;

    [Header("Posicionamiento")]
    [Tooltip("Distancia relativa al jugador. Y es la altura, Z es qué tan atrás está.")]
    public Vector3 offset = new Vector3(0f, 20f, -5f);

    [Header("Comportamiento")]
    [Tooltip("Si está activo, la cámara no seguirá al jugador a los costados.")]
    public bool lockXAxis = true;
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position + offset;

        if (lockXAxis)
        {
            targetPosition.x = 0f;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
