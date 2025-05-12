using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadLevel1 : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadSceneAsync(2);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
