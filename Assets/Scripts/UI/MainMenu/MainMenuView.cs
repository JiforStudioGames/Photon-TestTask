using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;
using System.Collections.Generic;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nicknameText;
    
    [SerializeField] Button loadButton;
    [SerializeField] Button createButton;
    [SerializeField] Button nicknameButton;
    
    [Space]
    [SerializeField] private PopupListItems popupListItems;
    [SerializeField] NicknamePopupView nicknamePopupView;

    public IObservable<Unit> OnLoadRooms => loadButton.OnClickAsObservable();
    public IObservable<Unit> OnCreateRoomClick => createButton.OnClickAsObservable();
    public IObservable<Unit> OnSetNicknameClick => nicknameButton.OnClickAsObservable();
    public IObservable<string> OnNicknameSubmitted => nicknamePopupView.OnSubmitClick;
    public IObservable<string> OnRoomSelected => popupListItems.OnRoomSelected;
    public IObservable<Unit> OnCancelPopupList => popupListItems.OnCancelButtonClick;
    public IObservable<Unit> OnCompletePopupList => popupListItems.OnCompleteButtonClick;

    public void ShowNicknamePopup()
    {
        nicknamePopupView.Show();
    }

    public void SetNickname(string nickname)
    {
        nicknameText.text = $"Nickname: {nickname}";
    }

    public void ShowRooms(IReadOnlyReactiveCollection<RoomData> rooms) => popupListItems.ShowRooms(rooms);
    public void ShowPlayers(IReadOnlyReactiveCollection<PlayerData> players, string roomName) => popupListItems.ShowPlayers(players, roomName);
    public void SetShowPopupList(bool show) => popupListItems.SetShow(show);
}
