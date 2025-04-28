using System;
using UniRx;

public interface IPlayerService
{
    IReadOnlyReactiveDictionary<int, PlayerData> Players { get; }
    IObservable<PlayerData> OnPlayerSpawned { get; }
    IObservable<PlayerData> OnPlayerDespawned { get; }
    IObservable<PlayerData> OnPlayerChangedHp { get; }
    void ApplyDamage(int playerId, int amount);
    void HealPlayer(int playerId, int amount);
    void RespawnPlayer(PlayerData player);
    void RegisterPlayer(PlayerView getComponent, int ownerId);
}
