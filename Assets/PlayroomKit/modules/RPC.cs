using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using SimpleJSON;
using UnityEngine;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public class RPC : IRPC
        {
            private PlayroomKit _playroomKit;

            public RPC(PlayroomKit playroomKit)
            {
                _playroomKit = playroomKit;
            }
            public void RpcRegister(string name, Action<string, string> rpcRegisterCallback,
                string onResponseReturn = null)
            {
                    Debug.Log("RPC Register: " + name);
                    CallbackManager.RegisterCallback(rpcRegisterCallback, name);
                    RpcRegisterInternal(name, IRPC.InvokeRpcRegisterCallBack, onResponseReturn);
                
            }
            

            public void RpcCall(string name, object data, RpcMode mode, Action callbackOnResponse = null)
            {
                    Debug.Log("RPC Call: " + name);
                    string jsonData = IRPC.ConvertToJson(data);
                    if (IRPC.OnResponseCallbacks.ContainsKey(name))
                    {
                        IRPC.OnResponseCallbacks[name].Add(callbackOnResponse);
                    }
                    else
                    {
                        IRPC.OnResponseCallbacks.Add(name, new List<Action> { callbackOnResponse });
                        if (!IRPC.rpcCalledEvents.Contains(name))
                        {
                            IRPC.rpcCalledEvents.Add(name);
                        }
                    }

                    JSONArray jsonArray = new JSONArray();
                    foreach (string item in IRPC.rpcCalledEvents)
                    {
                        jsonArray.Add(item);
                    }

                    string jsonString = jsonArray.ToString();
                    /*
                    This is requrired to sync the rpc events between all players, without this players won't know which event has been called.
                    this is a temporary fix, RPC's need to be handled within JS for better control.
                    */
                    
                    _playroomKit.SetState("rpcCalledEventName", jsonString, reliable: true);

                    RpcCallInternal(name, jsonData, mode, IRPC.InvokeOnResponseCallback);
            }

            // Default Mode
            public void RpcCall(string name, object data, Action callbackOnResponse = null)
            {
                RpcCall(name, data, RpcMode.ALL, callbackOnResponse);
            }
            
            
            [DllImport("__Internal")]
            private static extern void RpcRegisterInternal(string name, Action<string, string> rpcRegisterCallback,
                string onResponseReturn = null);
            
            [DllImport("__Internal")]
            private extern static void RpcCallInternal(string name, string data, RpcMode mode,
                Action callbackOnResponse);
        }

    }
}