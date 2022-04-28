using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ���� ���� ����
    public enum GameState
    {
        Ready,
        Run,
        GameOver
    }
    // �̱��� ���� 
    public static GameManager gm;
    // ���� ���� ����
    public GameState gState;
    // UI �ؽ�Ʈ ����
    public Text stateLabel;
    // �÷��̾� ���� ������Ʈ ����
    private GameObject player;
    // �÷��̾� ���� ������Ʈ ����
    private PlayerMove playerM;

    private void Awake()
    {
        if(gm == null)
        {
            gm = this;
        }
    }

    private void Start()
    {
        // �ʱ� ���� ���´� �غ� ���·� ����
        gState = GameState.Ready;

        // ���� ���� �ڷ�ƾ �Լ��� ����
        StartCoroutine(GameStart());

        // �÷��̾� ������Ʈ �˻� PlayerMove ������Ʈ �޾ƿ���
        player = GameObject.Find("Player");
        // PlayerMove ������Ʈ �޾ƿ���
        playerM = player.GetComponent<PlayerMove>();
    }

    IEnumerator GameStart()
    {
        // Ready .. ��� ���� ǥ��
        stateLabel.text = "Ready...";
        // Ready ���� ������ ��Ȳ������ ����
        stateLabel.color = new Color32(255, 185, 0, 255);
        // 2�ʰ� ���
        yield return new WaitForSeconds(2.0f);
        // Go! ��� ������ ����
        stateLabel.text = "Go!";
        // 0.5�ʰ� ���
        yield return new WaitForSeconds(0.5f);
        // Go ������ �����
        stateLabel.text = "";
        // ������ ���¸� �غ� ���¿��� ���� ���·� ��ȯ
        gState = GameState.Run;
    }

    private void Update()
    {
        // �÷��̾��� hp�� 0 �����̸�
        if(playerM.hp <= 0)
        {
            // �÷��̾��� �ִϸ��̼��� �����
            player.GetComponentInChildren<Animator>().SetFloat("MoveDirection", 0f);
            // Game Over ������ ����Ѵ�
            stateLabel.text = "Game Over...";
            // Game Over ������ ������
            stateLabel.color = new Color32(255, 0, 0, 255);
            // ������ ���¸� Game Over ���·� ��ȯ
            gState = GameState.GameOver;
        }
    }
}
