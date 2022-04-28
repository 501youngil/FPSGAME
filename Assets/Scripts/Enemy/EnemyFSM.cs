using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{    
    // ���ʹ� ���� ���
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    // ���ʹ� ���� ����
    EnemyState enemyState;
    // �÷��̾� ���� ������Ʈ
    public GameObject player;
    // ĳ���� ��Ʈ�ѷ�
    private CharacterController cc;
    // �ʱ� ��ġ, ȸ�� ���� ����
    Vector3 originPos;
    Quaternion originRot;
    // �÷��̾� ���� ����
    public float findDistance = 8.0f;
    // �̵� �ӵ� ����
    public float moveSpeed = 5.0f;
    // ���� ���� ���� 
    public float attackDistance = 2.0f;
    // ���� ������ �ð� ����
    public float attackDelayTime = 2.0f;
    // ���� ���� �ð� ����
    private float currentTime = 0f;
    // ���ݷ� ����
    public int attackPower = 2;
    // �̵� ������ �Ÿ�
    public float moveDistance = 20.0f;
    // �ִ� ü�� ����
    public int maxHp = 5;
    // ���� ü�� ����
    private int currentHp;
    // �����̴� UI
    public Slider hpSlider;
    // �ִϸ����� ����
    Animator anim;
    // �׺���̼� �޽� ������Ʈ 
    NavMeshAgent smith;

    private void Start()
    {
        // �ʱ� ���´� Idle
        enemyState = EnemyState.Idle;
        // �÷��̾� �˻�
        player = GameObject.Find("Player");
        // ĳ���� ��Ʈ�ѷ� ����
        cc = GetComponent<CharacterController>();
        // �ʱ� ��ġ�� ȸ�� ����
        originPos = transform.position;
        originRot = transform.rotation;
        // ���� ü�� ����
        currentHp = maxHp;
        // �ڽ� ������Ʈ���� �ִϸ����� ���� ����
        anim = GetComponentInChildren<Animator>();
        // �׺�޽�������Ʈ ������Ʈ ����
        smith = GetComponent<NavMeshAgent>();
        smith.speed = moveSpeed;
        smith.stoppingDistance = attackDistance;
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }

        // hp �����̴� ���� ü�� ���� ����
        hpSlider.value = (float)currentHp / (float)maxHp;
    }

    void Idle() // ��� ��
    {
        // �÷��̾���� �Ÿ��� �������� �̳��̸� Move ���·� ��ȯ
        if(Vector3.Distance(player.transform.position, transform.position) <= findDistance)
        {
            enemyState = EnemyState.Move;
            print("���� ��ȯ : Idle -> Move");
            // Move �ִϸ��̼����� ��ȯ
            anim.SetTrigger("IdleToMove");            
        }
    }
    void Move() // �̵� �Լ�
    {
        // �̵� �Ÿ� ���̸�
        if(Vector3.Distance(originPos, transform.position) > moveDistance)
        {
            // ���� ���¸� Return ���·� ��ȯ
            enemyState = EnemyState.Return;
            print("���� ��ȯ : Move -> Return");
        }
        // ���� ���� ���̸� �÷��̾ ���� �̵�
        else if (Vector3.Distance(player.transform.position, transform.position) > attackDistance)
        {
            // �̵� ���� ���ϱ�
            // Vector3 dir = (player.transform.position - this.transform.position).normalized;

            // ĳ���� ��Ʈ�ѷ��� �̵��������� �̵�
            // cc.Move(dir * moveSpeed * Time.deltaTime);

            // �÷��̾ ���� ���� ��ȯ
            // transform.forward = dir;

            // �׺�޽� ������Ʈ�� �̿��� Ÿ�� �������� �̵�
            smith.SetDestination(player.transform.position);
            smith.stoppingDistance = attackDistance;
        }
        // ���� ���� ���̸� ���� ���¸� Attack ���·� ��ȯ
        else
        {
            enemyState = EnemyState.Attack;
            print("���� ��ȯ : Move -> Attack");

            anim.SetTrigger("MoveToAttackDelay");

            // ���� �ð��� ���� ������ �ð���ŭ �̸� ���� (ù ������ �ٷ�)
            currentTime = attackDelayTime;

            // �̵��� ���߰�, Ÿ���� �ʱ�ȭ
            smith.isStopped = true;
            smith.ResetPath();
        }        
    }
    void Attack() // ���� �Լ�
    {
        // �÷��̾ ���� ���� �̳��� ������ �÷��̾� ����
        if(Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
        {
            // ���� �ð� ���� �÷��̾� ����
            if (currentTime >= attackDelayTime)
            {
                currentTime = 0;
                print("����");                

                anim.SetTrigger("StartAttack");
            }
            else
            {
                // �ð��� ����
                currentTime += Time.deltaTime;
            }
        }
        else
        {
            // �÷��̾ ���� ���� �̳��� ������ Move ���·� ��ȯ
            enemyState = EnemyState.Move;
            print("���� ��ȯ : Attack -> Move");

            anim.SetTrigger("AttackToMove");
        }        
    }    

    // �÷��̾�� �������� �ִ� �Լ�
    public void HitEvent()
    {
        PlayerMove pm = player.GetComponent<PlayerMove>();
        pm.OnDamage(attackPower);
    }

    void Return() // ���� �Լ�
    {
        // �ʱ� ��ġ�� ���� �������� �ʱ� ��ġ�� �̵�
        if (Vector3.Distance(originPos, transform.position) > 0.1f)
        {
            //Vector3 dir = (originPos - this.transform.position).normalized;
            //transform.forward = dir;
            //cc.Move(dir * moveSpeed * Time.deltaTime);
            smith.SetDestination(originPos);
            smith.stoppingDistance = 0f;
        }
        // �ʱ� ��ġ�� �����ϸ� Idle ���·� ��ȯ
        else
        {
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;

            enemyState = EnemyState.Idle;
            print("���� ��ȯ : Return -> Idle");
            anim.SetTrigger("MoveToIdle");

            // hp�� �ٽ� ȸ��
            currentHp = maxHp;
        }        
    }
    void Damaged() // �ǰ� �Լ�
    {
        // �ڷ�ƾ �Լ� ����
        StartCoroutine(DamageProcess());
    }
    IEnumerator DamageProcess()
    {
        // 2�ʰ� ����
        yield return new WaitForSeconds(1.0f);

        // 2�ʵ� Move ���·� ��ȯ
        enemyState = EnemyState.Move;
        print("���� ��ȯ : Damaged -> Move");
    }

    void Die()
    {
        // �������� �ڷ�ƾ �Լ��� ���� 
        StopAllCoroutines();

        // ��� �ڷ�ƾ �Լ� ����
        StartCoroutine(DieProcess());       
    }
    IEnumerator DieProcess()
    {
        // ĳ���� ��Ʈ�ѷ� ��Ȱ��ȭ
        cc.enabled = false;

        // 2�ʰ� ��ٷȴٰ� ����
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    // ������ ó�� �Լ�
    public void HitEnemy(int value)
    {
        // ���� ���°� �ǰ�, ����, ��� �����϶� �Լ��� ����
        if(enemyState == EnemyState.Damaged || enemyState == EnemyState.Return || enemyState == EnemyState.Die)
        {
            return;
        }

        // �÷��̾��� ���ݷ� ��ŭ ���ʹ� ü���� ���ҽ�Ŵ
        currentHp -= value;
        // ���̰��̼� �̵��� ���߰� ��� �ʱ�ȭ
        smith.isStopped = true;
        smith.ResetPath();

        // ���� hp�� 0���� ũ�� Damaged ���·� ��ȯ
        if (currentHp > 0)
        {
            enemyState = EnemyState.Damaged;
            print("���� ��ȯ : Any state -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        // �ƴϸ� Die ���·� ��ȯ
        else
        {
            enemyState = EnemyState.Die;
            print("���� ��ȯ : Any state -> Die");
            anim.SetTrigger("Die");
            Die();            
        }        
    }
} 