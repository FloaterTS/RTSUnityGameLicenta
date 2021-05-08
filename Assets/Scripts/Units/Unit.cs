using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum UnitState
{
    IDLE,
    MOVING,
    WORKING,
    ATTACKING
}

public class Unit : MonoBehaviour
{
    //Unit Stats
    public UnitStats unitStats;
    private float currentHealth;

    //Unit States
    [HideInInspector] public UnitState unitState;
    [HideInInspector] public Vector3 target;
    [HideInInspector] public Transform thingInHand;

    //Worker & Fighter
    [HideInInspector] public Worker worker;
    [HideInInspector] public Fighter fighter;

    //Components
    private Animator animator;
    private NavMeshAgent navAgent;
    private NavMeshObstacle navObstacle;
    private GameObject selectedArea = null;

    //Others
    private float minMovingVelocity = 0.6f;
    private bool isSelected = false;
    private bool immobile = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        navObstacle = GetComponent<NavMeshObstacle>();

        worker = GetComponent<Worker>();
        fighter = GetComponent<Fighter>();

        currentHealth = unitStats.maxHealth;
        navAgent.speed = unitStats.moveSpeed;

        unitState = UnitState.IDLE;

        if (unitStats.unitTeam == Team.PLAYER)
            selectedArea = transform.Find("Selected").gameObject;
    }

    private void Start()
    {
        GameManager.instance.activeUnits.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.instance.activeUnits.Remove(this);

        if (SelectionManager.instance.selectedUnits.Contains(this))
            SelectionManager.instance.selectedUnits.Remove(this);
    }

    private void Update()
    {
        CheckMovement();
    }

    private void CheckMovement()
    {
        if (Mathf.Abs(navAgent.velocity.x) > minMovingVelocity || Mathf.Abs(navAgent.velocity.z) > minMovingVelocity)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
            CheckIfIdle(); 
        }
    }

    private void CheckIfIdle()
    {
        if (unitState != UnitState.MOVING || !navAgent.enabled || (worker != null && worker.IsBusy()) || (fighter!= null && fighter.GetAttackTarget() != null))
            return;

        if (Vector3.Distance(transform.position, navAgent.destination) <= navAgent.stoppingDistance + 0.1f)
        {
            StopNavAgent();
            unitState = UnitState.IDLE;
            target = Vector3.zero;
        }
    }

    public void MoveToLocation(Vector3 targetPosition, bool attackMove = false)
    {
        StartCoroutine(MoveToLocationCo(targetPosition, attackMove));
    }

    public IEnumerator MoveToLocationCo(Vector3 targetPosition, bool attackMove = false)
    {
        if (target == targetPosition)
            yield break;

        target = targetPosition;

        if (fighter != null)
        {
            if (!attackMove)
                fighter.StopAttackMove();
            if (unitState == UnitState.ATTACKING || !navAgent.enabled)
                yield return StartCoroutine(fighter.StopAttackCo());
        }

        if (worker != null)
        {
            if (unitState == UnitState.WORKING || !navAgent.enabled)
                yield return StartCoroutine(worker.StopTaskCo()); 
        }
        
        yield return StartCoroutine(CheckIfImmobileCo());

        if ((targetPosition != target && !attackMove) || !navAgent.enabled)
            yield break;

        navAgent.SetDestination(targetPosition);

        unitState = UnitState.MOVING;
    }

    public IEnumerator CheckIfImmobileCo()
    {
        yield return null;
        while (immobile)
            yield return null;
    }

    public void SetImmobile(bool active)
    {
        immobile = active;
    }

    public void SetSelected(bool isSelected)
    {
        if (unitStats.unitTeam != Team.PLAYER)  //For Player Units Only
            return;

        if (selectedArea != null)
            selectedArea.SetActive(isSelected);

        this.isSelected = isSelected;
    }

    public void StopNavAgent()
    {
        if (navAgent.enabled)
        {
            navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;
        }
    }

    public void EnableNavAgent(bool isEnabled)
    {
        navAgent.enabled = isEnabled;
    }

    public void EnableNavObstacle(bool isEnabled)
    {
        navObstacle.enabled = isEnabled;
    }

    public void NavAgentToNavObstacle()
    {
        EnableNavAgent(false);
        EnableNavObstacle(true);
    }

    public bool IsNavObstacleEnabled()
    {
        return navObstacle.enabled;
    }

    public void ChangeUnitSpeed(UnitSpeed unitSpeed)
    {
        switch(unitSpeed)
        {
            case UnitSpeed.RUN: 
                navAgent.speed = unitStats.moveSpeed;
                break;
            case UnitSpeed.WALK:
                navAgent.speed = unitStats.moveSpeed * unitStats.walkSpeedMultiplier;
                break;
            case UnitSpeed.SPRINT:
                navAgent.speed = unitStats.moveSpeed * unitStats.sprintSpeedMultiplier;
                break;
            case UnitSpeed.CARRY_LIGHT:
                navAgent.speed = unitStats.moveSpeed * unitStats.carryLightSpeedMultiplier;
                break;
            case UnitSpeed.CARRY_HEAVY:
                navAgent.speed = unitStats.moveSpeed * unitStats.carryHeavySpeedMultiplier;
                break;
        }
    }

    public void StopAction()
    {
        StartCoroutine(StopActionCo());
    }

    private IEnumerator StopActionCo()
    {
        target = Vector3.zero;

        if (worker != null)
            yield return StartCoroutine(worker.StopWorkAction());

        if (fighter != null)
            fighter.StopAttackMove();

        StopNavAgent();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void SetCurrentHealth(float newHealth)
    {
        currentHealth = newHealth;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

}
