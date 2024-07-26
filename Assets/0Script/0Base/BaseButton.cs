using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// ボタンのクリック時に音を鳴らす機能や、アニメーションを再生する。
/// public UnityEvent _onClickUnityAction;でクリック時のUnityEventを設定できる。
/// アニメーションOBJとクリックの判定範囲を分けて下さい。
/// </summary>
public abstract class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler
{
    public UnityEvent _onClickUnityAction;
    [SerializeField] private AudioClip _enterSe;
    [SerializeField] private AudioClip _clickedSe;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _clickCoolTime = 1f;
    private bool _isButtonLocked;

    /// <summary>
    /// クリック時の処理を実装する。
    /// </summary>
    protected abstract void OnClicked();

    private bool AnimatorChack()
    {
        if (_animator == null) return true;
        return false;
    }

    private bool LockCheck()
    {
        if (_isButtonLocked) return true;
        return false;
    }

    private IEnumerator LockAnimationForSeconds(float seconds)
    {
        _isButtonLocked = true;
        yield return new WaitForSeconds(seconds);
        _isButtonLocked = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (LockCheck()) return;
        PlayDawnAnimation();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LockCheck()) return;
        PlayEnterSound();
        PlayEnterAnimation();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (LockCheck()) return;
        PlayExitAnimation();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (LockCheck()) return;
        _onClickUnityAction?.Invoke();
        OnClicked();
        PlayClickedSound();
        PlayClickAnimation();
        StartCoroutine(LockAnimationForSeconds(_clickCoolTime));
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
        if (AnimatorChack()) return;
        _animator.Play("Enter");
    }

    private void PlayExitAnimation()
    {
        if (AnimatorChack()) return;
        _animator.Play("Exit");
    }

    private void PlayDawnAnimation()
    {
        if (AnimatorChack()) return;
        _animator.Play("Dawn");
    }

    private void PlayClickAnimation()
    {
        if (AnimatorChack()) return;
        _animator.Play("Click");
    }
}