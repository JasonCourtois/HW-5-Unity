using UnityEngine;

public class Fireball : MonoBehaviour
{
    private float _damage = 15;
    private float _speed = 2;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    public void SetDirection(Vector2 direction)
    {
        Rigidbody2D _rb = GetComponent<Rigidbody2D>();
        _rb.linearVelocity = new Vector2(direction.x * _speed, direction.y * _speed);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ensure that the collision occurred on the ground or player layer.
        int layer = collision.gameObject.layer;
        if (((groundLayer.value & (1 << layer)) == 0) && ((playerLayer.value & (1 << layer)) == 0))
            return;

        // Only deal damage if player was hit
        if (collision.CompareTag("Player"))
        {
            PlayerControl.Health -= _damage;
        }

        Destroy(gameObject);
    }
}
