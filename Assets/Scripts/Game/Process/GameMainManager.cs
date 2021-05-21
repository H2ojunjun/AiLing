using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class GameMainManager : MonoSingleton<GameMainManager>
    {
        [SerializeField]
        private Camera _mainCamera;

        public Camera mainCamera { get { return _mainCamera; }set{ _mainCamera = value; } }

        [SerializeField]
        private GameObject _player;

        public GameObject player { get { return _player; } set { _player = value; } }

        [SerializeField]
        private CinemachineVirtualCamera _virtualCam;

        public CinemachineVirtualCamera virtualCam { get { return _virtualCam; } set { _virtualCam = value; } }

        [SerializeField]
        private PlayerInputInstance _mainPlayerInput;

        public PlayerInputInstance mainPlayerInput { get { return _mainPlayerInput; } set { _mainPlayerInput = value; } }

        private void Awake()
        {
            DontDestroyOnLoad(_mainCamera.gameObject);

            DontDestroyOnLoad(_player);

            DontDestroyOnLoad(transform.parent.gameObject);

            DontDestroyOnLoad(_virtualCam.gameObject);
        }
    }
}

