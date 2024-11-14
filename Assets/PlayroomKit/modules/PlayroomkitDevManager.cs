using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using ParrelSync;
using UBB;
#endif

namespace Playroom
{
    public class PlayroomkitDevManager : MonoBehaviour
    {
        [SerializeField]
        private PlayroomKit.MockModeSelector mockMode = PlayroomKit.CurrentMockMode;


        [Tooltip(
            "InsertCoin() must be called in order to connect PlayroomKit server.\n\nChoose the gameObject (with the script) which calls InsertCoin.\n\nRead More in the docs")]
        [SerializeField]
        private GameObject insertCoinCaller;

        private static PlayroomkitDevManager Instance { get; set; }

#if UNITY_EDITOR
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            UpdateMockMode();
            UnityBrowserBridge.Instance.RegisterGameObject("InsertCoin", insertCoinCaller);
            UnityBrowserBridge.Instance.RegisterGameObject("devManager", gameObject);
        }

        private void OnValidate()
        {
            UpdateMockMode();
        }

        private void UpdateMockMode()
        {
            PlayroomKit.CurrentMockMode = mockMode;
           
        }

        /// <summary>
        /// This is invoked from the JS bridge, when OnPlayerJoin is called, this is used to pass the id from playroom to unity.
        /// </summary>
        /// <param name="playerId"></param>
        private void GetPlayerID(string playerId)
        {
            PlayroomBrowserMockService.MockOnPlayerJoinWrapper(playerId);
        }
#endif
    }
}