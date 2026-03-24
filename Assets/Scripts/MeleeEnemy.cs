using System.Collections;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private int _health = 3;

    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _attackRange = 1.5f;

    [SerializeField] private float _attackCooldown = 1f;
    private float _nextAttackTime;

    [SerializeField] private GameObject _meleeHitbox;
    [SerializeField] private float _attackDuration = 0.15f;

    private Transform _player;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (_meleeHitbox != null)
        {
            _meleeHitbox.SetActive(false);
        }
    }

    void Update()
    {
        if (_player == null) return;

        Vector3 direction = (_player.position - transform.position);
        direction.y = 0f;

        float distance = direction.magnitude;
        direction.Normalize();

        // Always face player
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        // Move if not in range
        if (distance > _attackRange)
        {
            rb.velocity = direction * _moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.zero;

            if (Time.time >= _nextAttackTime)
            {
                StartCoroutine(MeleeAttackRoutine());
                _nextAttackTime = Time.time + _attackCooldown;
            }
        }
    }

    public void SetPlayer(Transform player)
    {
        _player = player;
    }

    private IEnumerator MeleeAttackRoutine()
    {
        // small wind-up
        yield return new WaitForSeconds(0.05f);

        _meleeHitbox.SetActive(true);

        // small forward push
        rb.AddForce(transform.forward * 2f, ForceMode.Impulse);

        yield return new WaitForSeconds(_attackDuration);

        _meleeHitbox.SetActive(false);
    }

    public void Damage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            BattleData.BattleFinished = true;
            BattleData.PlayerWon = true;

            Destroy(gameObject);
        }
    }
}