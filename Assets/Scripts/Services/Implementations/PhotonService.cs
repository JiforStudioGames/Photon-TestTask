using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using Zenject;

public class PhotonService : MonoBehaviourPunCallbacks, IPhotonService, IOnEventCallback, IInitializable
{

    private readonly Subject<Unit> _connected = new();
    private readonly Subject<DisconnectCause> _disconnected = new();
    private readonly Subject<Unit> _joinedRoom = new();
    private readonly Subject<List<RoomInfo>> _roomListSubject = new();
    private readonly Subject<Unit> _leftRoom = new();
    private readonly Subject<string> _playerEntered = new();
    private readonly Subject<string> _playerLeft = new();
    private readonly Subject<short> _errors = new();
    private readonly Subject<(Player player, Hashtable props)> _playerPropertiesUpdate = new();
    private readonly Subject<Unit> _masterClientSwitched = new();

    public IObservable<Unit> Connected => _connected.AsObservable();
    public IObservable<DisconnectCause> Disconnected => _disconnected.AsObservable();
    public IObservable<Unit> JoinedRoom => _joinedRoom.AsObservable();
    public IObservable<List<RoomInfo>> RoomListUpdated => _roomListSubject.AsObservable();
    public IObservable<short> OnError => _errors.AsObservable();
    public IObservable<Unit> LeftRoom => _leftRoom.AsObservable();
    public IObservable<string> PlayerEntered => _playerEntered.AsObservable();
    public IObservable<string> PlayerLeft => _playerLeft.AsObservable();
    public IObservable<Unit> MasterClientSwitched => _masterClientSwitched;
    public IObservable<(Player player, Hashtable props)> PlayerPropertiesUpdate => _playerPropertiesUpdate.AsObservable();


    public void Initialize()
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect(Application.version);
    }
    
    public void Connect(string gameVersion)
    {
        if (PhotonNetwork.IsConnected) return;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom(string roomName, RoomOptions options = null)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.CreateRoom(roomName, options);
        }
    }

    public void JoinRoom(string roomName)
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRoom(roomName);
            }
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.JoinLobby();
        }
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }
    
    public void SetNickname(string nickname)
    {
        PhotonNetwork.NickName = nickname;
    }

    // Callbacks
    public override void OnConnectedToMaster()
    {
        _connected.OnNext(Unit.Default);
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _disconnected.OnNext(cause);
    }

    public override void OnJoinedRoom()
    {
        _joinedRoom.OnNext(Unit.Default);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> rooms)
    {
        _roomListSubject.OnNext(rooms);
    }

    public override void OnLeftRoom()
    {
        _leftRoom.OnNext(Unit.Default);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _playerEntered.OnNext(newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _playerLeft.OnNext(otherPlayer.NickName);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCode.ErrorInfo)
        {
            var error = photonEvent[ParameterCode.Info] as OperationResponse;
            _errors.OnNext(error.ReturnCode);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errors.OnNext(returnCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _errors.OnNext(returnCode);
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        _playerPropertiesUpdate.OnNext((targetPlayer, changedProps));
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _masterClientSwitched.OnNext(Unit.Default);
        LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }

    public string GetActiveRoom() => PhotonNetwork.CurrentRoom.Name;
}