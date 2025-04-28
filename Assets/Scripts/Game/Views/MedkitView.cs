using System;
using Photon.Pun;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(PhotonView))]
public class MedkitView : MonoBehaviourPun
{
    [SerializeField] private int healedHp = 30;
    
    private readonly Subject<(int healedHp, int playerId)> _pickedUp = new();
    public IObservable<(int healedHp, int playerId)> OnPickedUp => _pickedUp.AsObservable();

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerView>(out var playerView))
        {
            _pickedUp.OnNext((healedHp, playerView.photonView.OwnerActorNr));
        }
    }
}
