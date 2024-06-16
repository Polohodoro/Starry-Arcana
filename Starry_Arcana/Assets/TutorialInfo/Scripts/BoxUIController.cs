using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxUIController : MonoBehaviour
{
    public GameObject boxUIPanel; // 상자 UI 패널
    public Button cancelButton; // 취소 버튼
    public Button OKButton; //OK 버튼

    private void Start()
    {
        boxUIPanel.SetActive(false); // 초기에는 상자 UI를 비활성화
        cancelButton.onClick.AddListener(HideBoxUI); // 취소 버튼 클릭 이벤트에 HideBoxUI 메서드 추가
        OKButton.onClick.AddListener(ShowBoxUI); // OK 버튼 클릭 이벤트에 ShowBoxUI 메서드 추가
    }

    public void ShowBoxUI()
    {
        Debug.Log("Showing Box UI");
        boxUIPanel.SetActive(true); // 상자 UI 표시
    }

    public void HideBoxUI()
    {
        Debug.Log("Hiding Box UI");
        boxUIPanel.SetActive(false); // 상자 UI 숨김
    }
}