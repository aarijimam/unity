using System;
using System.Collections.Generic;
using System.Reflection;
using UBB;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;
using UBB;
namespace Playroom
{
    public interface IPlayerService
    {
        void SetState(string playerID, string key, object value, bool reliable = false);
    }

    public class MockPlayerService : IPlayerService
    {
        public enum MockModeSelector
        {
            Local,
            BrowserBridge
        }

        public static MockModeSelector CurrentMockMode { get; set; } = MockModeSelector.Local;
        
        public void SetState(string playerID, string key, object value, bool reliable = false)
        {
            MockPlayerSetStateBrowser(playerID, key, value, reliable);
        }
        
        private static void ExecuteMockModeAction(Action localAction, Action browserAction)
        {
            switch (CurrentMockMode)
            {
                case MockModeSelector.Local:
                    localAction();
                    break;
#if UNITY_EDITOR
                case MockModeSelector.BrowserBridge:
                    browserAction();
                    break;
#endif
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static void MockPlayerSetStateBrowser(string playerID, string key, object value, bool reliable = false)
        {
            var flag = reliable ? 1 : 0;


            string jsonString;
            if (value is int intValue)
            {
                jsonString = intValue.ToString();
            }
            else
            {
                jsonString = JsonUtility.ToJson(value);
            }


#if UNITY_EDITOR
            UnityBrowserBridge.Instance.ExecuteJS(
                $"SetPlayerStateByPlayerId('{playerID}','{key}', {jsonString}, {flag})");
#endif
        }
        

    }
}