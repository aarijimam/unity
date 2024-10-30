using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public partial class Player
        {
            public class PlayerService : IPlayerBase
            {
                
                private string _id;
                
                private readonly IInterop _interop;

                public PlayerService(string id)
                {
                    _id = id;
                    _interop = new PlayroomKitInterop();
                }

                public PlayerService(string id, IInterop interop)
                {
                    _id = id;
                    _interop = interop;
                }
                
                
                public void SetState(string key, int value, bool reliable = false)
                {
                    _interop.SetPlayerStateIntWrapper(_id, key, value, reliable);
                }

                public void SetState(string key, float value, bool reliable = false)
                {
                    _interop.SetPlayerStateFloatWrapper(_id, key, value.ToString(CultureInfo.InvariantCulture), reliable);
                }

                public void SetState(string key, bool value, bool reliable = false)
                {
                    _interop.SetPlayerStateBoolWrapper(_id, key, value, reliable);
                }

                public void SetState(string key, string value, bool reliable = false)
                {
                    _interop.SetPlayerStateStringWrapper(_id, key, value, reliable);
                }

                public void SetState(string key, object value, bool reliable = false)
                {
                    string jsonString = JsonUtility.ToJson(value);
                    _interop.SetPlayerStateStringWrapper(_id, key, jsonString, reliable);
                }
                
                

                public T GetState<T>(string key)
                {
                    Type type = typeof(T);
                    if (type == typeof(int)) return (T)(object)_interop.GetPlayerStateIntWrapper(_id, key);
                    else if (type == typeof(float)) return (T)(object)_interop.GetPlayerStateFloatWrapper(_id, key);
                    else if (type == typeof(bool)) return (T)(object)GetPlayerStateBoolById(key);
                    else if (type == typeof(string)) return (T)(object)_interop.GetPlayerStateStringWrapper(_id, key);
                    else if (type == typeof(Vector3))
                    {
                        string json = _interop.GetPlayerStateStringWrapper(_id, key);
                        if (json != null)
                        {
                            return (T)(object)JsonUtility.FromJson<Vector3>(json);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else if (type == typeof(Color))
                    {
                        string json = _interop.GetPlayerStateStringWrapper(_id, key);
                        if (json != null)
                        {
                            return (T)(object)JsonUtility.FromJson<Color>(json);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else if (type == typeof(Vector2))
                    {
                        string json = _interop.GetPlayerStateStringWrapper(_id, key);
                        if (json != null)
                        {
                            return (T)(object)JsonUtility.FromJson<Vector2>(json);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else if (type == typeof(Quaternion))
                    {
                        string json = _interop.GetPlayerStateStringWrapper(_id, key);
                        if (json != null)
                        {
                            return (T)(object)JsonUtility.FromJson<Quaternion>(json);
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else throw new NotSupportedException($"Type {typeof(T)} is not supported by GetState");
                }


                public Profile GetProfile()
                {
                    var jsonString = _interop.GetProfileWrapper(_id);
                    var profileData = ParseProfile(jsonString);
                    return profileData;
                }
                    
                private List<Action<string>> OnQuitCallbacks = new();
                
                public Action OnQuit(Action<string> callback)
                {
                    OnQuitCallbacks.Add(callback);

                    void Unsubscribe()
                    {
                        OnQuitCallbacks.Remove(callback);
                    }

                    return Unsubscribe;
                }
                


                public void Kick(Action OnKickCallBack = null)
                {
                    IPlayerBase.onKickCallBack = OnKickCallBack;
                    _interop.KickPlayerWrapper(_id, InvokeKickCallBack);
                }

                public void WaitForState(string StateKey, Action onStateSetCallback = null)
                {
                    _interop.WaitForPlayerStateWrapper(_id, StateKey, onStateSetCallback);
                }

                [MonoPInvokeCallback(typeof(Action))]
                private static void InvokeKickCallBack()
                {
                    IPlayerBase.onKickCallBack?.Invoke();
                }

                [MonoPInvokeCallback(typeof(Action))]
                private void OnQuitWrapperCallback(string id)
                {
                    if (OnQuitCallbacks != null)
                        foreach (var callback in OnQuitCallbacks)
                            callback?.Invoke(id);
                }

                void InvokeOnQuitWrapperCallback(string id)
                {
                    OnQuitWrapperCallback(id);
                }

                private bool GetPlayerStateBoolById(string key)
                {
                    var stateValue = _interop.GetPlayerStateIntWrapper(_id, key);
                    return stateValue == 1 ? true :
                        stateValue == 0 ? false :
                        throw new InvalidOperationException($"GetStateBool: {key} is not a bool");
                }
                
            }
        }
    }
}