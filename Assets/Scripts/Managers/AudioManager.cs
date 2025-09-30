using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;   // Para efectos de sonido
    [SerializeField] private AudioSource bgmSource;   // Para música de fondo

    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> sfxClips;
    [SerializeField] private List<AudioClip> bgmClips;

    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> bgmDict;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializar diccionarios
        sfxDict = new Dictionary<string, AudioClip>();
        bgmDict = new Dictionary<string, AudioClip>();

        foreach (var clip in sfxClips)
            if (clip != null) sfxDict[clip.name] = clip;

        foreach (var clip in bgmClips)
            if (clip != null) bgmDict[clip.name] = clip;
    }

    public void PlaySFX(string name, float volume = 1f)
    {
        if (!sfxDict.ContainsKey(name))
        {
            Debug.LogWarning($"SFX clip '{name}' not found!");
            return;
        }
        sfxSource.Stop();
        sfxSource.PlayOneShot(sfxDict[name], volume);
    }
    public void PlayBGM(string name, bool loop = true)
    {
        if (!bgmDict.ContainsKey(name))
        {
            Debug.LogWarning($"BGM clip '{name}' not found!");
            return;
        }

        bgmSource.clip = bgmDict[name];
        bgmSource.loop = loop;

        bgmSource.Stop();
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
