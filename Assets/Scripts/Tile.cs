using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int z;

    private Renderer _renderer;
    [SerializeField]
    private Color _originalColor;
    private GridManager _gridManager;

    public void Setup(int gridX, int gridZ)
    {
        x = gridX;
        z = gridZ;
    }

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
        _gridManager = FindObjectOfType<GridManager>();
    }

    private void OnMouseDown()
    {
        _gridManager.MovePlayerToTile(x, z);
    }

    private void OnMouseEnter()
    {
        if (_gridManager != null && _gridManager.IsValidMoveTile(x, z))
        {
            _renderer.material.color = Color.gray;
        }
    }

    private void OnMouseExit()
    {
        _renderer.material.color = _originalColor;
    }

}
