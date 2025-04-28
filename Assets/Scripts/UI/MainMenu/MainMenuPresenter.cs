using System;
using Photon.Pun;
using UniRx;
using Zenject;
using UnityEngine.SceneManagement;

public class MainMenuPresenter : IInitializable, IDisposable
{
    private const string GameSceneName = "Playground";
    
    private readonly MainMenuView _view;
    private readonly IRoomService _roomService;
    private readonly IPhotonService _photon;
    private readonly CompositeDisposable _disposable = new();

    [Inject]
    public MainMenuPresenter(
        MainMenuView view,
        IRoomService roomService,
        IPhotonService photon)
    {
        _view = view;
        _roomService = roomService;
        _photon = photon;
    }

    public void Initialize()
    {
        _disposable?.Clear();
        
        _view.OnLoadRooms
            .Subscribe(_ =>
            {
                _view.ShowRooms(_roomService.RoomList);
                _view.SetShowPopupList(true);
            })
            .AddTo(_disposable);
            
        _view.OnCreateRoomClick
            .Subscribe(_ =>
            {
                _roomService.CreateRoom("Room_" + UnityEngine.Random.Range(1000, 9999));
                PhotonNetwork.LoadLevel(1);
            })
            .AddTo(_disposable);
        
        _view.OnSetNicknameClick
            .Subscribe(_ => _view.ShowNicknamePopup())
            .AddTo(_disposable);

        _view.OnNicknameSubmitted
            .Subscribe(name =>
            {
                _photon.SetNickname(name);
                _view.SetNickname(name);
            })
            .AddTo(_disposable);

        _view.OnRoomSelected
            .Subscribe(roomName =>
            {
                _roomService.JoinRoom(roomName);
                _view.SetShowPopupList(true);
            })
            .AddTo(_disposable);
        
        _view.OnCancelPopupList
            .Subscribe(_ =>
            {
                _photon.LeaveRoom();
                _view.SetShowPopupList(false);
            })
            .AddTo(_disposable);
        
        _view.OnCompletePopupList
            .Subscribe(_ =>
            {
                PhotonNetwork.LoadLevel(1);
            })
            .AddTo(_disposable);
        
        _roomService.RoomListUpdated
            .Subscribe(_ => _view.ShowRooms(_roomService.RoomList))
            .AddTo(_disposable);

        _roomService.JoinedRoom
            .Subscribe(_ =>
            {
                _view.ShowPlayers(_roomService.PlayerList, _photon.GetActiveRoom());
                _view.SetShowPopupList(true);
            })
            .AddTo(_disposable);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}