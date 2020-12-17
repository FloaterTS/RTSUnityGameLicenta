using UnityEngine;

[CreateAssetMenu]
public class UnitStats : ScriptableObject
{
    public Team unitTeam;
    public string unitName;
    public int maxHealth;
    public float moveSpeed;
    public int carryCapactity;
    public float harvestSpeedMultiplier;

    [SerializeField] private UnitStats baseUnitStats = null;

    public void ResetStats()
    {
        if (baseUnitStats == null)
            return;
        unitName = baseUnitStats.unitName;
        maxHealth = baseUnitStats.maxHealth;
        moveSpeed = baseUnitStats.moveSpeed;
        harvestSpeedMultiplier = baseUnitStats.harvestSpeedMultiplier;
        carryCapactity = baseUnitStats.carryCapactity;
    }
}
