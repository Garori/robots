using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HintPanelMove : MonoBehaviour
{

    [Header("Hint Panel")]
    [SerializeField] private float showHintPositionX;
    [SerializeField] private float hideHintPositionX;
    private TMPro.TextMeshProUGUI hintPanelText;
    private bool isOpen;
    private bool canShow;

    void Start()
    {
        isOpen = false;
        canShow = true;
        HideHintPanel();
        hintPanelText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public void TogglePanel()
    {
        if (isOpen)
        {
            HideHintPanel();
        }
        else
        {
            ShowHintPanel();
        }
    }

    public void ShowHintPanel()
    {
        gameObject.SetActive(canShow);
        transform.DOLocalMoveX(showHintPositionX, 0.5f, false);
    }

    public void HideHintPanel()
    {
        transform.DOLocalMoveX(hideHintPositionX, 0.5f, false);
    }

    public void SetText(string text)
    {
        hintPanelText.text = text;
        canShow = text.Length > 0;
    }
}
