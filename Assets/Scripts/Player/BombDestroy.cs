using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDestroy : MonoBehaviour
{    
    // 경과된 시간 변수
    private float currentTime = 0f;

    private void Update()
    {
        // 생성된후 1.5초가 경과되면 제거
        if(currentTime >= 1.5)
        {
            Destroy(gameObject);
        }
        currentTime += Time.deltaTime;
    }
}
