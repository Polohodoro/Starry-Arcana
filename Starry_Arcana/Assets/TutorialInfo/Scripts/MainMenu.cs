using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickNewGame()
    {
      Debug.Log("새 게임");
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
