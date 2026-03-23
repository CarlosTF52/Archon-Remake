using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int z;

    public void Setup(int gridX, int gridZ)
    {
        x = gridX;
        z = gridZ;
    }

    private void OnMouseDown()
    {
        FindObjectOfType<GridManager>().MovePlayerToTile(x, z);
    }
}
