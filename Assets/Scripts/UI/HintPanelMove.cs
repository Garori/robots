using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HintPanelMove : MonoBehaviour
{

    [Header("Hint Panel")]
    [SerializeField] private float showHintPositionX;
    [SerializeField] private float hideHintPositionX;
    [SerializeField] private TMPro.TextMeshProUGUI hintPanelText;
    private bool isShowing;
    private bool canShow;

    void Awake()
    {
        gameObject.SetActive(false);
        canShow = false;
        HideHintPanel();
    }

    public void TogglePanel()
    {
        if (!canShow) return;
        if (isShowing)
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
        isShowing = true;
        gameObject.SetActive(true);
        transform.DOLocalMoveX(showHintPositionX, 0.5f, false);
    }

    public void HideHintPanel()
    {
        isShowing = false;
        transform.DOLocalMoveX(hideHintPositionX, 0.5f, false);
    }

    public void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) 
        {
            canShow = false;
            return;
        }
        hintPanelText.text = text;
        canShow = true;
    }
}
