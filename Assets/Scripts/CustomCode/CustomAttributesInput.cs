using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAttributesInput : MonoBehaviour
{
    public int min;
    public int max;
    private TMPro.TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMPro.TMP_InputField>();
    }

    public void OnValueChanged()
    {
        int value;
        try
        {
            value = int.Parse(inputField.text);
        }
        catch (System.Exception)
        {
            value = min;
        }
        value = Mathf.Clamp(value, min, max);
        inputField.text = value.ToString();
    }
}
