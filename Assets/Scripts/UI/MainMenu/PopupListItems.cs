using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PopupListItems : MonoBehaviour
{
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _completeButton;
    [SerializeField] Transform listContainer;
    [SerializeField] RoomItemListView roomItemListPrefab;
    [SerializeField] PlayerItemListView playerItemListPrefab;
    [SerializeField] TMP_Text titleText;
    
    readonly Subject<string> _itemSelected = new();
    public IObservable<string> OnRoomSelected => _itemSelected;
    public IObservable<Unit> OnCancelButtonClick => _cancelButton.OnClickAsObservable();
    public IObservable<Unit> OnCompleteButtonClick => _completeButton.OnClickAsObservable();
    
    public void ShowRooms(IReadOnlyReactiveCollection<RoomData> rooms)
    {
        _completeButton.gameObject.SetActive(false);
        titleText.text = "Select Room";
        ClearList();
        foreach (var roomData in rooms)
        {
            RoomItemListView spawnedRoomItem = Instantiate(roomItemListPrefab, listContainer);
            spawnedRoomItem.SetTitle($"{roomData.Name} ({roomData.PlayerCount}/{roomData.MaxPlayers})");
            spawnedRoomItem.OnClick
                .Subscribe(_ => _itemSelected.OnNext(roomData.Name))
                .AddTo(spawnedRoomItem.gameObject);
        }
    }

    public void ShowPlayers(IReadOnlyReactiveCollection<PlayerData> players, string roomName)
    {
        _completeButton.gameObject.SetActive(true);
        titleText.text = $"Players in Room {roomName}";
        ClearList();
        foreach (var playerData in players)
        {
            PlayerItemListView spawnedPlayerItem = Instantiate(playerItemListPrefab, listContainer);
            spawnedPlayerItem.SetNickName(playerData.Nickname);
        }
    }
    
    void ClearList()
    {
        for (int i = listContainer.childCount - 1; i >= 0; i--)
            Destroy(listContainer.GetChild(i).gameObject);
    }

    public void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}
