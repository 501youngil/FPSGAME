using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float rotSpeed = 150.0f; // 회전 속력 변수

    // 회전 누적 변수
    private float mx = 0f;

    private void Update()
    {
        // 게임 상태가 게임 중 상태가 아니면 함수 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // 마우스 입력을 받는다.
        float mouse_X = Input.GetAxis("Mouse X");

        // 입력받은 값을 이용해 회전 방향 결정
        //Vector3 dir = new Vector3(0, mouse_X, 0);
        //dir.Normalize();
        mx += mouse_X * rotSpeed * Time.deltaTime;

        // 결정된 회전 방향을 물체의 회전 속성에 대입
        //transform.eulerAngles += dir * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, mx, 0);
    }
}
