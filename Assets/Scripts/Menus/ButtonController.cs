using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    private int id;
    private Action<int> OnClickAction;
    [SerializeField] private TMP_Text buttonText;

    public void Init(int id, Action<int> OnClickAction)
    {
        this.id = id;
        this.OnClickAction = OnClickAction;
        buttonText.SetText((id + 1).ToString());
    }

    public void OnClick()
    {
        OnClickAction(id);
    }
}
