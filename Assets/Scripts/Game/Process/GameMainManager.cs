using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMainManager : MonoSingleton<GameMainManager>
{
    public Camera mainCamera;

    public GameObject player;

    public CinemachineVirtualCamera virtualCam;

    private void Awake()
    {
        DontDestroyOnLoad(mainCamera.gameObject);

        DontDestroyOnLoad(player);

        DontDestroyOnLoad(transform.parent.gameObject);

        DontDestroyOnLoad(virtualCam.gameObject);
    }
}