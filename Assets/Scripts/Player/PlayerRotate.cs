using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float rotSpeed = 150.0f; // ȸ�� �ӷ� ����

    // ȸ�� ���� ����
    private float mx = 0f;

    private void Update()
    {
        // ���� ���°� ���� �� ���°� �ƴϸ� �Լ� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // ���콺 �Է��� �޴´�.
        float mouse_X = Input.GetAxis("Mouse X");

        // �Է¹��� ���� �̿��� ȸ�� ���� ����
        //Vector3 dir = new Vector3(0, mouse_X, 0);
        //dir.Normalize();
        mx += mouse_X * rotSpeed * Time.deltaTime;

        // ������ ȸ�� ������ ��ü�� ȸ�� �Ӽ��� ����
        //transform.eulerAngles += dir * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, mx, 0);
    }
}
