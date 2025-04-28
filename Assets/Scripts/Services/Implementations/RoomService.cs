using System;
using UniRx;
using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class RoomService : IRoomService
{
    private const byte LevelLoadedEventCode = 200;
    
    private readonly IPhotonService _photon;
    private readonly ReactiveCollection<RoomData> _roomList = new();
    private readonly ReactiveCollection<PlayerData> _playerList = new();
    private readonly Subject<Unit> _roomListUpdated = new();
    private readonly Subject<Unit> _joined = new();
    private readonly Subject<Unit> _leaved = new();
    private readonly Subject<Unit> _playerEnteredRoom = new();
    private readonly Subject<Unit> _playerLeftRoom = new();
    private readonly Subject<Unit> _roomPropertiesUpdate = new();
    private readonly Subject<Unit> _masterClientSwitched = new();
    private readonly Subject<(Player player, Hashtable props)> _playerPropertiesUpdate = new();

    public IReadOnlyReactiveCollection<RoomData> RoomList => _roomList;
    public IReadOnlyReactiveCollection<PlayerData> PlayerList => _playerList;
    public IObservable<Unit> RoomListUpdated => _roomListUpdated;
    public IObservable<Unit> JoinedRoom => _joined;
    public IObservable<Unit> LeavedRoom => _leaved;
    public IObservable<Unit> PlayerEnteredRoom => _playerEnteredRoom;
    public IObservable<Unit> PlayerLeftRoom => _playerLeftRoom;
    public IObservable<Unit> RoomPropertiesUpdate => _roomPropertiesUpdate;
    public IObservable<Unit> MasterClientSwitched => _masterClientSwitched;
    public IObservable<(Player player, Hashtable props)> PlayerPropertiesUpdate => _playerPropertiesUpdate.AsObservable();
    

    public RoomService(IPhotonService photon)
    {
        _photon = photon;

        _photon.RoomListUpdated.Subscribe(UpdateRooms);

        _photon.JoinedRoom.Subscribe(_ =>
        {
            RefreshPlayers();
            _joined.OnNext(Unit.Default);
        });
        
        _photon.PlayerPropertiesUpdate.Subscribe(data =>
        {
            _playerPropertiesUpdate.OnNext(data);
        });
        
        _photon.MasterClientSwitched.Subscribe(_ =>
        {
            _masterClientSwitched.OnNext(Unit.Default);
        });
        
        _photon.PlayerEntered.Subscribe(_ =>
        {
            RefreshPlayers();
            _joined.OnNext(Unit.Default);
        });
        
        _photon.PlayerLeft.Subscribe(_ =>
        {
            RefreshPlayers();
            _joined.OnNext(Unit.Default);
        });
    }

    public void RefreshPlayersEventCall()
    {
        PhotonNetwork.RaiseEvent(
            LevelLoadedEventCode,
            PhotonNetwork.LocalPlayer.UserId,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }

    private void UpdateRooms(List<RoomInfo> infos)
    {
        _roomList.Clear();
        foreach (var info in infos)
            _roomList.Add(new RoomData 
            {
                Name = info.Name,
                PlayerCount = info.PlayerCount,
                MaxPlayers = info.MaxPlayers
            });
        _roomListUpdated.OnNext(Unit.Default);
    }

    private void RefreshPlayers()
    {
        _playerList.Clear();
        foreach (var p in PhotonNetwork.PlayerList)
            _playerList.Add(new PlayerData 
            {
                Id = p.ActorNumber,
                Nickname = p.NickName
            });
    }

    public void CreateRoom(string name) => _photon.CreateRoom(name);
    public void JoinRoom(string name) => _photon.JoinRoom(name);
    public void LeaveRoom()
    {
        _photon.LeaveRoom();
        _leaved.OnNext(Unit.Default);
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        _playerEnteredRoom.OnNext(Unit.Default);
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        _playerLeftRoom.OnNext(Unit.Default);
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        _roomPropertiesUpdate.OnNext(Unit.Default);
    }
}
