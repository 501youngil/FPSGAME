using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 게임 상태 변수
    public enum GameState
    {
        Ready,
        Run,
        GameOver
    }
    // 싱글턴 변수 
    public static GameManager gm;
    // 게임 상태 변수
    public GameState gState;
    // UI 텍스트 변수
    public Text stateLabel;
    // 플레이어 게임 오브젝트 변수
    private GameObject player;
    // 플레이어 무브 컴포넌트 변수
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
        // 초기 게임 상태는 준비 상태로 설정
        gState = GameState.Ready;

        // 게임 시작 코루틴 함수를 실행
        StartCoroutine(GameStart());

        // 플레이어 오브젝트 검색 PlayerMove 컴포넌트 받아오기
        player = GameObject.Find("Player");
        // PlayerMove 컴포넌트 받아오기
        playerM = player.GetComponent<PlayerMove>();
    }

    IEnumerator GameStart()
    {
        // Ready .. 라는 문구 표시
        stateLabel.text = "Ready...";
        // Ready 문구 색상을 주황색으로 변경
        stateLabel.color = new Color32(255, 185, 0, 255);
        // 2초간 대기
        yield return new WaitForSeconds(2.0f);
        // Go! 라는 문구로 변경
        stateLabel.text = "Go!";
        // 0.5초가 대기
        yield return new WaitForSeconds(0.5f);
        // Go 문구를 지운다
        stateLabel.text = "";
        // 게임의 상태를 준비 상태에서 실행 상태로 전환
        gState = GameState.Run;
    }

    private void Update()
    {
        // 플레이어의 hp가 0 이하이면
        if(playerM.hp <= 0)
        {
            // 플레이어의 애니메이션을 멈춘다
            player.GetComponentInChildren<Animator>().SetFloat("MoveDirection", 0f);
            // Game Over 문구를 출력한다
            stateLabel.text = "Game Over...";
            // Game Over 색상은 붉은색
            stateLabel.color = new Color32(255, 0, 0, 255);
            // 게임의 상태를 Game Over 상태로 전환
            gState = GameState.GameOver;
        }
    }
}
