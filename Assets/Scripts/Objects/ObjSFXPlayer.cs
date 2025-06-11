using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSFXPlayer : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] bool relativeToPlayer = true;
    [SerializeField] float lowVolume = 0.01f;
    [SerializeField] float medVolume = 0.5f;
    [SerializeField] float highVolume = 0.1f;
    [SerializeField] float nearDist = 20f;
    [SerializeField] float farDist = 60f;
    Transform player1Pos;
    Transform player2Pos;
    // float player1Dist;
    // float player2Dist;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Start is called before the first frame update
    void Start()
    {
        player1Pos = GameObject.FindGameObjectWithTag("Player1").transform;
        player2Pos = GameObject.FindGameObjectWithTag("Player2").transform;
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySFX(int sfx)
    {
        audioSource.clip = audioClips[sfx];
        if (relativeToPlayer)
        {
            // player1Dist = Vector3.Distance(transform.position, player1Pos.position);
            // player2Dist = Vector3.Distance(transform.position, player2Pos.position);

            float nearestPos = Mathf.Min(Vector3.Distance(transform.position, player1Pos.position), Vector3.Distance(transform.position, player2Pos.position));

            if (nearestPos >= farDist)
                audioSource.volume = lowVolume;
            else if (nearestPos > nearDist && nearestPos < farDist)
                audioSource.volume = medVolume;
            else
                audioSource.volume = highVolume;

        }
        audioSource.Play();
    }
}
