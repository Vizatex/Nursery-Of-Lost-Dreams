using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadLevel2 : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadSceneAsync(3);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
