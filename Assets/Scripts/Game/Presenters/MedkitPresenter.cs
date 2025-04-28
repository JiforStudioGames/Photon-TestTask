using System;
using Photon.Pun;
using UniRx;
using UnityEngine;
using Zenject;

public class MedkitPresenter : IInitializable, IDisposable
{
    private const int MedkitCount = 5;
    private const string MedkitPrefabName = "MedkitPrefab";
    
    private readonly IPlayerService _playerService;
    private readonly IRoomService _roomService;
    
    private readonly CompositeDisposable _disposables = new();

    [Inject]
    public MedkitPresenter(IPlayerService playerService, IRoomService roomService)
    {
        _playerService = playerService;
        _roomService = roomService;
    }

    public void Initialize()
    {
        _roomService.JoinedRoom
            .Subscribe(_ => SpawnMedkits())
            .AddTo(_disposables);
    }

    private void SpawnMedkits()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        var spawnPoints = UnityEngine.Object.FindObjectsOfType<MedkitSpawnPoint>();

        for (int i = 0; i < MedkitCount; i++)
        {
            var pos = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
            var medkitView = PhotonNetwork.Instantiate(MedkitPrefabName, pos, Quaternion.identity).GetComponent<MedkitView>();

            medkitView.OnPickedUp
                .Subscribe(data =>
                {
                    _playerService.HealPlayer(data.playerId, data.healedHp);
                    PhotonNetwork.Destroy(medkitView.gameObject);
                })
                .AddTo(_disposables);
        }
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
}
