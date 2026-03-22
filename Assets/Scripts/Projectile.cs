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
    private Grunt _enemy;
    
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


}
