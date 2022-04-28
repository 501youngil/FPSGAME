using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    // 폭발 효과 프리펩 변수
    public GameObject explosion;
    // 수류탄 데미지
    public int bombPower = 3;
    // 폭발 반경
    public float explosionRadius = 5.0f;

    private void OnCollisionEnter(Collision collision)
    {
        // 충돌하면 폭발 이펙트를 생성
        GameObject go = Instantiate(explosion);
        go.transform.position = transform.position;

        // 자신의 위치에서 일정반경 검색후 그 범위의 에너미들을 찾는다
        Collider[] enemies = Physics.OverlapSphere(this.transform.position, explosionRadius, 1 << 10);

        // 검출된 에너미들에게 수류탄 데미지를 입힌다
        for(int i = 0; i < enemies.Length; i++)
        {
            EnemyFSM eFSM = enemies[i].transform.GetComponent<EnemyFSM>();
            eFSM.HitEnemy(bombPower);
        }

        // 자기 자신 제거
        Destroy(gameObject);
    }
}
