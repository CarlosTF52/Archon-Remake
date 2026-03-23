using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _projectileSpeed = 5f;
    [SerializeField]
    private float _projectileLifeTime = 2f;
    [SerializeField]
    public int _damage;
    [SerializeField]
    private string _targetTag;
    private GameObject _owner;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, _projectileLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * _projectileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner) return;

        if (other.CompareTag(_targetTag))
        {
            Destroy(gameObject);
        }
    }

    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }


}
