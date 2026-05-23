using UnityEngine;

public class EnemySquad : EnemyBase
{
    public enum SquadPattern { Circulo, FormacionV, OchoInfinito }

    [Header("Configuración del Escuadrón")]
    public SquadPattern patronDeVuelo = SquadPattern.Circulo;
    public float orbitRadius = 4f;
    public float orbitSpeed = 3f;

    [Header("Vaivén de todo el grupo")]
    public float waveSpeed = 1.5f;
    public float waveWidth = 5f;

    [Header("Límites de la Zona Superior")]
    [Tooltip("El punto más bajo al que pueden llegar (Ej: 3 o 4)")]
    public float limiteZInferior = 0f;
    [Tooltip("El borde superior de la pantalla (Ej: 10)")]
    public float limiteZSuperior = 10f;

    [HideInInspector] public int memberIndex = 0;
    [HideInInspector] public float spawnAnchorX;
    [HideInInspector] public float spawnAnchorZ;
    [HideInInspector] public float battleAnchorX;
    [HideInInspector] public float battleAnchorZ;

    private float currentAnchorX;
    private float currentAnchorZ;

    protected override void Start()
    {
        base.Start();
        isEntering = true;
        if (rb != null) rb.isKinematic = true;

        currentAnchorX = spawnAnchorX;
        currentAnchorZ = spawnAnchorZ;
    }

    protected override void MovePattern()
    {
        if (isEntering)
        {
            currentAnchorX = Mathf.MoveTowards(currentAnchorX, battleAnchorX, baseSpeed * Time.deltaTime);
            currentAnchorZ = Mathf.MoveTowards(currentAnchorZ, battleAnchorZ, baseSpeed * Time.deltaTime);

            float finalX = currentAnchorX;
            float finalZ = currentAnchorZ;

            ApplyPatternOffset(ref finalX, ref finalZ, 0f);

            transform.position = new Vector3(finalX, transform.position.y, finalZ);

            if (Mathf.Abs(currentAnchorX - battleAnchorX) < 0.1f && Mathf.Abs(currentAnchorZ - battleAnchorZ) < 0.1f)
            {
                isEntering = false;
            }
        }
        else
        {
            float time = Time.time;
            float swayX = Mathf.Sin(time * waveSpeed) * waveWidth;

            float finalX = battleAnchorX + swayX;
            float finalZ = battleAnchorZ;

            ApplyPatternOffset(ref finalX, ref finalZ, time);

            finalX = Mathf.Clamp(finalX, -limiteX, limiteX);
            finalZ = Mathf.Clamp(finalZ, limiteZInferior, limiteZSuperior);

            transform.position = new Vector3(finalX, transform.position.y, finalZ);
        }
    }


    void ApplyPatternOffset(ref float x, ref float z, float timeValue)
    {
        switch (patronDeVuelo)
        {
            case SquadPattern.Circulo:
                float angleOffset = memberIndex * (Mathf.PI * 2f / 5f);
                float currentAngle = (timeValue * orbitSpeed) + angleOffset;
                x += Mathf.Cos(currentAngle) * orbitRadius;
                z += Mathf.Sin(currentAngle) * orbitRadius;
                break;

            case SquadPattern.FormacionV:
                float espaciadoX = orbitRadius * 0.8f;
                float espaciadoZ = orbitRadius * 0.8f;

                if (memberIndex == 1) { x -= espaciadoX; z += espaciadoZ; }
                else if (memberIndex == 2) { x += espaciadoX; z += espaciadoZ; }
                else if (memberIndex == 3) { x -= espaciadoX * 2; z += espaciadoZ * 2; }
                else if (memberIndex == 4) { x += espaciadoX * 2; z += espaciadoZ * 2; }
                break;

            case SquadPattern.OchoInfinito:
                float angle8Offset = memberIndex * (Mathf.PI * 2f / 5f);
                float angle8 = (timeValue * orbitSpeed) + angle8Offset;
                x += Mathf.Sin(angle8) * (orbitRadius * 1.5f);
                z += Mathf.Sin(angle8 * 2f) * orbitRadius;
                break;
        }
    }

    protected override bool CanShoot()
    {
        return base.CanShoot() && !isEntering;
    }
}
