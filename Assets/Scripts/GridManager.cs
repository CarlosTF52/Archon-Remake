using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class EnemyData
{
    public GameObject unit;
    public int x;
    public int z;
}

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _tilePrefab;
    [SerializeField]
    private int _width = 8;
    [SerializeField]
    private int _height = 8;
    [SerializeField]
    private float _tileSize = 1f;
    [SerializeField]
    private Material _whiteMaterial;
    [SerializeField]
    private Material _blackMaterial;
    private GameObject[,] _tiles;
    [SerializeField]
    private GameObject _playerUnitPrefab;
    [SerializeField]
    private GameObject _enemyUnitPrefab;
    private GameObject _playerUnit;
    private int _playerX;
    private int _playerZ;
    private GameObject[,] _occupied;
    [SerializeField]
    private string _duelSceneName = "SampleScene";
    private bool _playerTurn = true;
    [SerializeField]
    private TextMeshProUGUI _turnText;
    [SerializeField] private float _boardMoveSpeed = 4f;

    private List<EnemyData> _enemies = new List<EnemyData>();


    private void Start()
    {
        if (!BattleData.Initialized)
        {
            InitializeBoardData();
        }

        GenerateGridFromData();
        ResolveBattleResult();
    }

    private void InitializeBoardData()
    {
        BattleData.PlayerX = 0;
        BattleData.PlayerZ = 0;

        BattleData.EnemyPositions.Clear();
        BattleData.EnemyPositions.Add(new Vector2Int(7, 7));
        BattleData.EnemyPositions.Add(new Vector2Int(6, 7));
        BattleData.EnemyPositions.Add(new Vector2Int(7, 6));

        BattleData.Initialized = true;
    }

    private void GenerateGridFromData()
    {
        _turnText.color = _playerTurn ? Color.white : Color.red;
        _occupied = new GameObject[_width, _height];
        _tiles = new GameObject[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                Vector3 spawnPosition = new Vector3((x - _width / 2f + 0.5f) * _tileSize, (z - _height / 2f + 0.5f) * _tileSize, 0f);

                GameObject tile = Instantiate(_tilePrefab, spawnPosition, Quaternion.identity, transform);

                Renderer tileRenderer = tile.GetComponent<Renderer>();

                if ((x + z) % 2 == 0)
                {
                    tileRenderer.material = _whiteMaterial;
                }
                else
                {
                    tileRenderer.material = _blackMaterial;
                }
                Tile tileScript = tile.GetComponent<Tile>();
                tileScript.Setup(x, z);
                _tiles[x, z] = tile;
            }
        }


        // Spawn player
        _playerX = BattleData.PlayerX;
        _playerZ = BattleData.PlayerZ;

        _playerUnit = SpawnUnit(_playerUnitPrefab, _playerX, _playerZ, true);
        _occupied[_playerX, _playerZ] = _playerUnit;

        // Spawn enemies
        _enemies.Clear();

        foreach (Vector2Int pos in BattleData.EnemyPositions)
        {
            SpawnEnemy(pos.x, pos.y);
        }

        UpdateTurnUI();

    }

    private GameObject SpawnUnit(GameObject unitPrefab, int x, int z, bool isPlayer)
    {
        Vector3 tilePosition = _tiles[x, z].transform.position;
        Vector3 spawnPosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);

        Quaternion rotation;

        if (isPlayer)
        {
            rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else
        {
            rotation = Quaternion.Euler(0f, -90f, 0f);
        }

        return Instantiate(unitPrefab, spawnPosition, rotation);
    }

    public void MovePlayerToTile(int x, int z)
    {
        if (!_playerTurn) return;

        int distance = Mathf.Abs(x - _playerX) + Mathf.Abs(z - _playerZ);
        if (distance != 1) return;

        GameObject occupant = _occupied[x, z];

        if (occupant != null)
        {
            BoardUnit unit = occupant.GetComponent<BoardUnit>();

            if (unit != null && unit.Team == UnitTeam.Enemy)
            {
                BattleData.PlayerX = _playerX;
                BattleData.PlayerZ = _playerZ;

                BattleData.EnemyX = x;
                BattleData.EnemyZ = z;

                BattleData.EnemyTag = occupant.tag;
                BattleData.EnemyName = occupant.name;

                BoardUnit playerBoardUnit = _playerUnit.GetComponent<BoardUnit>();
                BoardUnit enemyBoardUnit = occupant.GetComponent<BoardUnit>();

                BattleData.PlayerUnitClass = playerBoardUnit.Class;
                BattleData.EnemyUnitClass = enemyBoardUnit.Class;

                SceneManager.LoadScene(_duelSceneName);
            }

            return;
        }

        StartCoroutine(PlayerMoveRoutine(x, z));
    }

    private void ResolveBattleResult()
    {
        if (!BattleData.BattleFinished) return;

        if (BattleData.PlayerWon)
        {
            GameObject enemy = _occupied[BattleData.EnemyX, BattleData.EnemyZ];

            if (enemy != null)
            {
                Destroy(enemy);
                _occupied[BattleData.EnemyX, BattleData.EnemyZ] = null;

                for (int i = _enemies.Count - 1; i >= 0; i--)
                {
                    if (_enemies[i].x == BattleData.EnemyX && _enemies[i].z == BattleData.EnemyZ)
                    {
                        _enemies.RemoveAt(i);
                        break;
                    }
                }

                BattleData.EnemyPositions.Remove(new Vector2Int(BattleData.EnemyX, BattleData.EnemyZ));
            }
        }
        else
        {
            if (_playerUnit != null)
            {
                Destroy(_playerUnit);
                _occupied[BattleData.PlayerX, BattleData.PlayerZ] = null;
            }
        }

        BattleData.BattleFinished = false;
    }

    public bool IsValidMoveTile(int x, int z)
    {
        if (_occupied == null) return false;

        int distance = Mathf.Abs(x - _playerX) + Mathf.Abs(z - _playerZ);

        if (distance != 1) return false;

        GameObject occupant = _occupied[x, z];

        if (occupant != null)
        {
            BoardUnit unit = occupant.GetComponent<BoardUnit>();
            if (unit != null && unit.Team != UnitTeam.Enemy)
            {
                return false;
            }
        }

        return true;
    }

    private void EnemyTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private void UpdateTurnUI()
    {
        if (_playerTurn)
        {
            _turnText.text = "Player Turn";
        }
        else
        {
            _turnText.text = "Enemy Turn";
        }
    }

    private IEnumerator MoveUnitToTile(GameObject unit, Vector3 targetPosition)
    {
        while (Vector3.Distance(unit.transform.position, targetPosition) > 0.01f)
        {
            unit.transform.position = Vector3.MoveTowards(
                unit.transform.position,
                targetPosition,
                _boardMoveSpeed * Time.deltaTime
            );

            yield return null;
        }

        unit.transform.position = targetPosition;
    }

    private IEnumerator PlayerMoveRoutine(int x, int z)
    {
        Vector3 tilePosition = _tiles[x, z].transform.position;
        Vector3 targetPosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);

        _occupied[_playerX, _playerZ] = null;

        yield return StartCoroutine(MoveUnitToTile(_playerUnit, targetPosition));

        _playerX = x;
        _playerZ = z;

        BattleData.PlayerX = _playerX;
        BattleData.PlayerZ = _playerZ;

        _occupied[_playerX, _playerZ] = _playerUnit;

        _playerTurn = false;
        UpdateTurnUI();

        yield return new WaitForSeconds(0.5f);

        EnemyTurn();
    }

    private IEnumerator EnemyTurnRoutine()
    {
        if (_enemies.Count == 0)
        {
            _playerTurn = true;
            UpdateTurnUI();
            yield break;
        }

        for (int i = 0; i < _enemies.Count; i++)
        {
            EnemyData enemy = _enemies[i];

            if (enemy.unit == null) continue;

            int oldX = enemy.x;
            int oldZ = enemy.z;

            int newX = enemy.x;
            int newZ = enemy.z;

            if (Mathf.Abs(_playerX - enemy.x) > Mathf.Abs(_playerZ - enemy.z))
            {
                if (_playerX > enemy.x) newX++;
                else if (_playerX < enemy.x) newX--;
            }
            else
            {
                if (_playerZ > enemy.z) newZ++;
                else if (_playerZ < enemy.z) newZ--;
            }

            int distanceToPlayer = Mathf.Abs(newX - _playerX) + Mathf.Abs(newZ - _playerZ);

            if (distanceToPlayer == 0)
            {
                BattleData.PlayerX = _playerX;
                BattleData.PlayerZ = _playerZ;

                BattleData.EnemyX = enemy.x;
                BattleData.EnemyZ = enemy.z;

                BattleData.EnemyTag = enemy.unit.tag;
                BattleData.EnemyName = enemy.unit.name;

                SceneManager.LoadScene(_duelSceneName);
                yield break;
            }

            if (newX >= 0 && newX < _width && newZ >= 0 && newZ < _height && _occupied[newX, newZ] == null)
            {
                Vector3 tilePosition = _tiles[newX, newZ].transform.position;
                Vector3 targetPosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);

                _occupied[oldX, oldZ] = null;

                yield return StartCoroutine(MoveUnitToTile(enemy.unit, targetPosition));

                enemy.x = newX;
                enemy.z = newZ;

                _occupied[enemy.x, enemy.z] = enemy.unit;

                for (int j = 0; j < BattleData.EnemyPositions.Count; j++)
                {
                    if (BattleData.EnemyPositions[j].x == oldX && BattleData.EnemyPositions[j].y == oldZ)
                    {
                        BattleData.EnemyPositions[j] = new Vector2Int(newX, newZ);
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }

        _playerTurn = true;
        UpdateTurnUI();
    }

    private void SpawnEnemy(int x, int z)
    {
        GameObject enemyUnit = SpawnUnit(_enemyUnitPrefab, x, z, false);
        _occupied[x, z] = enemyUnit;

        EnemyData enemyData = new EnemyData();
        enemyData.unit = enemyUnit;
        enemyData.x = x;
        enemyData.z = z;

        _enemies.Add(enemyData);
    }
}

