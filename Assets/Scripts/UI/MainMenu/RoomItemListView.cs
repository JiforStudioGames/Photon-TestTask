using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemListView: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button roomItemButton;

    public IObservable<Unit> OnClick => roomItemButton.OnClickAsObservable();
    
    public void SetTitle(string title) => titleText.text = title;
}
