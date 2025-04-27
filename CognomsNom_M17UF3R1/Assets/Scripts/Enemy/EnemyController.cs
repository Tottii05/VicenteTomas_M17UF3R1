using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EnemyController : MonoBehaviour, IDamageable
{
    public int HP;
    public GameObject target;
    public bool OnVisionRange = false, OnAttackRange = false, runAway = false;
    public Pathfinding _chaseB;
    public StateSO currentNode;
    public List<StateSO> Nodes;
    public float AttackRange = 2f;
    public EnemyFOV EnemyFOV;
    public Pathfinding Pathfinding;
    public Animator animator;

    private Vector3 previousPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        EnemyFOV = GetComponent<EnemyFOV>();
        Pathfinding = GetComponent<Pathfinding>();
        _chaseB = GetComponent<Pathfinding>();
        previousPosition = transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;
            _chaseB.target = target;
            OnVisionRange = true;
            CheckEndingConditions();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (EnemyFOV.CheckPlayerInVision(other.gameObject))
            {
                OnAttackRange = Vector3.Distance(transform.position, other.transform.position) < AttackRange;
                CheckEndingConditions();
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnVisionRange = false;
            CheckEndingConditions();
        }
    }

    private void Update()
    {
        currentNode.OnStateUpdate(this);

        Vector3 currentPosition = transform.position;
        bool isMoving = Vector3.Distance(currentPosition, previousPosition) > 0.01f;
        animator.SetBool("walking", isMoving);
        previousPosition = currentPosition;
    }

    public void CheckEndingConditions()
    {
        foreach (ConditionSO condition in currentNode.EndConditions)
        {
            if (condition.CheckCondition(this) == condition.answer) ExitCurrentNode();
        }
    }

    public void ExitCurrentNode()
    {
        foreach (StateSO stateSO in Nodes)
        {
            if (stateSO.StartCondition == null || stateSO.StartCondition.CheckCondition(this) == stateSO.StartCondition.answer)
            {
                EnterNewState(stateSO);
                break;
            }
        }
        currentNode.OnStateEnter(this);
    }

    private void EnterNewState(StateSO state)
    {
        currentNode.OnStateExit(this);
        currentNode = state;
        currentNode.OnStateEnter(this);
    }

    public void TakeDamage(float damage)
    {
        HP -= (int)damage;
        animator.SetTrigger("hit");
        CheckEndingConditions();
    }
}
