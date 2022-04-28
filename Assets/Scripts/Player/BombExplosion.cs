using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    // ���� ȿ�� ������ ����
    public GameObject explosion;
    // ����ź ������
    public int bombPower = 3;
    // ���� �ݰ�
    public float explosionRadius = 5.0f;

    private void OnCollisionEnter(Collision collision)
    {
        // �浹�ϸ� ���� ����Ʈ�� ����
        GameObject go = Instantiate(explosion);
        go.transform.position = transform.position;

        // �ڽ��� ��ġ���� �����ݰ� �˻��� �� ������ ���ʹ̵��� ã�´�
        Collider[] enemies = Physics.OverlapSphere(this.transform.position, explosionRadius, 1 << 10);

        // ����� ���ʹ̵鿡�� ����ź �������� ������
        for(int i = 0; i < enemies.Length; i++)
        {
            EnemyFSM eFSM = enemies[i].transform.GetComponent<EnemyFSM>();
            eFSM.HitEnemy(bombPower);
        }

        // �ڱ� �ڽ� ����
        Destroy(gameObject);
    }
}
