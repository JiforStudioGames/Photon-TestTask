using System;
using Cinemachine;
using Photon.Pun;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class PlayerView : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private Transform CameraFallowPoint;
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private ThirdPersonController playerController;
    
    [Header("Fall Damage Settings")]
    [SerializeField] float multiplyDamage = 10f;    
    [SerializeField] float minFallVelocity = 10f;    
    [SerializeField] float maxFallVelocity = 30f;    
    [SerializeField] int maxDamage = 50; 
    
    private float _peakFallSpeed;
    private bool _wasGrounded;
    
    private IPlayerService  _playerService;
    private SceneContext _sceneContext;

    public void SetNickname(string name)
    {
        if (nicknameText != null)
            nicknameText.text = name;
    }

    public void SetHealth(int health)
    {
        if (healthSlider != null)
            healthSlider.value = health;
    }

    public void SetMainPlayer(bool isMainPlayer)
    {
        if (!isMainPlayer)
        {
            return;
        }
        
        playerInput.enabled = true;
        playerController.enabled = true;
        CinemachineVirtualCamera cinemachineFallow = FindAnyObjectByType<CinemachineVirtualCamera>();
        cinemachineFallow.Follow = CameraFallowPoint;
    }
    
    void Update()
    {
        bool isGrounded = playerController.Grounded;
        
        if (!isGrounded)
        {
            _peakFallSpeed = Mathf.Min(maxFallVelocity, -playerController.VerticalVelocity);
        }
        else if (_wasGrounded == false)
        {
            if (_peakFallSpeed > minFallVelocity && photonView.IsMine)
            {
                float t = Mathf.InverseLerp(minFallVelocity, maxFallVelocity, _peakFallSpeed);
                int dmg = Mathf.CeilToInt(t * maxDamage);
                _playerService.ApplyDamage(photonView.OwnerActorNr, (int)(dmg * multiplyDamage));
            }
            _peakFallSpeed = 0f;
        }

        _wasGrounded = isGrounded;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _sceneContext = FindObjectOfType<SceneContext>();
        
        _playerService = _sceneContext.Container.Resolve<IPlayerService>();
        var ownerId = photonView.OwnerActorNr;
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            _playerService.RegisterPlayer(this, ownerId);
        }
    }
}
