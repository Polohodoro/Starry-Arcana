using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int keys = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKey()
    {
        keys++;
        Debug.Log("Total keys: " + keys);
    }

    public int GetKeys()
    {
        return keys;
    }
}
