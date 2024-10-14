﻿InsertCoin = async function (options, onLaunchCallBackName, gameObjectName) {
  function onLaunchCallBack() {
    unityInstance.SendMessage(gameObjectName, onLaunchCallBackName);
  }

  await Playroom.insertCoin(options, onLaunchCallBack);
};

OnPlayerJoin = function (gameObjectName) {
  Playroom.onPlayerJoin((player) => {
    unityInstance.SendMessage(gameObjectName, "GetPlayerID", player.id);
  });
};

OnPlayerJoin = function (gameObjectName) {
  Playroom.onPlayerJoin((player) => {
    unityInstance.SendMessage(gameObjectName, "GetPlayerID", player.id);
  });
};

OnPlayerQuit = function (gameObjectName, playerId) {
    console.log(playerId)
    const players = window._multiplayer.getPlayers();
    console.log(gameObjectName)
    

    if (typeof players !== "object" || players === null) {
      console.error('The "players" variable is not an object:', players);
      
      return null;
    }
    const playerState = players[playerId];

    if (!playerState) {
      console.error("Player with ID", playerId, "not found.");
      return null;
    }
   
    const unsubscribe = playerState.onQuit((player) => {
      console.log("Callback sent");
      unityInstance.SendMessage(gameObjectName, "QuitPlayer", player.id);
    });
    
    console.log(unsubscribe);
  // Add an event listener to detect tab close or window unload
  window.addEventListener("beforeunload", (event) => {
    console.log("Tab/window is being closed for player", playerId);
    unityInstance.SendMessage(gameObjectName, "ConsoleLog", "Tab/window is being closed for player");
    // Call the onQuit event for the player
    
      //playerState.onQuit(playerState); // Manually trigger the quit callback
      unityInstance.SendMessage(gameObjectName, "QuitPlayer", playerState.id);
      unityInstance.SendMessage(gameObjectName, "ConsoleLog", "Quit called");
    
    

    // Optionally, cancel the default unload event
    event.preventDefault();
    event.returnValue = ''; // Older browsers need this to prevent tab close without confirmation
  });


  window.addEventListener("beforeunload", (event) => {
    alert("The player is about to quit!");  // Shows a dialog before the tab closes
    event.returnValue = ''; // Necessary for some browsers to show confirmation
  });

  return unsubscribe; // Return the unsubscribe function if needed
}



// States
SetState = function (key, value, reliable) {
  reliable = !!reliable;

  Playroom.setState(key, value, reliable);
};

GetState = function (key) {
  return JSON.stringify(Playroom.getState(key));
};

SetPlayerStateByPlayerId = function (playerId, key, value, reliable) {
  const players = window._multiplayer.getPlayers();

  reliable = !!reliable;

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }
  const playerState = players[playerId];

  if (!playerState) {
    console.error("Player with ID", playerId, "not found.");
    return null;
  }

  if (typeof playerState.setState === "function") {
    playerState.setState(key, value, reliable);
  } else {
    console.error('The player state object does not have a "setState" method.');
    return null;
  }
};

GetPlayerStateByPlayerId = function (playerId, key) {
  const players = window._multiplayer.getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }

  const playerState = players[playerId];

  if (!playerState) {
    console.error("Player with ID", playerId, "not found.");
    return null;
  }

  if (typeof playerState.getState === "function") {
    try {
      var stateVal = playerState.getState(key);

      if (stateVal === undefined) {
        return null;
      }

      return JSON.stringify(stateVal);
    } catch (error) {
      console.log("There was an error: " + error);
    }
  } else {
    console.error('The player state object does not have a "getState" method.');
    return null;
  }
};

GetRoomCode = function () {
  return Playroom.getRoomCode();
};

MyPlayer = function () {
  return Playroom.myPlayer().id;
};

IsHost = function () {
  return Playroom.isHost();
};

IsStreamScreen = function () {
  return Playroom.isStreamScreen();
};

