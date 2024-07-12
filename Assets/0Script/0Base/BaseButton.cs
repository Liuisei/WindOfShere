using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// ボタンのクリック時に音を鳴らす機能や、アニメーションを再生する。
/// public UnityEvent _onClickUnityAction;でクリック時のUnityEventを設定できる。
/// </summary>

public abstract class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    public UnityEvent _onClickUnityAction;
    [SerializeField] private AudioClip _enterSe;
    [SerializeField] private AudioClip _clickedSe;
    [SerializeField] private Animator _animator;
    private bool _isAnimatingLocked ;

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
    private void PlayEnterSound()
    {
        if (_enterSe == null) return;
        SoundManager.Instance.PlaySE(_enterSe);
    }

    private void PlayClickedSound()
    {
        if (_clickedSe == null) return;
        SoundManager.Instance.PlaySE(_clickedSe);
    }
    //Play Animation
    private void PlayEnterAnimation()
    {
        if (AnimatorAndLockedChack()) return;
        _animator.Play("Enter");
    }

    private void PlayExitAnimation()
    {
        if (AnimatorAndLockedChack()) return;
        _animator.Play("Exit");
    }

    private void PlayDawnAnimation()
    {
        if (AnimatorAndLockedChack()) return;
        _animator.Play("Dawn");
    }

    private void PlayClickAnimation()
    {
        if (AnimatorAndLockedChack()) return;
        _animator.Play("Click");
        StartCoroutine(LockAnimationForSeconds(1f));
    }

    private bool AnimatorAndLockedChack()
    {
        if (_animator == null || _isAnimatingLocked ) return true;
        return false;
    }

    private IEnumerator LockAnimationForSeconds(float seconds)
    {
        _isAnimatingLocked = true;
        yield return new WaitForSeconds(seconds);
        _isAnimatingLocked = false;
    }
}