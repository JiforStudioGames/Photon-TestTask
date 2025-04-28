using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemListView: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    
    public void SetNickName(string title) => titleText.text = $"Player Nickname: {title}";
}
