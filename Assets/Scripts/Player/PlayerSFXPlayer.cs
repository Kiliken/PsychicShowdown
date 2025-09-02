using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXPlayer : MonoBehaviour
{
    Player playerScript;
    AudioSource audioSource;
    [SerializeField] private float defaultVolume = 0.1f;
    [SerializeField] AudioClip[] playerAudio;   // 0: , 1: , 2:


    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = defaultVolume;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // 0: jump, 1: dash, 2: pick up, 3: throw
    public void PlaySFX(int sfx)
    {
        //audioSource.Stop();
        // adjust volumes for each sfx
        // switch (sfx)
        // {
        //     case 0:
        //         audioSource.volume = 0.01f;
        //         break;
        // }
        audioSource.clip = playerAudio[sfx];
        audioSource.Play();
        playerScript.soundFlag++;
    }

    public void StopSFX()
    {
        audioSource.Stop();
    }
}
