using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가

public class BoxUIController : MonoBehaviour
{
    public GameObject boxUIPanel; // 상자 UI 패널
    public Button cancelButton; // 취소 버튼
    public Button okButton; // OK 버튼
    public TextMeshProUGUI keyCountText; // 열쇠 개수를 표시할 TextMeshPro UI 요소
    private GameObject currentBox; // 현재 상자 오브젝트 참조
    private bool isProcessing = false; // 중복 호출 방지 플래그

    private void Start()
    {
        boxUIPanel.SetActive(false); // 초기에는 상자 UI를 비활성화
        
        // 기존 이벤트 리스너 제거 후 다시 등록
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(HideBoxUI);

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(OnOkButtonClick);
    }

    public void ShowBoxUI(GameObject box)
    {
        Debug.Log("Showing Box UI");
        currentBox = box; // 현재 상자 오브젝트를 저장
        Debug.Log("Current Box: " + currentBox.name);
        boxUIPanel.SetActive(true); // 상자 UI 표시
        isProcessing = false; // 초기화

        // 열쇠 개수 업데이트
        UpdateKeyCount();
    }

    public void HideBoxUI()
    {
        Debug.Log("Hiding Box UI");
        boxUIPanel.SetActive(false); // 상자 UI 숨김
        isProcessing = false; // 초기화
    }

    public void OnOkButtonClick()
    {
        if (isProcessing) return; // 이미 처리 중이면 리턴

        isProcessing = true; // 중복 호출 방지

        if (currentBox == null)
        {
            Debug.LogError("Current Box is null");
            return;
        }

        Debug.Log("OK Button Clicked");
        
        // 열쇠 추가
        GameManager.instance.AddKey();
        // 상자 오브젝트 삭제
        Destroy(currentBox);
        // UI 닫기
        HideBoxUI();
    }

    private void UpdateKeyCount()
    {
        int currentKeys = GameManager.instance.GetKeys();
        keyCountText.text = "Keys: " + currentKeys;
    }
}
