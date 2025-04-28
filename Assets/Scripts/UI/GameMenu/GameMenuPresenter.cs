using System;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

public class GameMenuPresenter : IInitializable, IDisposable
{
    private const string MainMenuSceneName = "MainMenu";
    
    private readonly GameMenuView _view;
    private readonly IRoomService _roomService;
    private readonly CompositeDisposable _disposable = new();
    
    [Inject]
    public GameMenuPresenter(
        GameMenuView view,
        IRoomService roomService)
    {
        _view = view;
        _roomService = roomService;
    }

    public void Initialize()
    {
        _view.OnLogOutRoomClick
            .Subscribe(_ =>
            {
                _roomService.LeaveRoom();
                _roomService.RefreshPlayersEventCall();
                SceneManager.LoadScene(MainMenuSceneName);
            })
            .AddTo(_disposable);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}
