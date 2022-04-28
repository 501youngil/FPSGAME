using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDestroy : MonoBehaviour
{    
    // ����� �ð� ����
    private float currentTime = 0f;

    private void Update()
    {
        // �������� 1.5�ʰ� ����Ǹ� ����
        if(currentTime >= 1.5)
        {
            Destroy(gameObject);
        }
        currentTime += Time.deltaTime;
    }
}
