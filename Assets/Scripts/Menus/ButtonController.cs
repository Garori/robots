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
    [SerializeField] private Image sizeMedal;
    [SerializeField] private Image roundsMedal;
    [SerializeField] private Color wonMedalColor;

    public void Init(int id, Action<int> OnClickAction, Medal medals)
    {
        this.id = id;
        this.OnClickAction = OnClickAction;
        buttonText.SetText((id + 1).ToString());
        CheckMedals(medals);
    }

    public void OnClick()
    {
        OnClickAction(id);
    }

    private void CheckMedals(Medal medals)
    {
        if (medals.roundsMedal)
        {
            roundsMedal.color = wonMedalColor;
        }

        if (medals.sizeMedal)
        {
            sizeMedal.color = wonMedalColor;
        }
    }
}
