using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    void Awake()
    {
        GameObject[] GOs = GameObject.FindGameObjectsWithTag("Music");
        if (GOs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
