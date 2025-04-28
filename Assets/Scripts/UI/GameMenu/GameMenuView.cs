using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuView : MonoBehaviour
{
    [SerializeField] Button logOutButton;
    
    public IObservable<Unit> OnLogOutRoomClick => logOutButton.OnClickAsObservable();
}
