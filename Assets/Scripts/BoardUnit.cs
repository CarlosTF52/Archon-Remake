using UnityEngine;

public enum UnitTeam
{
    Player,
    Enemy
}

public enum UnitClass
{
    Ranged,
    Melee
}

public class BoardUnit : MonoBehaviour
{
    public UnitTeam Team;
    public UnitClass Class;
}