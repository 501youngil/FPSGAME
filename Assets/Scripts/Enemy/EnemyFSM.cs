using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{    
    // 에너미 상태 상수
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    // 에너미 상태 변수
    EnemyState enemyState;
    // 플레이어 게임 오브젝트
    public GameObject player;
    // 캐릭터 컨트롤러
    private CharacterController cc;
    // 초기 위치, 회전 저장 변수
    Vector3 originPos;
    Quaternion originRot;
    // 플레이어 감지 범위
    public float findDistance = 8.0f;
    // 이동 속도 변수
    public float moveSpeed = 5.0f;
    // 공격 가능 범위 
    public float attackDistance = 2.0f;
    // 공격 딜레이 시간 변수
    public float attackDelayTime = 2.0f;
    // 현재 누적 시간 변수
    private float currentTime = 0f;
    // 공격력 변수
    public int attackPower = 2;
    // 이동 가능한 거리
    public float moveDistance = 20.0f;
    // 최대 체력 변수
    public int maxHp = 5;
    // 현재 체력 변수
    private int currentHp;
    // 슬라이더 UI
    public Slider hpSlider;
    // 애니메이터 변수
    Animator anim;
    // 네비게이션 메쉬 에이전트 
    NavMeshAgent smith;

    private void Start()
    {
        // 초기 상태는 Idle
        enemyState = EnemyState.Idle;
        // 플레이어 검색
        player = GameObject.Find("Player");
        // 캐릭터 컨트롤러 추출
        cc = GetComponent<CharacterController>();
        // 초기 위치와 회전 저장
        originPos = transform.position;
        originRot = transform.rotation;
        // 현재 체력 설정
        currentHp = maxHp;
        // 자식 오브젝트에서 애니메이터 변수 추출
        anim = GetComponentInChildren<Animator>();
        // 네비메쉬엔이전트 컨포넌트 추출
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

        // hp 슬라이더 값에 체력 비율 적용
        hpSlider.value = (float)currentHp / (float)maxHp;
    }

    void Idle() // 대기 수
    {
        // 플레이어와의 거리가 감지범위 이내이면 Move 상태로 전환
        if(Vector3.Distance(player.transform.position, transform.position) <= findDistance)
        {
            enemyState = EnemyState.Move;
            print("상태 전환 : Idle -> Move");
            // Move 애니메이션으로 전환
            anim.SetTrigger("IdleToMove");            
        }
    }
    void Move() // 이동 함수
    {
        // 이동 거리 밖이면
        if(Vector3.Distance(originPos, transform.position) > moveDistance)
        {
            // 현재 상태를 Return 상태로 전환
            enemyState = EnemyState.Return;
            print("상태 전환 : Move -> Return");
        }
        // 공격 범위 밖이면 플레이어를 향해 이동
        else if (Vector3.Distance(player.transform.position, transform.position) > attackDistance)
        {
            // 이동 방향 구하기
            // Vector3 dir = (player.transform.position - this.transform.position).normalized;

            // 캐릭터 컨트롤러로 이동방향으로 이동
            // cc.Move(dir * moveSpeed * Time.deltaTime);

            // 플레이어를 향해 방향 전환
            // transform.forward = dir;

            // 네비메쉬 에이전트를 이용해 타겟 방향으로 이동
            smith.SetDestination(player.transform.position);
            smith.stoppingDistance = attackDistance;
        }
        // 공격 범위 안이면 현재 상태를 Attack 상태로 전환
        else
        {
            enemyState = EnemyState.Attack;
            print("상태 전환 : Move -> Attack");

            anim.SetTrigger("MoveToAttackDelay");

            // 누적 시간을 공격 딜레이 시간만큼 미리 진행 (첫 공격은 바로)
            currentTime = attackDelayTime;

            // 이동을 멈추고, 타겟을 초기화
            smith.isStopped = true;
            smith.ResetPath();
        }        
    }
    void Attack() // 공격 함수
    {
        // 플레이어가 공격 범위 이내에 있으면 플레이어 공격
        if(Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
        {
            // 일정 시간 마다 플레이어 공격
            if (currentTime >= attackDelayTime)
            {
                currentTime = 0;
                print("공격");                

                anim.SetTrigger("StartAttack");
            }
            else
            {
                // 시간을 누적
                currentTime += Time.deltaTime;
            }
        }
        else
        {
            // 플레이어가 공격 범위 이내에 없으면 Move 상태로 전환
            enemyState = EnemyState.Move;
            print("상태 전환 : Attack -> Move");

            anim.SetTrigger("AttackToMove");
        }        
    }    

    // 플레이어에게 데미지를 주는 함수
    public void HitEvent()
    {
        PlayerMove pm = player.GetComponent<PlayerMove>();
        pm.OnDamage(attackPower);
    }

    void Return() // 복귀 함수
    {
        // 초기 위치에 도달 안했으면 초기 위치로 이동
        if (Vector3.Distance(originPos, transform.position) > 0.1f)
        {
            //Vector3 dir = (originPos - this.transform.position).normalized;
            //transform.forward = dir;
            //cc.Move(dir * moveSpeed * Time.deltaTime);
            smith.SetDestination(originPos);
            smith.stoppingDistance = 0f;
        }
        // 초기 위치에 도달하면 Idle 상태로 전환
        else
        {
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;

            enemyState = EnemyState.Idle;
            print("상태 전환 : Return -> Idle");
            anim.SetTrigger("MoveToIdle");

            // hp를 다시 회복
            currentHp = maxHp;
        }        
    }
    void Damaged() // 피격 함수
    {
        // 코루틴 함수 실행
        StartCoroutine(DamageProcess());
    }
    IEnumerator DamageProcess()
    {
        // 2초가 정지
        yield return new WaitForSeconds(1.0f);

        // 2초뒤 Move 상태로 전환
        enemyState = EnemyState.Move;
        print("상태 전환 : Damaged -> Move");
    }

    void Die()
    {
        // 진행중인 코루틴 함수를 중지 
        StopAllCoroutines();

        // 사망 코루틴 함수 실행
        StartCoroutine(DieProcess());       
    }
    IEnumerator DieProcess()
    {
        // 캐릭터 컨트롤러 비활성화
        cc.enabled = false;

        // 2초가 기다렸다가 제거
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    // 데미지 처리 함수
    public void HitEnemy(int value)
    {
        // 나의 상태가 피격, 복귀, 사망 상태일때 함수를 종료
        if(enemyState == EnemyState.Damaged || enemyState == EnemyState.Return || enemyState == EnemyState.Die)
        {
            return;
        }

        // 플레이어의 공격력 만큼 에너미 체력을 감소시킴
        currentHp -= value;
        // 네이게이션 이동을 멈추고 경로 초기화
        smith.isStopped = true;
        smith.ResetPath();

        // 남은 hp가 0보다 크면 Damaged 상태로 전환
        if (currentHp > 0)
        {
            enemyState = EnemyState.Damaged;
            print("상태 전환 : Any state -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        // 아니면 Die 상태로 전환
        else
        {
            enemyState = EnemyState.Die;
            print("상태 전환 : Any state -> Die");
            anim.SetTrigger("Die");
            Die();            
        }        
    }
} 