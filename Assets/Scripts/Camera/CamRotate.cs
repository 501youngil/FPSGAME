using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float rotSpeed = 150.0f; // 회전 속력 변수 

    // 회전값 변수
    private float mx = 0f;
    private float my = 0f;


    private void Update()
    {
        // 게임 상태가 게임 중 상태가 아니면 함수 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // 마우스 입력을 받는다.
        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");

        // 회전 값 변수에 마우스 입력값을 미리 누적
        mx += mouse_X * rotSpeed * Time.deltaTime;
        my += mouse_Y * rotSpeed * Time.deltaTime;

        // 상하 회전값을 -90 ~ 90 까지 제한
        my = Mathf.Clamp(my, -90.0f, 90.0f);

        // 회전 방향으로 물체를 회전 시킨다
        transform.eulerAngles = new Vector3(-my, mx, 0);
    }
}
