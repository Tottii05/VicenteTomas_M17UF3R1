using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EnemyController : MonoBehaviour, IDamageable
{
    public int HP;
    public GameObject target;
    public bool OnVisionRange = false, OnAttackRange = false;
    public Pathfinding _chaseB;
    public StateSO currentNode;
    public List<StateSO> Nodes;
    public float AttackRange = 0.5f;
    public EnemyFOV EnemyFOV;
    public Pathfinding Pathfinding;
    public Animator animator;
    public BoxCollider enemyDmgSource;
    public SphereCollider attackRangeCollider;
    private Vector3 previousPosition;

    public void OnEnable()
    {
        PlayerBehaviour.PlayerDead += OnPlayerDead;
    }

    private void OnPlayerDead()
    {
        OnAttackRange = false;
        OnVisionRange = false;
    }

    public void OnDisable()
    {
        PlayerBehaviour.PlayerDead -= OnPlayerDead;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        EnemyFOV = GetComponent<EnemyFOV>();
        Pathfinding = GetComponent<Pathfinding>();
        _chaseB = GetComponent<Pathfinding>();
        previousPosition = transform.position;

        attackRangeCollider = gameObject.AddComponent<SphereCollider>();
        attackRangeCollider.isTrigger = true;
        attackRangeCollider.radius = AttackRange;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (EnemyFOV.CheckPlayerInVision(other.gameObject))
            {
                Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);
                bool isPlayerInFront = dotProduct > 0;

                if (isPlayerInFront)
                {
                    OnAttackRange = attackRangeCollider.bounds.Contains(other.transform.position);
                }
                else
                {
                    OnAttackRange = false;
                }
                CheckEndingConditions();
            }
            else
            {
                OnAttackRange = false;
                CheckEndingConditions();
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnVisionRange = false;
            OnAttackRange = false;
            target = null;
            _chaseB.target = null;
            CheckEndingConditions();
        }
    }

    private void Update()
    {
        if (target != null && (!target.activeInHierarchy || target == null))
        {
            OnVisionRange = false;
            OnAttackRange = false;
            target = null;
            _chaseB.target = null;
            CheckEndingConditions();
        }

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

    public void EnableDamageSource()
    {
        enemyDmgSource.enabled = true;
    }

    public void DisableDamageSource()
    {
        enemyDmgSource.enabled = false;
    }
}