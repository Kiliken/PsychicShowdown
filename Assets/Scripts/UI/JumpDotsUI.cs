using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpDotsUI : MonoBehaviour
{

    public PlayerMovement player;
    public Image[] jumpDots;
    public Color activeColor = Color.white;
    public Color usedColor = Color.gray;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < jumpDots.Length; i++)
        {
            jumpDots[i].color = i < player.jumpsLeft ? activeColor : usedColor;
        }
    }
}
