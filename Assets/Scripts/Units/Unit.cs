using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    //Unit Stats
    public UnitStats unitStats;
    private float currentHealth;

    //Unit States
    [HideInInspector] public UnitState unitState;
    [HideInInspector] public Vector3 target;

    //Worker
    [HideInInspector] public Worker worker;

    //Components
    private Animator animator;
    private NavMeshAgent navAgent;
    private GameObject selectedArea = null;

    //Others
    private float minMovingVelocity = 0.6f;
    private bool isSelected = false;


    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        worker = GetComponent<Worker>();

        currentHealth = unitStats.maxHealth;
        navAgent.speed = unitStats.moveSpeed;

        if (unitStats.unitTeam == Team.Player)
            selectedArea = transform.Find("Selected").gameObject;

        unitState = UnitState.idle;
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

    public void MoveToLocation(Vector3 targetPosition)
    {
        StartCoroutine(MoveToLocationCo(targetPosition));
    }

    public IEnumerator MoveToLocationCo(Vector3 targetPosition)
    {
        if (target == targetPosition)
            yield break;

        target = targetPosition;

        if (worker != null)
        {
            if (unitState == UnitState.working)
                yield return StartCoroutine(worker.StopTaskCo());
            yield return StartCoroutine(worker.CheckIfImmobileCo());
            if (targetPosition != target)
                yield break;
        }
        navAgent.isStopped = false;
        navAgent.SetDestination(targetPosition);

        unitState = UnitState.moving;
    }

    void CheckIfIdle()
    {
        if (unitState != UnitState.moving || !navAgent.enabled || (worker != null && worker.IsBusy()))
            return;
        //Do I need the distance condition?
        if (navAgent.hasPath && Vector3.Distance(transform.position, navAgent.destination) <= navAgent.stoppingDistance + 0.1f) 
        {
            StopNavAgent();
            unitState = UnitState.idle;
            target = Vector3.zero;
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (unitStats.unitTeam != Team.Player)  //For Player Units Only
            return;

        if (selectedArea != null)
            selectedArea.SetActive(isSelected);

        this.isSelected = isSelected;
    }

    public void StopNavAgent()
    {
        if (navAgent.enabled)
        {
            //navAgent.isStopped = true;
            navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;
        }
    }

    public void EnableNavAgent(bool isEnabled)
    {
        navAgent.enabled = isEnabled;
    }

    public void ChangeUnitSpeed(UnitSpeed unitSpeed)
    {
        switch(unitSpeed)
        {
            case UnitSpeed.run: 
                navAgent.speed = unitStats.moveSpeed;
                break;
            case UnitSpeed.walk:
                navAgent.speed = unitStats.moveSpeed * unitStats.walkSpeedMultiplier;
                break;
            case UnitSpeed.sprint:
                navAgent.speed = unitStats.moveSpeed * unitStats.sprintSpeedMultiplier;
                break;
            case UnitSpeed.carryLight:
                navAgent.speed = unitStats.moveSpeed * unitStats.carryLightSpeedMultiplier;
                break;
            case UnitSpeed.carryHeavy:
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

        StopNavAgent();
    }

}
