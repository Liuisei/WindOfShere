
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-100)]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] AudioSource _bgmAudioSource;
    [SerializeField] AudioSource _seAudioSource;

    protected  void Awake()
    {
        Instance = this;
        if (_bgmAudioSource == null) Debug.LogError("BGM AudioSource is null.");
        if (_seAudioSource == null) Debug.LogError("SE AudioSource is null.");
    }

    /// <summary> BGMを再生 </summary> ///
    public void PlayBGM(AudioClip audioClipBGM)
    {
        _bgmAudioSource.clip = audioClipBGM;
        _bgmAudioSource.Play();
    }

    /// <summary> SEを再生 </summary> ///
    public void PlaySE(AudioClip audioClipSE)
    {
        _seAudioSource.PlayOneShot(audioClipSE);
    }

    public void MuteBGM(bool mute)
    {
        _bgmAudioSource.mute = mute;
    }

    public void MuteSE(bool mute)
    {
        _seAudioSource.mute = mute;
    }

    public void SetVolumeBGM(float volume)
    {
        _audioMixer.SetFloat("BGM", volume);
    }
}