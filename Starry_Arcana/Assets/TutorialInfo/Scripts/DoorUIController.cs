using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DoorUIController : MonoBehaviour
{
    public GameObject doorUIPanel; // 문 UI 패널
    public Button cancelButton; // 취소 버튼
    public Button okButton; // OK 버튼
    public TextMeshProUGUI keyCountText; // 열쇠 개수를 표시할 TextMeshPro UI 요소
    public GameObject congratulationsPanel; // 축하 메시지 패널

    private GameObject currentDoor; // 현재 문 오브젝트 참조
    private bool isProcessing = false; // 중복 호출 방지 플래그

    private void Start()
    {
        doorUIPanel.SetActive(false); // 초기에는 문 UI를 비활성화
        congratulationsPanel.SetActive(false); // 초기에는 축하 메시지 패널을 비활성화
        
        // 기존 이벤트 리스너 제거 후 다시 등록
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(HideDoorUI);

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(OnOkButtonClick);
    }

    public void ShowDoorUI(GameObject door)
    {
        Debug.Log("Showing Door UI");
        currentDoor = door; // 현재 문 오브젝트를 저장
        Debug.Log("Current Door: " + currentDoor.name);
        doorUIPanel.SetActive(true); // 문 UI 표시
        isProcessing = false; // 초기화

        // 열쇠 개수 업데이트
        UpdateKeyCount();

        // 열쇠 개수에 따라 OK 버튼 활성화/비활성화
        int currentKeys = GameManager.instance.GetKeys();
        okButton.interactable = (currentKeys >= 4);
    }

    public void HideDoorUI()
    {
        Debug.Log("Hiding Door UI");
        doorUIPanel.SetActive(false); // 문 UI 숨김
        isProcessing = false; // 초기화
    }

    public void OnOkButtonClick()
    {
        if (isProcessing) return; // 이미 처리 중이면 리턴

        isProcessing = true; // 중복 호출 방지

        if (currentDoor == null)
        {
            Debug.LogError("Current Door is null");
            return;
        }

        Debug.Log("OK Button Clicked");
        
        // 축하 메시지 표시
        ShowCongratulationsMessage();
    }

    private void ShowCongratulationsMessage()
    {
        doorUIPanel.SetActive(false); // 문 UI 숨김
        congratulationsPanel.SetActive(true); // 축하 메시지 패널 표시

        // 터치 또는 클릭 입력 대기
        StartCoroutine(WaitForTouchOrClick());
    }

    private IEnumerator WaitForTouchOrClick()
    {
        bool inputDetected = false;
        while (!inputDetected)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                inputDetected = true;
            }
            yield return null;
        }

        // 타이틀 씬으로 전환
        SceneManager.LoadScene("TitleScene"); // 타이틀 씬의 이름을 "TitleScene"으로 가정
    }

    private void UpdateKeyCount()
    {
        int currentKeys = GameManager.instance.GetKeys();
        keyCountText.text = "Keys: " + currentKeys + " / 4";
    }
}
