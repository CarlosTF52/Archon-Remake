using UnityEngine;
using System.Collections.Generic;

public static class BattleData
{
    public static int PlayerX;
    public static int PlayerZ;

    public static List<Vector2Int> EnemyPositions = new List<Vector2Int>();

    public static bool Initialized = false;

    public static bool BattleFinished;
    public static bool PlayerWon;

    public static int EnemyX;
    public static int EnemyZ;

    public static string EnemyTag;
    public static string EnemyName;

    public static UnitClass PlayerUnitClass;
    public static UnitClass EnemyUnitClass;

}
