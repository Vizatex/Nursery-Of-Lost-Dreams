using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ToNextScene : MonoBehaviour
{
    private int nextScenetoLoad;
    void Start()
    {
        nextScenetoLoad = SceneManager.GetActiveScene().buildIndex + 1;
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
             SceneManager.LoadScene(nextScenetoLoad);
        }
    }
}
