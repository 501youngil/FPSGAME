using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    // �̵� �ӵ� ����
    public float moveSpeed = 7f;
    // �߷� ����
    public float gravity = -20f;
    // ���� �ӷ� ����
    public float yVelocity = 0f;
    // ������ ����
    public float jumpPower = 10f;
    // ���� ���� ����
    private bool isJumping = false;
    // ü�� ����
    public int hp;
    // �ִ� ü��
    public int maxHp = 10;

    // �����̴� UI
    public Slider hpSlider;

    // ĳ���� ��Ʈ�ѷ� ����
    CharacterController cc;

    // �ǰ� ����Ʈ UI ������Ʈ
    public GameObject hitEffect;

    // �ִϸ����� ������Ʈ ����
    private Animator anim;

    private void Start()
    {
        Cursor.visible = false; // ���콺 Ŀ���� ������ �ʰ�
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ��ġ ����      

        // ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
        cc = GetComponent<CharacterController>();

        // ü�� ���� �ʱ�ȭ
        hp = maxHp;

        // �ڽ� ������Ʈ���� �ִϸ����� ���� ����
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // �����̴� value�� ü�� ������ ����
        hpSlider.value = (float)hp / (float)maxHp;

        // ���� ���°� ���� �� ���°� �ƴϸ� �Լ� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // Ű���� �Է��� ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // �̵� ���� ����
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        // �̵� ���� ���Ϳ� ũ�Ⱚ�� �ִϸ������� �̵� ���� Ʈ���� ����
        anim.SetFloat("MoveDirection", dir.magnitude);

        // �̵� ����(������ǥ)�� ī�޶��� ����(������ǥ) �������� ��ȯ
        dir = Camera.main.transform.TransformDirection(dir); // transform.TransformDirection : ��ȯ�� ���� ����

        // ���� ���̿���, �ٽ� �ٴڿ� ����������
        if(cc.collisionFlags == CollisionFlags.Below) // cc.collisionFlags ĳ���� ��Ʈ�ѷ� �浹ü�� �浹 ���� üũ
        {
            // ���� ���̸�
            if (isJumping)
            {
                // ���� �� ���·� �ʱ�ȭ
                isJumping = false;                
            }
            // ĳ������ ���� �ӵ��� 0���� 
            yVelocity = 0f;
        }
        // �����̽��� ������ ����,  ���� ���� ���� ���¸�
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            // ĳ���� ���� �ӵ��� ������ ����
            yVelocity = jumpPower;
            isJumping = true;
        }

        //  ĳ������ �����ӵ�(�߷�) ����
        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        // �̵� �������� �÷��̾� �̵� , �浹���� üũ
        cc.Move(dir * moveSpeed * Time.deltaTime);        
    }

    // �÷��̾� �ǰ� �Լ�
    public void OnDamage(int value)
    {
        hp -= value;
        if(hp < 0)
        {
            hp = 0;
        }
        // hp�� 0���� ū ��� �ǰ� ����Ʈ �ڷ�ƾ ����
        else
        {
            StartCoroutine(HitEffect());
        }
    }

    IEnumerator HitEffect()
    {
        // �ǰ� UI Ȱ��ȭ
        hitEffect.SetActive(true);
        // 0.3�� ���
        yield return new WaitForSeconds(0.3f);
        // �ǰ� UI ��Ȱ��ȭ
        hitEffect.SetActive(false);
    }
}
