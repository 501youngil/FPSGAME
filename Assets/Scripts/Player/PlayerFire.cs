using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    // ����ź ������Ʈ
    public GameObject bombFactory;
    // �߻� ��ġ
    public Transform firePosition;
    // �߻��� ��
    public float throwPower = 10.0f;
    // �Ѿ� ����Ʈ ������Ʈ
    public GameObject bulletEffect;
    // �Ѿ� ���ݷ� ����
    public int attackPower = 2;
    // ���� ��� �ؽ�Ʈ
    public Text weaponText;
    public Image magazineImg;
    public Text magazineText;
    public int maxBullet = 30;
    public int remainingBullet = 30;
    public float reloadTime = 2.0f;
    private bool isReloading = false;    
    
    // ��ƼŬ �ý��� ���� ������Ʈ
    private ParticleSystem ps;
    // ����� �ҽ� ������Ʈ ����
    private AudioSource aSource;
    // �ִϸ����� ������Ʈ ����
    private Animator anim;
    // �ѹ߻� ȿ�� ������Ʈ �迭
    public GameObject[] eff_Flash;

    // ���� ��� ���
    enum weaponMode
    {
        Normal,
        Sniper
    }
    private weaponMode wMode;
    // ī�޶� ���� �� �ƿ� üũ ����
    private bool isZoom = false;

    private void Start()
    {
        // �Ѿ� ����Ʈ ������Ʈ���� ��ƼŬ �ý��� ������Ʈ ����
        ps = bulletEffect.GetComponent<ParticleSystem>();
        // ����� �ҽ� �����Ʈ ����
        aSource = GetComponent<AudioSource>();
        // �ڽ� ������Ʈ���� �ִϸ����� ���� ����
        anim = GetComponentInChildren<Animator>();
        // ������ �⺻ ��带 Normal���� ����
        wMode = weaponMode.Normal;
        weaponText.text = "Normal";
    }

    private void Update()
    {        
        // ���� ���°� ���� �� ���°� �ƴϸ� �Լ� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            switch (wMode)
            {
                case weaponMode.Normal:
                    // ����ź ������Ʈ ����, ����ź�� ���� ��ġ�� �߻� ��ġ��
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.position;

                    // ����ź�� Rigidbody ������Ʈ �ޱ�
                    Rigidbody rd = bomb.GetComponent<Rigidbody>();

                    // ī�޶� ������ �������� ����ź�� �������� ���� ���Ѵ�
                    rd.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse); // ForceMode.Impulse : �������� ���� ���Ѵ�, ������ ���� O.
                    break;                                                                      // ForceMode.Force : �������� ���� ���Ѵ�, ������ ���� O.
                case weaponMode.Sniper:                                                         // ForceMode.VelocityChange : �������� ���� ���Ѵ�, ������ ���� X
                        // ���� �� �ƿ� ���¸�                                                  // ForceMode.Acceleration : �������� ���� ���Ѵ�, ������ ���� X.
                    if (!isZoom)
                    {   // ���� ���·� �����, ī�޶��� �þ߰�(FoV)�� 15���� ����
                        isZoom = true;
                        Camera.main.fieldOfView = 15.0f;
                    }
                    // �� �� �����̸�
                    else
                    {  // �� �ƿ� ���·� �����, ī�޶��� �þ߰�(FoV)�� 60���� ����
                        isZoom = false;
                        Camera.main.fieldOfView = 60.0f;
                    }
                    break;
            }            
        }

        // ����Ű �Է����� ������ ����
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = weaponMode.Normal;
            weaponText.text = "Normal";
            // �� �ƿ� ���·� ��ȯ
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

            // ī�޶��� ��ġ���� ������ ī�޶��� ���� �������� �߻�,  new Ray(���� ��ġ, �߻����)
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // ���̿� �ε��� ����� ������ ������ ����
            RaycastHit hitinfo = new RaycastHit();

            // ���̸� �߻��� �ε��� ����� ������ �ǰ� ����Ʈ ǥ��
            if(Physics.Raycast(ray, out hitinfo))
            {
                // �ε��� ����� ���̾ Enemy�̸� ������ �Լ� ����
                if(hitinfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitinfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(attackPower);
                }

                // �ε��� ��ġ�� �Ѿ� ����Ʈ ������Ʈ�� ��ġ
                bulletEffect.transform.position = hitinfo.point;

                // �Ѿ� ����Ʈ�� ������ �ε��� ������Ʈ�� ���� ����� ��ġ��Ų��
                bulletEffect.transform.forward = hitinfo.normal;

                // �Ѿ� ����Ʈ �÷���
                ps.Play();
            }
            // �ѼҸ� �÷���
            aSource.Play();

            // ���� . ���� Ʈ���� MoveDirection �Ķ������ ���� 0 �϶� 
            if(anim.GetFloat("MoveDirection") == 0)
            {
                // �� �߻� �ִϸ��̼� �÷���
                anim.SetTrigger("Attack");
            }
            // �ѱ� ����Ʈ �ڷ�ƾ ����
            StartCoroutine(ShootEffect(0.05f));

            magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;
            UpdateBulletText();

            if(remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
    }

    // �ѱ� ����Ʈ �ڷ�ƾ �Լ�
    IEnumerator ShootEffect(float duration)
    {
        // 5���� ����Ʈ ������Ʈ �� �������� 1�� ����
        int num = Random.Range(0, eff_Flash.Length - 1);

        // ���õ� ������Ʈ Ȱ��ȭ
        eff_Flash[num].SetActive(true);

        // �����ð� ���� ��ٸ���
        yield return new WaitForSeconds(duration);

        // Ȱ��ȭ�� ������Ʈ �ٽ� ��Ȱ��ȭ
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
