
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System;

public class NicknamePopupView : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button submitButton;
    [SerializeField] Button cancelButton;

    readonly Subject<string> _onSubmit = new Subject<string>();
    public IObservable<string> OnSubmitClick => _onSubmit.AsObservable();
    public IObservable<Unit> OnCancelClick => cancelButton.OnClickAsObservable();
    
    private CompositeDisposable _disposable = new();

    private void BeforeShow()
    {
        submitButton.OnClickAsObservable()
            .Select(_ => inputField.text)
            .Where(name => !string.IsNullOrEmpty(name))
            .Subscribe(name =>
            {
                _onSubmit.OnNext(name);
                Hide();
            })
            .AddTo(_disposable);
        
        cancelButton.OnClickAsObservable()
            .Subscribe(_ => Hide())
            .AddTo(_disposable);
    }

    public void Show()
    {
        BeforeShow();
        
        inputField.text = string.Empty;
        gameObject.SetActive(true);
        inputField.ActivateInputField();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _disposable?.Clear();
    }
}

