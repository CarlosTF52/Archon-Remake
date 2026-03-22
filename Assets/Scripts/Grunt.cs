using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : MonoBehaviour
{
    public int Health;

    [SerializeField]
    private bool _canDamage = true;
    [SerializeField]
    private float _moveSpeed = 2f;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private GameObject _projectile;
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private float _fireCooldown = 3f;
    [SerializeField]
    private float _chaseRange = 8f;
    [SerializeField]
    private float _retreatRange = 4f;
    [SerializeField]
    private bool _strafeRight = true;

    private float _nextFireTime;

    private IEnumerator ResetDamage;

    [SerializeField]
    private float _strafeChangeInterval = 2f;

    private float _nextStrafeChangeTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time >= _nextStrafeChangeTime)
        {
            _strafeRight = Random.value > 0.5f;
            _nextStrafeChangeTime = Time.time + _strafeChangeInterval;
        }

        Vector3 toPlayer = (_player.position - transform.position).normalized;
        toPlayer.y = 0f;

        float distance = Vector3.Distance(transform.position, _player.position);

        Vector3 moveDirection = Vector3.zero;

        if (distance > _chaseRange)
        {
            moveDirection = toPlayer;
        }
        else if (distance > _retreatRange)
        {
            moveDirection = _strafeRight ? Vector3.Cross(toPlayer, Vector3.up) : -Vector3.Cross(toPlayer, Vector3.up);
        }
        else
        {
            moveDirection = -toPlayer;
        }

        transform.position += moveDirection * _moveSpeed * Time.deltaTime;

        if (toPlayer != Vector3.zero)
        {
            transform.forward = toPlayer;
        }

        if (Time.time >= _nextFireTime && distance <= _chaseRange)
        {
            Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
            _nextFireTime = Time.time + _fireCooldown;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ResetDamage = hitRoutine(1.0f);

        ResetDamage = hitRoutine(1.0f);

        if (other.CompareTag("Projectile") && _canDamage)
        {
            Projectile proj = other.GetComponent<Projectile>();

            if (proj != null)
            {
                Damage(proj._damage);
            }

            _canDamage = false;
            StartCoroutine(ResetDamage);
        }
    }

    public void Damage(int damage)
    {
        Debug.Log("Damage");

        Health = Health - damage;
        if (Health <= 1)
        {
            Destroy(this.gameObject);
        }

    }

    private IEnumerator hitRoutine(float cooldown)
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldown);
            _canDamage = true;
        }
    }
}
