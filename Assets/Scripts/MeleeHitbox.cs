using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private int _damage = 1;
    [SerializeField] private string _targetTag;

    private HashSet<GameObject> _hitTargets = new HashSet<GameObject>();

    private void OnEnable()
    {
        _hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hitTargets.Contains(other.gameObject)) return;
        if (!other.CompareTag(_targetTag)) return;

        Grunt grunt = other.GetComponent<Grunt>();
        if (grunt != null)
        {
            grunt.Damage(_damage);
            _hitTargets.Add(other.gameObject);
            return;
        }

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.Damage(_damage);
            _hitTargets.Add(other.gameObject);
            return;
        }

        MeleeEnemy meleeEnemy = other.GetComponent<MeleeEnemy>();
        if (meleeEnemy != null)
        {
            meleeEnemy.Damage(_damage);
            _hitTargets.Add(other.gameObject);
            return;
        }

        MeleePlayer meleePlayer = other.GetComponent<MeleePlayer>();
        if (meleePlayer != null)
        {
            meleePlayer.Damage(_damage);
            _hitTargets.Add(other.gameObject);
        }
    }
}