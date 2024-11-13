using System.Globalization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



public class CustomAttributesInput : MonoBehaviour
{
    public int min;
    public int max;
    private TMPro.TMP_InputField inputField;

    public void Start()
    {
        inputField = GetComponent<TMPro.TMP_InputField>();
        inputField.onDeselect.AddListener((_) => OnDeselect()); 
    }
    public void OnValueChanged()
    {
        int value;
        string valueSTR = "";
        try
        {
            if (inputField == null)
            {
                inputField = GetComponent<TMPro.TMP_InputField>();
            }
            valueSTR = inputField.text;
            if (valueSTR != "")
            {
                value = int.Parse(valueSTR);
            }
            else
            {
                value = min;
            }
        }
        catch (System.Exception)
        {
            value = min;
        }

        if (valueSTR != "")
        {
            value = Mathf.Clamp(value, min, max);
            inputField.text = value.ToString();
            
        }
    }

    public void OnDeselect()
    {
        int value;
        try
        {
            if (inputField == null)
            {
                inputField = GetComponent<TMPro.TMP_InputField>();
            }
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
