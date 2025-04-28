using System;
using UniRx;
using Zenject;

public class PlayerPresenter : IInitializable, IDisposable
{
    readonly IPlayerService _playerService;
    readonly CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public PlayerPresenter(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    public void Initialize()
    {
        _playerService.OnPlayerChangedHp
            .Subscribe(data =>
            {
                data.PlayerView.SetHealth(data.Health);
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
