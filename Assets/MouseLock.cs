using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseLock : MonoBehaviour 
{
    private PlayerController playerController;

    void Awake()
	{
        if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
        {
            DontDestroyOnLoad(this.gameObject);
        } 
    }

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "MainMenu" || currentScene.name == "ReportScreen")
        {
            UnlockMouse();
        }

        GameObject pauseMenu = GameObject.Find("pause(Clone)");
        GameObject statsOverlay = GameObject.Find("teamStats_overlay(Clone)");
    
        GameObject player = null;

        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains("Local Player"))
            {
                player = obj;
                break;
            }
        }

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null )
            {
                if (!playerController.IsDead && pauseMenu == null && statsOverlay == null && !CustomLayoutController.Instance.IsOpen)
                {
                    LockMouse();
                }
                else
                {
                    UnlockMouse();
                }
            }
        }
    }
    private void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}