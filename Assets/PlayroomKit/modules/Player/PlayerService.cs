using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public partial class Player
        {
            public class PlayerService : IPlayerBase
            {
                
                private readonly IInterop _interop;

                public PlayerService()
                {
                    _interop = new PlayroomKitInterop();
                }

                public PlayerService(IInterop interop)
                {
                    _interop = interop;
                }

                public void SetState(string id, string key, object value, bool reliable = false)
                {
                    string jsonString = JsonUtility.ToJson(value);
                    // Debug.Log(jsonString);
                    _interop.SetPlayerStateStringWrapper(id, key, jsonString, reliable);
                }

                public T GetState<T>(string id, string key)
                {
                    ;
                    Type type = typeof(T);
                    if (type == typeof(int)) return (T)(object)_interop.GetPlayerStateIntWrapper(id, key);
                    else if (type == typeof(float)) return (T)(object)_interop.GetPlayerStateFloatWrapper(id, key);
                    else if (type == typeof(bool)) return (T)(object)GetPlayerStateBoolById(id, key);
                    else if (type == typeof(string)) return (T)(object)_interop.GetPlayerStateStringWrapper(id, key);
                    else if (type == typeof(Vector3))
                    {
                        string json = _interop.GetPlayerStateStringWrapper(id, key);
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
                        string json = _interop.GetPlayerStateStringWrapper(id, key);
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
                        string json = _interop.GetPlayerStateStringWrapper(id, key);
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
                        string json = _interop.GetPlayerStateStringWrapper(id, key);
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


                public Profile GetProfile(string id)
                {
                    var jsonString = _interop.GetProfileWrapper(id);
                    var profileData = ParseProfile(jsonString);
                    return profileData;
                }

                private bool GetPlayerStateBoolById(string id, string key)
                {
                    var stateValue = _interop.GetPlayerStateIntWrapper(id, key);
                    return stateValue == 1 ? true :
                        stateValue == 0 ? false :
                        throw new InvalidOperationException($"GetStateBool: {key} is not a bool");
                }
                
                
            }
        }
    }
}