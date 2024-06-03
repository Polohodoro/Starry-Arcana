using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickNewGame()
    {
      SceneManager.LoadScene("SampleScene");
    }
    public void OnClickLoad()
    {
      Debug.Log("불러오기");
    }
    public void OnClickOption()
    {
      Debug.Log("옵션");
    }
    public void OnClickQuit()
    {
      Debug.Log("게임 종료");
      Application.Quit();
    }
}
