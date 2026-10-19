using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance { get; private set; }

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip successSound;
    private AudioSource audioSource;

    private AudioSource cameraAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.priority = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cameraAudioSource = Camera.main.GetComponent<AudioSource>();

        StartCoroutine(SetVolume());
    }

    private IEnumerator SetVolume()
    {
        while (true)
        {
            if(audioSource.volume!= PlayerPrefs.GetFloat("UI",1f))
                audioSource.volume = PlayerPrefs.GetFloat("UI",1f);

            if (cameraAudioSource != null)
            {
                if (cameraAudioSource.isPlaying)
                {
                    if (cameraAudioSource.volume != PlayerPrefs.GetFloat("Music", 0.5f))
                        cameraAudioSource.volume = PlayerPrefs.GetFloat("Music", 0.5f);
                }
            }
            else
            {
                cameraAudioSource = Camera.main.GetComponent<AudioSource>();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void PlayErrorSound()
    {
        if (errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    public void PlayWinSound()
    {
        if (winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }
    }

    public void PlaySuccessSound()
    {
        if (successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }
}
