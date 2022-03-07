using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    private bool isPaused = false;
    [SerializeField] GameObject pauseMenu;
    
    void Update()
    {
        Pause();
    }

    public void LeaveGame()
    {
            Debug.Log("Clicked Left Room!");
            PhotonNetwork.LeaveRoom();
            //while(PhotonNetwork.InRoom)
            //    Debug.Log("Waiting...");
            SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    void Pause()
    {
        if(Input.GetKeyDown("escape"))
        {
            isPaused = !isPaused;
            Cursor.lockState = (isPaused) ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = isPaused;
            pauseMenu.SetActive(isPaused);
        }
    }

    // public void Resume()
    // {
    //     isPaused = !isPaused;
    //     Cursor.lockState = (isPaused) ? CursorLockMode.Confined : CursorLockMode.Locked;
    //     Cursor.visible = isPaused;
    //     pauseMenu.SetActive(isPaused);
    // }
}
