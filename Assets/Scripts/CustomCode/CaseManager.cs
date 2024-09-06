using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

[System.Serializable]
public class CaseManager
{
    public static List<TMP_InputField> variables;
    public int index;
    // public CustomCodeManager ccm;

    public CaseManager()
    {

    }
    public void carregarCase()
    {   
        Debug.Log("entrou no carregar");
        foreach (var variable in variables)
        {
            Debug.Log(variable);
        }
        // CustomCodeManager.ca

        // CustomCodeManager.carregarCase(variables);
    }

    public void Start()
    {
        
    }
}
