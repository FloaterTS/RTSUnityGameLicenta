using UnityEngine;

public enum UnitSpeed
{ 
    RUN,
    WALK,
    SPRINT,
    CARRY_LIGHT,
    CARRY_HEAVY
}

public enum UnitType
{
    VILLAGER
}

[CreateAssetMenu]
public class UnitStats : ScriptableObject
{
    public Team unitTeam;
    public UnitType unitType;
    public string unitName;
    public float maxHealth;
    public float moveSpeed;
    public float walkSpeedMultiplier;
    public float sprintSpeedMultiplier;
    public float carryLightSpeedMultiplier;
    public float carryHeavySpeedMultiplier;
    public float harvestSpeedMultiplier;
    public float resourceSearchDistance;
    public float checkSurroundingsDistance;
    public float checkSurroundingsRate;
    public float alertDistance;
    public float chaseDistance;
    public float chaseVision;
    public float attackRange;
    public float attackDamage;
    public float attackAnimationDuration;
    public float deathAnimationDuration;
    public float spawnDuration;
    public int numberOfAttackTypes;
    public int numberOfDeathTypes;
    public int carryCapactity;
    public ResourceCost unitCost;

    [SerializeField] private UnitStats baseUnitStats = null;

    public void ResetStats()
    {
        if (baseUnitStats == null)
            return;

        unitName = baseUnitStats.unitName;
        maxHealth = baseUnitStats.maxHealth;
        moveSpeed = baseUnitStats.moveSpeed;
        walkSpeedMultiplier = baseUnitStats.walkSpeedMultiplier;
        sprintSpeedMultiplier = baseUnitStats.sprintSpeedMultiplier;
        carryLightSpeedMultiplier = baseUnitStats.carryLightSpeedMultiplier;
        carryHeavySpeedMultiplier = baseUnitStats.carryHeavySpeedMultiplier;
        harvestSpeedMultiplier = baseUnitStats.harvestSpeedMultiplier;
        resourceSearchDistance = baseUnitStats.resourceSearchDistance;
        checkSurroundingsDistance = baseUnitStats.checkSurroundingsDistance;
        checkSurroundingsRate = baseUnitStats.checkSurroundingsRate;
        alertDistance = baseUnitStats.alertDistance;
        chaseDistance = baseUnitStats.chaseDistance;
        chaseVision = baseUnitStats.chaseVision;
        attackRange = baseUnitStats.attackRange;
        attackDamage = baseUnitStats.attackDamage;
        attackAnimationDuration = baseUnitStats.attackAnimationDuration;
        deathAnimationDuration = baseUnitStats.deathAnimationDuration;
        spawnDuration = baseUnitStats.spawnDuration;
        numberOfAttackTypes = baseUnitStats.numberOfAttackTypes;
        numberOfDeathTypes = baseUnitStats.numberOfDeathTypes;
        carryCapactity = baseUnitStats.carryCapactity;

        unitCost.foodCost = baseUnitStats.unitCost.foodCost;
        unitCost.woodCost = baseUnitStats.unitCost.woodCost;
        unitCost.goldCost = baseUnitStats.unitCost.goldCost;
    }
}
