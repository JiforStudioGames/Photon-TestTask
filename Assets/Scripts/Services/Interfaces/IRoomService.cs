using System;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UniRx;

public interface IRoomService
{
    IReadOnlyReactiveCollection<RoomData> RoomList { get; }
    IReadOnlyReactiveCollection<PlayerData> PlayerList { get; }
    IObservable<Unit> JoinedRoom { get; }
    IObservable<Unit> LeavedRoom { get; }
    IObservable<Unit> RoomListUpdated { get; }
    IObservable<Unit> PlayerEnteredRoom { get; }
    IObservable<Unit> PlayerLeftRoom { get; }
    IObservable<Unit> RoomPropertiesUpdate { get; }
    IObservable<Unit> MasterClientSwitched { get; }
    IObservable<(Player player, Hashtable props)> PlayerPropertiesUpdate { get; }

    void CreateRoom(string name);
    void JoinRoom(string name);
    void LeaveRoom();
    void RefreshPlayersEventCall();
}