GetProfile = function (playerId) {
  const players = window._multiplayer.getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }

  const playerState = players[playerId];

  if (!playerState) {
    console.error("Player with ID", playerId, "not found.");
    return null;
  }

  if (typeof playerState.getProfile === "function") {
    const profile = playerState.getProfile();
    var returnStr = JSON.stringify(profile);

    return returnStr;
  } else {
    console.error(
      'The player state object does not have a "getProfile" method.'
    );
    return null;
  }
};

StartMatchmaking = async function () {
  await Playroom.startMatchmaking();
};

OnDisconnect = async function (callbackkey) {
  console.log("onDisconectCalled called", callbackkey);

  Playroom.onDisconnect((e) => {
    console.log(`Disconnected!`, e.code, e.reason, typeof e);
    unityInstance.SendMessage("CallbackManager", "InvokeCallback", callbackkey);
  });
};

WaitForState = function (stateKey, callbackKey) {
  Playroom.waitForState(stateKey)
    .then((stateVal) => {
      const data = {
        key: callbackKey,
        parameter: stateVal,
      };

      const jsonData = JSON.stringify(data);

      unityInstance.SendMessage("CallbackManager", "InvokeCallback", jsonData);
    })
    .catch((error) => {
      console.error("Error Waiting for state:", error);
    });
};

WaitForPlayerState = async function (playerId, stateKey, onStateSetCallback) {
  if (!window.Playroom) {
    console.error(
      "Playroom library is not loaded. Please make sure to call InsertCoin first."
    );
    reject("Playroom library not loaded");
    return;
  }

  const players = window._multiplayer.getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }
  const playerState = players[playerId];

  if (!playerState) {
    console.error("Player with ID", playerId, "not found.");
    return null;
  }

  await Playroom.waitForPlayerState(playerState, stateKey, onStateSetCallback);
};

Kick = async function (playerID) {
  if (!window.Playroom) {
    console.error(
      "Playroom library is not loaded. Please make sure to call InsertCoin first."
    );
    reject("Playroom library not loaded");
    return;
  }

  const players = window._multiplayer.getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }
  const playerState = players[playerID];

  if (!playerState) {
    console.error("Player with ID", playerID, "not found.");
    return null;
  }

  await playerState.kick();
};

OnQuit = function (playerID) {
  if (!window.Playroom) {
    console.error(
      "Playroom library is not loaded. Please make sure to call InsertCoin first."
    );
    reject("Playroom library not loaded");
    return;
  }

  const players = window._multiplayer.getPlayers();

  if (typeof players !== "object" || players === null) {
    console.error('The "players" variable is not an object:', players);
    return null;
  }
  const playerState = players[playerID];

  if (!playerState) {
    console.error("Player with ID", playerID, "not found.");
    return null;
  }

  playerState.onQuit((state) => {
    console.log(`${state.id} quit!`);
  });
};

ResetPlayersStates = async function (keysToExclude) {
  await Playroom.resetPlayersStates(keysToExclude);
};

ResetStates = async function (keysToExclude) {
  await Playroom.resetStates(keysToExclude);
};

RpcRegister = function (name, callbackKey) {
  console.log(name);

  Playroom.RPC.register(name, (data, caller) => {
    const jsonData = {
      key: callbackKey,
      parameter: { data: data, callerId: caller.id },
    };

    const jsonString = JSON.stringify(jsonData);

    console.log(jsonString);

    unityInstance.SendMessage("CallbackManager", "HandleRPC", jsonString);
  });
};

RpcCall = function (name, data, rpcMode) {
  let mode;

  if (rpcMode === "ALL") {
    mode = Playroom.RPC.Mode.ALL;
  }

  if (rpcMode === "OTHERS") {
    mode = Playroom.RPC.Mode.OTHERS;
  }

  if (rpcMode === "HOST") {
    mode = Playroom.RPC.Mode.HOST;
  }

  Playroom.RPC.call(name, data, mode);
};
