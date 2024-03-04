using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelPuller : MonoBehaviour
{
    [Header("Panel Variables")]
    [SerializeField] private GameObject blocksArea;
    [SerializeField] private float openPositionY;
    [SerializeField] private float closedPositionY;

    [Header("Panel Status")]
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private float statusOpenPositionY;
    [SerializeField] private float statusClosedPositionY;

    private bool isOpen;

    void Start()
    {
        isOpen = true;
    }

    public void TogglePanel()
    {
        if (isOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    private void OpenPanel()
    {
        isOpen = true;
        blocksArea.SetActive(true);
        transform.DOLocalMoveY(openPositionY, 0.5f, false);

        statusPanel.transform.DOLocalMoveY(statusOpenPositionY, 0.5f, false);
    }

    private void ClosePanel()
    {
        isOpen = false;
        transform.DOLocalMoveY(closedPositionY, 0.5f, false).OnComplete(() => blocksArea.SetActive(false));

        statusPanel.transform.DOLocalMoveY(statusClosedPositionY, 0.5f, false);
    }
}
