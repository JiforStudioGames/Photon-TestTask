using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using Zenject;

public class PlayerService : IPlayerService, IInitializable, IDisposable
{
    private const int MaxHealth = 100;
    private const byte LevelLoadedEventCode = 255;
    
    private readonly IRoomService _roomService;
    
    private readonly ReactiveDictionary<int, PlayerData> _players = new();
    private readonly Subject<PlayerData> _spawned = new();
    private readonly Subject<PlayerData> _despawned = new();
    private readonly Subject<PlayerData> _changedHp = new();
    private readonly CompositeDisposable _disposables = new();
    
    private HashSet<string> _loadedPlayers = new();

    public IReadOnlyReactiveDictionary<int, PlayerData> Players => _players;
    public IObservable<PlayerData> OnPlayerSpawned => _spawned;
    public IObservable<PlayerData> OnPlayerDespawned => _despawned;
    public IObservable<PlayerData> OnPlayerChangedHp => _changedHp;

    [Inject]
    public PlayerService(IRoomService roomService)
    {
        _roomService = roomService;
    }

    public void Initialize()
    {
        PhotonNetwork.AddCallbackTarget(this);
        
        _roomService.JoinedRoom
            .Subscribe(_ => SpawnAvatar(PhotonNetwork.LocalPlayer))
            .AddTo(_disposables);
        
        _roomService.LeavedRoom.Subscribe(_ => DespawnAvatar(PhotonNetwork.LocalPlayer.ActorNumber)).AddTo(_disposables);

        _roomService.PlayerPropertiesUpdate.Subscribe(data => ApplyNewHp(data.player, data.props));
    }
    
    void SpawnAvatar(Player player)
    {
        if (_players.ContainsKey(player.ActorNumber)) return;
        var spawnPoints = UnityEngine.Object.FindObjectsOfType<SpawnPoint>();
        var position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
        var playerView = PhotonNetwork.Instantiate("AvatarPrefab", position, Quaternion.identity).GetComponent<PlayerView>();
        RegisterPlayer(playerView, player.ActorNumber);
    }

    void DespawnAvatar(int playerId)
    {
        if (_players.TryGetValue(playerId, out var data))
        {
            PhotonNetwork.Destroy(data.PlayerView.gameObject);
            _players.Remove(playerId);
            _despawned.OnNext(data);
        }
    }

    public void ReplaceHp(int playerId, int amount)
    {
        if(!PhotonNetwork.InRoom) return;
        
        if (_players.TryGetValue(playerId, out var data))
        {
            var player = PhotonNetwork.CurrentRoom.GetPlayer(playerId);
            int oldHp = (int)player.CustomProperties["Health"];
            int newHp = Mathf.Clamp(oldHp + amount, 0, MaxHealth);
            
            var props = new Hashtable { { "Health", newHp } };
            player.SetCustomProperties(props);
            data.PlayerView.SetHealth(newHp);
            
            if (newHp == 0)
            {
                RespawnPlayer(data);
            }
        }
    }

    public void ApplyNewHp(Player targetPlayer, Hashtable changedProps)
    {
        if (_players.TryGetValue(targetPlayer.ActorNumber, out var player))
        {
            player.PlayerView.SetHealth((int)changedProps["Health"]);
        }
    }

    public void ApplyDamage(int playerId, int amount) => ReplaceHp(playerId, -amount);

    public void HealPlayer(int playerId, int amount) => ReplaceHp(playerId, amount);

    public void RespawnPlayer(PlayerData player)
    {
        var spawnPoints = UnityEngine.Object.FindObjectsOfType<SpawnPoint>();
        var position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
       
        var controller = player.PlayerView.GetComponent<CharacterController>();
        controller.enabled = false;
        player.PlayerView.transform.position = position;
        controller.enabled = true;
        HealPlayer(player.Id, MaxHealth);
    }

    public void RegisterPlayer(PlayerView playerView, int ownerId)
    {
        var data = new PlayerData { Id = ownerId, IsMain = ownerId == PhotonNetwork.LocalPlayer.ActorNumber, Nickname = PhotonNetwork.CurrentRoom.GetPlayer(ownerId).NickName, Health = MaxHealth, PlayerView = playerView};
        
        playerView.SetNickname(data.Nickname);
        playerView.SetHealth(data.Health);
        playerView.SetMainPlayer(data.IsMain);
        
        _players[ownerId] = data;
        _spawned.OnNext(data);
        
        var props = new Hashtable { { "Health", MaxHealth } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Dispose()
    {
        _disposables.Dispose();
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
