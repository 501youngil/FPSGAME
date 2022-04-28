using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEvent : MonoBehaviour
{
    // EnemyFSM 컴포넌트 변수
    public EnemyFSM eFSM;

    public void OnHit()
    {
        // 플레이어에게 데미지를 주는 함수 실행
        eFSM.HitEvent();
    }
}
