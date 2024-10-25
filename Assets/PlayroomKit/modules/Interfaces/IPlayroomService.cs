using System;
using System.Collections.Generic;
using AOT;

namespace Playroom
{
    public partial class PlayroomKit
    {
        public interface IPlayroomService
        {
            protected static List<Action<Player>> OnPlayerJoinCallbacks = new();

            public Action OnPlayerJoin(Action<Player> onPlayerJoinCallback);

            public void InsertCoin(InitOptions options = null, Action onLaunchCallBack = null,
                Action onDisconnectCallback = null);

            public bool IsHost();

            public string GetRoomCode();

            public void StartMatchmaking(Action callback = null);

            public void SetState<T>(string key, T value, bool reliable = false);

            public T GetState<T>(string key);

            [MonoPInvokeCallback(typeof(Action<string>))]
            protected static void __OnPlayerJoinCallbackHandler(string id)
            {
                OnPlayerJoinWrapperCallback(id);
            }

            protected static void OnPlayerJoinWrapperCallback(string id)
            {
                var player = GetPlayer(id);
                foreach (var callback in IPlayroomService.OnPlayerJoinCallbacks)
                {
                    callback?.Invoke(player);
                }
            }
        }
    }
}