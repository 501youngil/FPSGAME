using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    public float rotSpeed = 150.0f; // ȸ�� �ӷ� ���� 

    // ȸ���� ����
    private float mx = 0f;
    private float my = 0f;


    private void Update()
    {
        // ���� ���°� ���� �� ���°� �ƴϸ� �Լ� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // ���콺 �Է��� �޴´�.
        float mouse_X = Input.GetAxis("Mouse X");
        float mouse_Y = Input.GetAxis("Mouse Y");

        // ȸ�� �� ������ ���콺 �Է°��� �̸� ����
        mx += mouse_X * rotSpeed * Time.deltaTime;
        my += mouse_Y * rotSpeed * Time.deltaTime;

        // ���� ȸ������ -90 ~ 90 ���� ����
        my = Mathf.Clamp(my, -90.0f, 90.0f);

        // ȸ�� �������� ��ü�� ȸ�� ��Ų��
        transform.eulerAngles = new Vector3(-my, mx, 0);
    }
}
