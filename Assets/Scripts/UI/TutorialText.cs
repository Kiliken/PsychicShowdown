using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    [SerializeField] private string text1;
    [SerializeField] private string text2;

    private TMP_Text tmp;
    private bool showingFirst = true;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        if (tmp != null)
        {
            tmp.text = text1; 
        }
    }

    public void ToggleText(bool en)
    {
        if (tmp == null) return;

        if (en)
        {
            tmp.text = text1;

        }
        else
        {
            tmp.text = text2;

        }
    }
}
