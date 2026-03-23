using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
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

        GameObject enemyUnit = SpawnUnit(_enemyUnitPrefab, 7, 7, false);
        _occupied[7, 7] = enemyUnit;
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

        Vector3 tilePosition = _tiles[x, z].transform.position;
        _playerUnit.transform.position = new Vector3(tilePosition.x, tilePosition.y, tilePosition.z);

        _occupied[_playerX, _playerZ] = null;

        _playerX = x;
        _playerZ = z;

        _occupied[_playerX, _playerZ] = _playerUnit;
    }
}

