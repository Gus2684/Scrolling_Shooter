using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 15;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);

        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
