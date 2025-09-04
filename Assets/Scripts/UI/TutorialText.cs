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

    public void ToggleText()
    {
        if (tmp == null) return;

        if (showingFirst)
        {
            tmp.text = text2;
            showingFirst = false;
        }
        else
        {
            tmp.text = text1;
            showingFirst = true;
        }
    }
}
