using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    // ī�޶� �Ѿƴٴ� ��ġ ����
    public Transform followPosition;

    private void LateUpdate()
    {
        // ���� ��ġ�� followPosition ��ġ�� ��ġ ��Ŵ
        transform.position = followPosition.position;
    }
}
