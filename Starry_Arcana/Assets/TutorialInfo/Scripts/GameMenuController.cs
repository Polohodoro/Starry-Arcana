using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    public GameObject gameMenuPanel; // 게임 메뉴 패널
    public Button resumeButton; // Resume Game 버튼
    public Button quitButton; // Quit Game 버튼
    public Image menuBackground; // Menu Background 이미지
    public Button gameMenuButton; // Game Menu 버튼

    private void Start()
    {
        // 버튼 클릭 이벤트 설정
        resumeButton.onClick.AddListener(OnResumeButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
        gameMenuButton.onClick.AddListener(OnGameMenuButtonClick);

        // 시작할 때 메뉴 패널 비활성화
        gameMenuPanel.SetActive(false);
        menuBackground.gameObject.SetActive(true); // Menu Background 활성화
    }

    private void OnGameMenuButtonClick()
    {
        // 게임 메뉴 패널 활성화
        gameMenuPanel.SetActive(true);
        menuBackground.gameObject.SetActive(false); // Menu Background 비활성화
    }

    private void OnResumeButtonClick()
    {
        // 게임 메뉴 패널 비활성화
        gameMenuPanel.SetActive(false);
        menuBackground.gameObject.SetActive(true); // Menu Background 활성화
    }

    private void OnQuitButtonClick()
    {
        SceneManager.LoadScene("TitleScene"); // 타이틀 씬으로 전환
    }
}
