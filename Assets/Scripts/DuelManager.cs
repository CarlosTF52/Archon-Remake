using UnityEngine;

public class DuelManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerRangedPrefab;
    [SerializeField] private GameObject _playerMeleePrefab;

    [SerializeField] private GameObject _enemyRangedPrefab;
    [SerializeField] private GameObject _enemyMeleePrefab;

    [SerializeField] private Transform _playerSpawn;
    [SerializeField] private Transform _enemySpawn;

    private void Start()
    {
        SpawnCombatants();
    }

    private void SpawnCombatants()
    {
        GameObject playerPrefabToSpawn = null;
        GameObject enemyPrefabToSpawn = null;

        if (BattleData.PlayerUnitClass == UnitClass.Ranged)
        {
            playerPrefabToSpawn = _playerRangedPrefab;
        }
        else if (BattleData.PlayerUnitClass == UnitClass.Melee)
        {
            playerPrefabToSpawn = _playerMeleePrefab;
        }

        if (BattleData.EnemyUnitClass == UnitClass.Ranged)
        {
            enemyPrefabToSpawn = _enemyRangedPrefab;
        }
        else if (BattleData.EnemyUnitClass == UnitClass.Melee)
        {
            enemyPrefabToSpawn = _enemyMeleePrefab;
        }

        if (playerPrefabToSpawn == null || enemyPrefabToSpawn == null)
        {
            Debug.LogError("Duel prefab not assigned for one of the unit classes.");
            return;
        }
        GameObject player = Instantiate(playerPrefabToSpawn, _playerSpawn.position, _playerSpawn.rotation);
        GameObject enemy = Instantiate(enemyPrefabToSpawn, _enemySpawn.position, _enemySpawn.rotation);

        Grunt grunt = enemy.GetComponent<Grunt>();
        if (grunt != null)
        {
            grunt.SetPlayer(player.transform);
        }

        MeleeEnemy meleeEnemy = enemy.GetComponent<MeleeEnemy>();
        if (meleeEnemy != null)
        {
            meleeEnemy.SetPlayer(player.transform);
        }
    }
}