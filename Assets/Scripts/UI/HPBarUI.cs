using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    public Image hpFillImage;
    public Player player; // Reference to your player

    void Update()
    {
        if (player == null) return;

        float fillAmount = (float)player.hp / player.maxHP;
        hpFillImage.fillAmount = fillAmount;
    }
}
