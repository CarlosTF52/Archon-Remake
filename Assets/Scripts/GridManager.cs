using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    private int _enemyX = 7;
    private int _enemyZ = 7;
    private GameObject _enemyUnit;
    [SerializeField]
    private TextMeshProUGUI _turnText;
    [SerializeField] private float _boardMoveSpeed = 4f;


    private void Start()
    {
        GenerateGrid();
        ResolveBattleResult();
    }

    private void GenerateGrid()
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
        _playerX = 0;
        _playerZ = 0;
        _playerUnit = SpawnUnit(_playerUnitPrefab, 0, 0, true);
        _occupied[0, 0] = _playerUnit;

        _enemyUnit = SpawnUnit(_enemyUnitPrefab, 7, 7, false);
        _occupied[7, 7] = _enemyUnit;

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
            if (occupant.CompareTag("Enemy"))
            {
                BattleData.PlayerX = _playerX;
                BattleData.PlayerZ = _playerZ;

                BattleData.EnemyX = x;
                BattleData.EnemyZ = z;

                BattleData.EnemyTag = occupant.tag;
                BattleData.EnemyName = occupant.name;

                SceneManager.LoadScene(_duelSceneName);
            }

            return;
        }

        StartCoroutine(PlayerMoveRoutine(x, z));

        Vector3 tilePosition = _tiles[x, z].transform.position;
        Vector3 targetPosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);
        StartCoroutine(MoveUnitToTile(_playerUnit, targetPosition));

        _occupied[_playerX, _playerZ] = null;

        _playerX = x;
        _playerZ = z;

        

        _occupied[_playerX, _playerZ] = _playerUnit;

        _playerTurn = false;
        UpdateTurnUI();


        StartCoroutine(EnemyTurnRoutine());
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
        if (!_playerTurn) return false;

        int distance = Mathf.Abs(x - _playerX) + Mathf.Abs(z - _playerZ);

        if (distance != 1) return false;

        GameObject occupant = _occupied[x, z];

        if (occupant != null && !occupant.CompareTag("Enemy"))
        {
            return false;
        }

        return true;
    }

    private void EnemyTurn()
    {
        if (_enemyUnit == null)
        {
            _playerTurn = true;
            
            UpdateTurnUI();
            return;
        }

        int newX = _enemyX;
        int newZ = _enemyZ;

        if (Mathf.Abs(_playerX - _enemyX) > Mathf.Abs(_playerZ - _enemyZ))
        {
            if (_playerX > _enemyX) newX++;
            else if (_playerX < _enemyX) newX--;
        }
        else
        {
            if (_playerZ > _enemyZ) newZ++;
            else if (_playerZ < _enemyZ) newZ--;
        }

        int distanceToPlayer = Mathf.Abs(newX - _playerX) + Mathf.Abs(newZ - _playerZ);

        if (distanceToPlayer == 0)
        {
            BattleData.PlayerX = _playerX;
            BattleData.PlayerZ = _playerZ;

            BattleData.EnemyX = _enemyX;
            BattleData.EnemyZ = _enemyZ;

            BattleData.EnemyTag = _enemyUnit.tag;
            BattleData.EnemyName = _enemyUnit.name;

            SceneManager.LoadScene(_duelSceneName);
            return;
        }

        if (_occupied[newX, newZ] == null)
        {
            StartCoroutine(EnemyMoveRoutine(newX, newZ));
        }
        else
        {
            _playerTurn = true;
            
            UpdateTurnUI();
        }

    }

    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        EnemyTurn();
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

        _occupied[_playerX, _playerZ] = _playerUnit;

        _playerTurn = false;
        
        UpdateTurnUI();

        yield return new WaitForSeconds(0.5f);

        EnemyTurn();
    }

    private IEnumerator EnemyMoveRoutine(int newX, int newZ)
    {
        Vector3 tilePosition = _tiles[newX, newZ].transform.position;
        Vector3 targetPosition = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);

        _occupied[_enemyX, _enemyZ] = null;

        yield return StartCoroutine(MoveUnitToTile(_enemyUnit, targetPosition));

        _enemyX = newX;
        _enemyZ = newZ;

        _occupied[_enemyX, _enemyZ] = _enemyUnit;

        _playerTurn = true;
        
        UpdateTurnUI();
    }
}

