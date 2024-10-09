using System;
using System.Collections.Generic;
using UBB;
using UnityEngine;

#if UNITY_EDITOR
using ParrelSync;
#endif

namespace Playroom
{
    public class PlayroomkitDevManager : MonoBehaviour
    {
        [SerializeField] private PlayroomKit.MockModeSelector mockMode = PlayroomKit.CurrentMockMode;


        [Tooltip(
            "InsertCoin() must be called in order to connect PlayroomKit server.\n\nChoose the gameObject (with the script) which calls InsertCoin.\n\nRead More in the docs")]
        [SerializeField]
        private GameObject insertCoinCaller;


        private static PlayroomkitDevManager Instance { get; set; }

#if UNITY_EDITOR
        private void Awake()
        {
#if UNITY_EDITOR
            if (ClonesManager.IsClone()) UnityBrowserBridge.Instance.httpServerPort += 1;
#endif
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (PlayroomKit.CurrentMockMode == PlayroomKit.MockModeSelector.BrowserBridge)
                UnityBrowserBridge.Instance.StartUBB();

            UpdateMockMode();
        }

        private void OnValidate()
        {
            UpdateMockMode();
        }

        private void UpdateMockMode()
        {
            PlayroomKit.CurrentMockMode = mockMode;
            PlayroomKit.RegisterGameObject("InsertCoin", insertCoinCaller);
            PlayroomKit.RegisterGameObject("devManager", gameObject);
        }

        // Called from JS side for onPlayerJoin
        private void GetPlayerID(string playerId)
        {
            PlayroomKit.MockOnPlayerJoinWrapper(playerId);
        }

        private void QuitPlayer(string playerId)
        {
            PlayroomKit.MockOnPlayerQuitWrapper(playerId);
        }

        private string InvokeWaitForState(string stateValue)
        {
            Debug.Log(stateValue);
            return stateValue;
        }

#endif
    }
}