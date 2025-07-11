using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderColumn : MonoBehaviour, IPointerEnterHandler
{
    private Slider slider;

    [SerializeField] private float sliderSpeed = 0.5f;
    [SerializeField] private float deadzone = 0.3f;

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        if (slider != null)
        {
            slider.value = 0.5f;
        }
    }

    void Update()
    {
        if (slider != null && EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            float input1 = Input.GetAxisRaw("Horizontal1");
            float input2 = Input.GetAxisRaw("Horizontal2");
            float totalInput = input1 + input2;

            if (Mathf.Abs(totalInput) > deadzone)
            {
                slider.value += totalInput * sliderSpeed * Time.deltaTime;
                slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
            }
        }
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