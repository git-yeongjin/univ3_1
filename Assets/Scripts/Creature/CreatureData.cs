using UnityEngine;

public enum CreatureType
{
    none,
    Doll,
    Frog,
    Mushroom,
    SlimeHorse,
    Mammoth
}
public enum CreatureOrigin
{
    none,
    Pure,
    Mutated
}
public enum ItemData
{
    none,
}

[CreateAssetMenu(fileName = "CreatureSetting", menuName = "Creature/CreatureSetting")]
public class CreatureData : ScriptableObject
{
    public CreatureType type;
    public CreatureOrigin origin;
    public ItemData dropitem;
    public int UnLockDay;
    public bool isSpawning = false;
}