using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    // 이동 속도 변수
    public float moveSpeed = 7f;
    // 중력 변수
    public float gravity = -20f;
    // 수직 속력 변수
    public float yVelocity = 0f;
    // 점프력 변수
    public float jumpPower = 10f;
    // 점프 상태 변수
    private bool isJumping = false;
    // 체력 변수
    public int hp;
    // 최대 체력
    public int maxHp = 10;

    // 슬라이더 UI
    public Slider hpSlider;

    // 캐릭터 컨트롤러 변수
    CharacterController cc;

    // 피격 이펙트 UI 오브젝트
    public GameObject hitEffect;

    // 애니메이터 컴포넌트 변수
    private Animator anim;

    private void Start()
    {
        Cursor.visible = false; // 마우스 커서를 보이지 않게
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 위치 고정      

        // 캐릭터 컨트롤러 컨포넌트 받아오기
        cc = GetComponent<CharacterController>();

        // 체력 변수 초기화
        hp = maxHp;

        // 자식 오브젝트에서 애니메이터 변수 추출
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // 슬라이더 value를 체력 비율로 적용
        hpSlider.value = (float)hp / (float)maxHp;

        // 게임 상태가 게임 중 상태가 아니면 함수 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // 키보드 입력을 받음
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 이동 방향 설정
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        // 이동 방향 벡터에 크기값을 애니메이터의 이동 블랜드 트리에 전달
        anim.SetFloat("MoveDirection", dir.magnitude);

        // 이동 방향(월드좌표)을 카메라의 방향(로컬좌표) 기준으로 전환
        dir = Camera.main.transform.TransformDirection(dir); // transform.TransformDirection : 변환할 월드 벡터

        // 점프 중이였고, 다시 바닥에 착지했으면
        if(cc.collisionFlags == CollisionFlags.Below) // cc.collisionFlags 캐릭터 컨트롤러 충돌체의 충돌 부위 체크
        {
            // 점프 중이면
            if (isJumping)
            {
                // 점프 전 상태로 초기화
                isJumping = false;                
            }
            // 캐릭터의 수직 속도를 0으로 
            yVelocity = 0f;
        }
        // 스페이스바 누르면 점프,  점프 하지 않은 상태면
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            // 캐릭터 수직 속도에 점프력 적용
            yVelocity = jumpPower;
            isJumping = true;
        }

        //  캐릭터의 수직속도(중력) 적용
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        // 이동 방향으로 플레이어 이동 , 충돌여부 체크
        cc.Move(dir * moveSpeed * Time.deltaTime);        
    }

    // 플레이어 피격 함수
    public void OnDamage(int value)
    {
        hp -= value;
        if(hp < 0)
        {
            hp = 0;
        }
        // hp가 0보다 큰 경우 피격 이펙트 코루틴 실행
        else
        {
            StartCoroutine(HitEffect());
        }
    }

    IEnumerator HitEffect()
    {
        // 피격 UI 활성화
        hitEffect.SetActive(true);
        // 0.3초 대기
        yield return new WaitForSeconds(0.3f);
        // 피격 UI 비활성화
        hitEffect.SetActive(false);
    }
}
