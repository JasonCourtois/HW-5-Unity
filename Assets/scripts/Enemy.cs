using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D _rb;
    [Header("Collision")]
    [SerializeField] private Transform headLocationTransform;
    [SerializeField] private Vector3 wallCheckOffset = new Vector3(1f, 0f, 0f);
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    [SerializeField] private Vector3 ledgeCheckOffset = new Vector3(1f, 0f, 0f);
    [SerializeField] private Vector2 ledgeCheckSize = new Vector2(0.5f, 1f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    private Vector2 headLocation => headLocationTransform.position;
    private Vector2 _playerPosition => PlayerControl.PlayerPosition;
    
    // Attacking
    [SerializeField] private Fireball fireballPrefab;
    private float _fireRange = 5f;
    private float _attackCooldown = 5f;
    private float _attackTimer;
    private bool _isAttackReady => _attackTimer >= _attackCooldown;
    // Patrolling
    private float _patrolWalkSpeed = 1f;
    private bool _isFacingLeft => transform.localScale.x < 0;
    private int _directionModifier => _isFacingLeft ? -1 : 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (Random.Range(0f, 1f) > 0.5) {
            FlipEnemy();
        }
    }

    private void Update()
    {
        if (!_isAttackReady)
        {
            _attackTimer += Time.deltaTime;
        } 
        else if (_isAttackReady)
        {
            Vector2? attackDirection = GetAttackDirection();
            
            if (attackDirection != null)
            {
                Attack((Vector2) attackDirection);
            }
        }

        if (IsHittingWall() || IsNearLedge())
        {
            FlipEnemy();
        }
        _rb.linearVelocityX = _patrolWalkSpeed * _directionModifier;
    }

    private void Attack(Vector2 direction)
    {
        _attackTimer = 0f;
        Fireball fireball = Instantiate(fireballPrefab, headLocation, transform.rotation);
        fireball.SetDirection(direction);
    }

    private bool IsHittingWall()
    {
        Vector2 wallCheckPosition = GetWallCheckPosition();
        bool isHittingWall = Physics2D.OverlapBox(wallCheckPosition, wallCheckSize, 0f, groundLayer);
        return isHittingWall;
    }

    private bool IsNearLedge()
    {
        // If enemy is moving on the y axis, return false.
        if (_rb.linearVelocityY != 0) return false;
        Vector2 ledgeCheckPosition = GetLedgeCheckPosition();
        bool isHittingFloor = Physics2D.OverlapBox(ledgeCheckPosition, ledgeCheckSize, 0f, groundLayer);
        return !isHittingFloor;
    }

    private Vector2? GetAttackDirection()
    {
        // Only perform raycast if player is within chaseable range.
        if (Vector2.Distance(headLocation, _playerPosition) > _fireRange) return null;

        Vector2 enemyPosition = headLocation;
        Vector2 playerPosition = _playerPosition;
        Vector2 directionToPlayer = playerPosition - enemyPosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(
            enemyPosition,
            directionToPlayer / distanceToPlayer,
            distanceToPlayer,
            groundLayer | playerLayer
        );
        bool _wasPlayerHit = hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerLayer) != 0;
        if (_wasPlayerHit)
        {
            return directionToPlayer / distanceToPlayer;
        }
        else
        {
            return null;
        }
    }


    private Vector2 GetWallCheckPosition() => transform.position + new Vector3(wallCheckOffset.x * _directionModifier, wallCheckOffset.y, wallCheckOffset.z);

    private Vector2 GetLedgeCheckPosition() => transform.position + new Vector3(ledgeCheckOffset.x * _directionModifier, ledgeCheckOffset.y, ledgeCheckOffset.z);

    private void FlipEnemy()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 wallCheckPosition = GetWallCheckPosition();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(wallCheckPosition, wallCheckSize);

        Vector2 ledgeCheckPosition = GetLedgeCheckPosition();
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(ledgeCheckPosition, ledgeCheckSize);
    }
}