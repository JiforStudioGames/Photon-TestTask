using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;

public interface IPhotonService
{
    // Connection events
    IObservable<Unit> Connected { get; }
    IObservable<DisconnectCause> Disconnected { get; }

    // Room events
    IObservable<Unit> JoinedRoom { get; }
    IObservable<List<RoomInfo>> RoomListUpdated { get; }
    IObservable<Unit> LeftRoom { get; }
    IObservable<string> PlayerEntered { get; }
    IObservable<string> PlayerLeft { get; }
    IObservable<(Player player, Hashtable props)> PlayerPropertiesUpdate { get; }
    IObservable<Unit> MasterClientSwitched { get; }

    // Error events
    IObservable<short> OnError { get; }

    // Control methods
    void Connect(string gameVersion);
    void CreateRoom(string roomName, RoomOptions options = null);
    void JoinRoom(string roomName);
    void LeaveRoom();
    void Disconnect();
    void SetNickname(string nickname);
    string GetActiveRoom();
}