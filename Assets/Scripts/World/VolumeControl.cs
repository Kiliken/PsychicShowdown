using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeControl : MonoBehaviour
{
    private AudioSource audioSource;
    private GameSettings gameSettings;
    // Start is called before the first frame update
    void Start()
    {
        gameSettings = GameObject.FindGameObjectWithTag("GameSettings").GetComponent<GameSettings>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = gameSettings.soundVolume / 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
