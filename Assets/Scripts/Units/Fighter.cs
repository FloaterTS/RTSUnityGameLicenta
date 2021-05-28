using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class Fighter : MonoBehaviour
{
    private Unit unit;
    private Animator animator;

    private List<Team> enemyTeams;
    private List<Team> alliedTeams;

    private GameObject attackTarget;
    private Vector3 positionBeforeAttack;
    private bool attackCommand = false;


    private void Awake()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();

        InitTeams();
    }

    private void Start()
    {
        StartCoroutine(CheckSurroundingsCo());
        StartCoroutine(AttackMoveAreaCheckCo());
    }

    private void Update()
    {
        if (unit.unitState != UnitState.DEAD)
            CheckAttackTarget();
    }

    private void InitTeams()
    {
        if (unit.unitStats.unitTeam == Team.PLAYER)
            enemyTeams = new List<Team>() { Team.ENEMY1, Team.ENEMY2, Team.ENEMY3 };
        else
            enemyTeams = new List<Team>() { Team.PLAYER };

        alliedTeams = new List<Team>() { unit.unitStats.unitTeam };
    }

    private void CheckAttackTarget()
    {
        if (attackTarget == null || unit.unitState == UnitState.ATTACKING)
            return;

        float distanceFromTarget = Vector3.Distance(transform.position, attackTarget.transform.position);
        if (distanceFromTarget <= unit.unitStats.attackRange)
            StartAttack();
        else
            unit.MoveToLocation(attackTarget.transform.position, true);

        if(!attackCommand)
        {
            float distanceFromInitialSpot = Vector3.Distance(transform.position, positionBeforeAttack);
            if (distanceFromInitialSpot > unit.unitStats.chaseDistance || distanceFromTarget > unit.unitStats.chaseVision)
                StopChase(true);
        }
    }

    public void AttackCommand(GameObject target, bool activeAttack)
    {
        StartCoroutine(AttackComandCo(target, activeAttack));
    }

    private IEnumerator AttackComandCo(GameObject target, bool activeAttack)
    {
        if (unit.worker != null)
            yield return StartCoroutine(unit.worker.StopWorkActionCo());

        if (attackTarget == null && !activeAttack)
            positionBeforeAttack = transform.position;

        attackTarget = target;
        attackCommand = activeAttack;
        animator.SetBool("isFighting", true);
    }

    private void StartAttack()
    {
        if (unit.unitState == UnitState.ATTACKING || attackTarget == null)
            return;

        //unit.NavAgentToNavObstacle();

        StartCoroutine(PerformAttackCo());
    }

    private IEnumerator PerformAttackCo()
    {
        unit.SetImmobile(true);

        unit.unitState = UnitState.ATTACKING;
        transform.LookAt(new Vector3(attackTarget.transform.position.x, transform.position.y, attackTarget.transform.position.z));

        animator.SetTrigger("attack");

        yield return new WaitForSeconds(unit.unitStats.attackAnimationDuration);

        if (unit.unitState == UnitState.DEAD)
            yield break;

        Unit attackTargetUnit = null;
        if (attackTarget != null)
        {
            attackTargetUnit = attackTarget.GetComponent<Unit>();
            if (attackTargetUnit == null)
            {
                Debug.LogError("Attack target GameObject " + attackTarget + " doesn't have Unit script attached");
                yield break;
            }
        }

        if (attackTarget != null && attackTargetUnit.unitState != UnitState.DEAD && unit.unitState == UnitState.ATTACKING)
            attackTargetUnit.TakeDamage(unit.unitStats.attackDamage);
        else
            StopAttackAction();
            //yield return StartCoroutine(StopAttackAction());

        unit.unitState = UnitState.IDLE;

        unit.SetImmobile(false);
    }

    private void StopChase(bool returnToInitialPosition)
    {
        StopAttackMove();

        if (returnToInitialPosition)
            unit.MoveToLocation(positionBeforeAttack);
        else
            unit.StopNavAgent();
    }

    private List<Unit> GetUnitsNearby(float nearbyDistance, List<Team> fromTeams = null)
    {
        List<Unit> nearbyUnits = new List<Unit>();
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, nearbyDistance, LayerMask.GetMask("Interactable"));
        foreach (Collider other in nearbyColliders)
        {
            if (other.gameObject.CompareTag("Unit"))
            {
                Unit otherUnit = other.gameObject.GetComponent<Unit>();

                if (otherUnit == null)
                {
                    Debug.LogError("GameObject " + other.gameObject + " tagged as Unit doesn't have Unit script attached");
                    continue;
                }

                if (otherUnit.unitState == UnitState.DEAD)
                    continue;

                if(fromTeams == null || fromTeams.Contains(otherUnit.unitStats.unitTeam))
                    nearbyUnits.Add(otherUnit);
            }
        }

        if (nearbyUnits.Contains(unit)) 
            nearbyUnits.Remove(unit); // excluding self

        return nearbyUnits;
    }

    private IEnumerator CheckSurroundingsCo()
    {
        yield return new WaitForSeconds(unit.unitStats.checkSurroundingsRate);
        StartCoroutine(CheckSurroundingsCo());

        if (unit.unitState != UnitState.IDLE || attackTarget != null) // unit must not be moving and not have an attack target
            yield break;

        if (unit.worker != null && unit.worker.carriedResource.amount > 0)
            yield break;

        List<Unit> enemyUnitsNearby = GetUnitsNearby(unit.unitStats.checkSurroundingsDistance, enemyTeams);
        List<Unit> alliedUnitsNearby = GetUnitsNearby(unit.unitStats.alertDistance, alliedTeams);
        
        if (enemyUnitsNearby.Count == 0)
            yield break;

        Unit closestEnemyUnit = GetClosestUnit(enemyUnitsNearby);
        
        AttackCommand(closestEnemyUnit.gameObject, false);

        foreach(Unit alliedUnit in alliedUnitsNearby)
        {
            if (alliedUnit.fighter != null)
                alliedUnit.fighter.AttackCommand(closestEnemyUnit.gameObject, false);
        }
    }

    private IEnumerator AttackMoveAreaCheckCo()
    {
        yield return new WaitForSeconds(unit.unitStats.checkSurroundingsRate);
        StartCoroutine(AttackMoveAreaCheckCo());

        if (unit.unitState != UnitState.MOVING || attackTarget == null || attackCommand) //unit must be moving and passively attacking
            yield break;

        List<Unit> enemyUnitsNearby = GetUnitsNearby(unit.unitStats.checkSurroundingsDistance, enemyTeams);

        if (enemyUnitsNearby.Count == 0)
            yield break;

        Unit closestEnemyUnit = GetClosestUnit(enemyUnitsNearby);

        if (attackTarget != closestEnemyUnit.gameObject)
            AttackCommand(closestEnemyUnit.gameObject, false);
    }

    private Unit GetClosestUnit(List<Unit> units)
    {
        if (units.Count == 0)
            return null;

        Unit closestUnit = units[0];
        float minDistanceFromUnit = 9999f;
        foreach (Unit otherUnit in units)
        {
            float distanceFromUnit = Vector3.Distance(transform.position, otherUnit.transform.position);
            if (distanceFromUnit < minDistanceFromUnit)
            {
                minDistanceFromUnit = distanceFromUnit;
                closestUnit = otherUnit;
            }
        }
        return closestUnit;
    }

    public void StopAttackMove()
    {
        attackCommand = false;
        attackTarget = null;
        animator.SetBool("isFighting", false);
    }

    /*public IEnumerator StopAttackCo()
    {
        unit.unitState = UnitState.IDLE;
        yield return StartCoroutine(unit.NavObstacleToNavAgent());
    }*/

    /*public IEnumerator StopAttackAction()
    {
        StopAttackMove();
        yield return StartCoroutine(StopAttackCo());
    }*/

    public void StopAttackAction()
    {
        StopAttackMove();
    }

    public GameObject GetAttackTarget()
    {
        return attackTarget;
    }
}
