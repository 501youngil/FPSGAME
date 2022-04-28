using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    // 수류탄 오브젝트
    public GameObject bombFactory;
    // 발사 위치
    public Transform firePosition;
    // 발사할 힘
    public float throwPower = 10.0f;
    // 총알 이펙트 오브젝트
    public GameObject bulletEffect;
    // 총알 공격력 변수
    public int attackPower = 2;
    // 무기 모드 텍스트
    public Text weaponText;
    public Image magazineImg;
    public Text magazineText;
    public int maxBullet = 30;
    public int remainingBullet = 30;
    public float reloadTime = 2.0f;
    private bool isReloading = false;    
    
    // 파티클 시스템 게임 오브젝트
    private ParticleSystem ps;
    // 오디오 소스 컴포넌트 변수
    private AudioSource aSource;
    // 애니메이터 컴포넌트 변수
    private Animator anim;
    // 총발사 효과 오브젝트 배열
    public GameObject[] eff_Flash;

    // 무기 모드 상수
    enum weaponMode
    {
        Normal,
        Sniper
    }
    private weaponMode wMode;
    // 카메라 줌인 줌 아웃 체크 변수
    private bool isZoom = false;

    private void Start()
    {
        // 총알 이펙트 오브젝트에서 파티클 시스템 컨포넌트 추출
        ps = bulletEffect.GetComponent<ParticleSystem>();
        // 오디오 소스 컴토넌트 추출
        aSource = GetComponent<AudioSource>();
        // 자식 오브젝트에서 애니메이터 변수 추출
        anim = GetComponentInChildren<Animator>();
        // 무기의 기본 모드를 Normal모드로 설정
        wMode = weaponMode.Normal;
        weaponText.text = "Normal";
    }

    private void Update()
    {        
        // 게임 상태가 게임 중 상태가 아니면 함수 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            switch (wMode)
            {
                case weaponMode.Normal:
                    // 수류탄 오브젝트 생성, 수류탄의 생성 위치를 발사 위치로
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.position;

                    // 수류탄의 Rigidbody 컴포넌트 받기
                    Rigidbody rd = bomb.GetComponent<Rigidbody>();

                    // 카메라 정면의 방향으로 수류탄에 물리적인 힘을 가한다
                    rd.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse); // ForceMode.Impulse : 순간적인 힘을 가한다, 질량의 영향 O.
                    break;                                                                      // ForceMode.Force : 연속적인 힘을 가한다, 질량의 영향 O.
                case weaponMode.Sniper:                                                         // ForceMode.VelocityChange : 순간적인 힘을 가한다, 질량의 영향 X
                        // 만일 줌 아웃 상태면                                                  // ForceMode.Acceleration : 연속적인 힘을 가한다, 질량의 영향 X.
                    if (!isZoom)
                    {   // 줌인 상태로 만들고, 카메라의 시야각(FoV)를 15도로 변경
                        isZoom = true;
                        Camera.main.fieldOfView = 15.0f;
                    }
                    // 줌 인 상태이면
                    else
                    {  // 줌 아웃 상태로 만들고, 카메라의 시야각(FoV)를 60도로 변경
                        isZoom = false;
                        Camera.main.fieldOfView = 60.0f;
                    }
                    break;
            }            
        }

        // 숫자키 입력으로 무기모드 변경
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = weaponMode.Normal;
            weaponText.text = "Normal";
            // 줌 아웃 상태로 전환
            Camera.main.fieldOfView = 60.0f;
            isZoom = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            wMode = weaponMode.Sniper;
            weaponText.text = "Sniper";
        }

        if(!isReloading && Input.GetMouseButtonDown(0))                                                 
        {
            --remainingBullet;

            // 카메라의 위치에서 생성후 카메라의 정면 방향으로 발사,  new Ray(생성 위치, 발사방향)
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // 레이에 부딪힌 대상의 정보를 저장할 변수
            RaycastHit hitinfo = new RaycastHit();

            // 레이를 발사후 부딪힌 대상이 있으면 피격 이펙트 표시
            if(Physics.Raycast(ray, out hitinfo))
            {
                // 부딪힌 대상의 레이어가 Enemy이면 데미지 함수 실행
                if(hitinfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitinfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(attackPower);
                }

                // 부딪힌 위치에 총알 이펙트 오브젝트를 위치
                bulletEffect.transform.position = hitinfo.point;

                // 총알 이펙트의 방향을 부딪힌 오브젝트의 수직 방향과 일치시킨다
                bulletEffect.transform.forward = hitinfo.normal;

                // 총알 이펙트 플레이
                ps.Play();
            }
            // 총소리 플레이
            aSource.Play();

            // 만일 . 블랜드 트리의 MoveDirection 파라메터의 값이 0 일때 
            if(anim.GetFloat("MoveDirection") == 0)
            {
                // 총 발사 애니메이션 플레이
                anim.SetTrigger("Attack");
            }
            // 총구 이펙트 코루틴 실행
            StartCoroutine(ShootEffect(0.05f));

            magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;
            UpdateBulletText();

            if(remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
    }

    // 총구 이펙트 코루틴 함수
    IEnumerator ShootEffect(float duration)
    {
        // 5개의 이펙트 오브젝트 중 랜덤으로 1개 고르기
        int num = Random.Range(0, eff_Flash.Length - 1);

        // 선택된 오브제트 활성화
        eff_Flash[num].SetActive(true);

        // 일정시간 동안 기다리기
        yield return new WaitForSeconds(duration);

        // 활성화된 오브젝트 다시 비활성화
        eff_Flash[num].SetActive(false);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(0.3f);
        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainingBullet = maxBullet;
        UpdateBulletText();
        anim.SetTrigger("Reloading");
    }
    void UpdateBulletText()
    {
        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainingBullet, maxBullet);
    }
}
