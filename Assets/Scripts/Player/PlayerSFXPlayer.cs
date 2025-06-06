using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXPlayer : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] private float defaultVolume = 0.1f;
    [SerializeField] AudioClip[] playerAudio;   // 0: , 1: , 2:


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySFX(int sfx)
    {
        //audioSource.Stop();
        // adjust volumes for each sfx
        switch (sfx)
        {
            case 0:
                audioSource.volume = 0.01f;
                break;
        }
        audioSource.clip = playerAudio[sfx];
        audioSource.Play();
    }
}
