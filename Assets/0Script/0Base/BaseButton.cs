using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    public UnityEvent _onClickUnityAction;
    [SerializeField] private AudioClip _enterSe;
    [SerializeField] private AudioClip _clickedSe;
    [SerializeField] private Animator _animator;
    protected abstract void OnClicked();

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        PlayDawnAnimation();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
        PlayEnterSound();
        PlayEnterAnimation();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        PlayExitAnimation();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        _onClickUnityAction?.Invoke();
        OnClicked();
        PlayClickedSound();
        PlayClickAnimation();
    }

    //Play Sound
    public void PlayEnterSound()
    {
        if (_enterSe == null) return;
        SoundManager.Instance.PlaySE(_enterSe);
    }

    public void PlayClickedSound()
    {
        if (_clickedSe == null) return;
        SoundManager.Instance.PlaySE(_clickedSe);
    }

    //Play Animation
    public void PlayEnterAnimation()
    {
        if (_animator == null) return;
        _animator.Play("Enter");
    }

    public void PlayExitAnimation()
    {
        if (_animator == null) return;
        _animator.Play("Exit");
    }

    public void PlayDawnAnimation()
    {
        if (_animator == null) return;
        _animator.Play("Dawn");
    }

    public void PlayClickAnimation()
    {
        if (_animator == null) return;
        _animator.Play("Click");
    }
}