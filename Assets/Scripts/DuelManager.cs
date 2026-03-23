using UnityEngine;

public class DuelManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _gruntPrefab;

    [SerializeField] private Transform _playerSpawn;
    [SerializeField] private Transform _enemySpawn;

    private void Start()
    {
        SpawnCombatants();
    }

    private void SpawnCombatants()
    {
        GameObject player = Instantiate(_playerPrefab, _playerSpawn.position, _playerSpawn.rotation);
        GameObject enemy = Instantiate(_gruntPrefab, _enemySpawn.position, _enemySpawn.rotation);

        enemy.GetComponent<Grunt>().SetPlayer(player.transform);
    }
}
