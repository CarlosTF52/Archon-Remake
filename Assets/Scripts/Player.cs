using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private GameObject _projectile;
    [SerializeField]
    private Transform _firePoint;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool _fire;
    private float _fireCooldown = 0.3f;
    private float _nextFireTime;
    [SerializeField]
    private float _dashForce = 10f;
    [SerializeField]
    private float _dashDuration = 0.15f;
    [SerializeField]
    private float _dashCooldown = 1f;
    private bool _isDashing;
    private float _dashEndTime;
    private float _nextDashTime;
    private Vector3 _dashDirection;
    [SerializeField]
    private float _secondaryCooldown = 1.5f;

    private float _nextSecondaryTime;

    [SerializeField]
    private int _health = 5;

    [SerializeField]
    private GameObject _secondaryProjectile;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        if (_isDashing)
        {
            rb.velocity = _dashDirection * _dashForce;

            if (Time.time >= _dashEndTime)
            {
                _isDashing = false;
            }
        }
        else
        {
            rb.velocity = new Vector3(movement.x, 0f, movement.z) * _movementSpeed;
        }

        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 direction = targetPoint - transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Time.time >= _nextFireTime)
        {
                Shoot();
                _nextFireTime = Time.time + _fireCooldown;
        }
    }

    public void Dodge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Time.time < _nextDashTime) return;
        if (moveInput == Vector2.zero) return;

        _dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        _isDashing = true;
        _dashEndTime = Time.time + _dashDuration;
        _nextDashTime = Time.time + _dashCooldown;
    }

    public void SecondaryFire(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Time.time < _nextSecondaryTime) return;

        SecondaryShot();
        _nextSecondaryTime = Time.time + _secondaryCooldown;
    }

    private void Shoot()
    {
        //Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
        GameObject proj = Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
        proj.GetComponent<Projectile>().SetOwner(gameObject);
    }

    private void SecondaryShot()
    {
        Instantiate(_secondaryProjectile, _firePoint.position, _firePoint.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        _health--;
        Debug.Log("Player Health: " + _health);

        if (_health <= 0)
        {
            
            Invoke(nameof(RestartScene), 0.5f);
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
