using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance { get; private set; }

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    private AudioSource audioSource;

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

            Vibration.Vibrate();
        }
    }

    public void PlaySuccessSound()
    {
        if (successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
    }

    public void PlayWinSound()
    {
        if (winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }
    }

    public void PlayLoseSound()
    {
        if (loseSound != null)
        {
            audioSource.PlayOneShot(loseSound);
        }
    }
}
