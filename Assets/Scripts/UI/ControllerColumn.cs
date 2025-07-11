using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerColumn : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Image psImage;
    [SerializeField] private Image xboxImage;

    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color hoveredColor = new Color(1f, 1f, 1f, 0.7f);
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.3f);

    private int selectedIndex = 0; // 0 = PS, 1 = Xbox
    private int hoveredIndex = -1;

    private float lastInputTime;
    private float inputCooldown = 0.3f;

    void Start()
    {
        UpdateVisuals();
    }

    void Update()
    {

        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            float h1 = Input.GetAxisRaw("Horizontal1");
            float h2 = Input.GetAxisRaw("Horizontal2");
            float h = h1 + h2;

            if (Time.time - lastInputTime > inputCooldown)
            {
                if (h > 0.5f)
                {
                    selectedIndex = 1;
                    lastInputTime = Time.time;
                }
                else if (h < -0.5f)
                {
                    selectedIndex = 0;
                    lastInputTime = Time.time;
                }

                UpdateVisuals();
            }

            if (Input.GetButtonDown("Jump1") || Input.GetButtonDown("Jump2")
                || Input.GetButtonDown("Jump1X") || Input.GetButtonDown("Jump2X"))
            {
                ApplySelection();
            }
        }
    }

    void UpdateVisuals()
    {
        SetImageState(psImage, 0);
        SetImageState(xboxImage, 1);
    }

    void SetImageState(Image img, int index)
    {
        if (selectedIndex == index)
            img.color = selectedColor;
        else if (hoveredIndex == index)
            img.color = hoveredColor;
        else
            img.color = normalColor;
    }

    public void OnHoverEnter(int index)
    {
        hoveredIndex = index;
        UpdateVisuals();
    }

    public void OnHoverExit(int index)
    {
        hoveredIndex = -1;
        UpdateVisuals();
    }

    public void OnClick(int index)
    {
        selectedIndex = index;
        ApplySelection();
        UpdateVisuals();
    }

    private void ApplySelection()
    {
        Debug.Log($"Platform selected: {(selectedIndex == 0 ? "PlayStation" : "Xbox")}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            //EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("Pointer entered: " + gameObject.name);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}