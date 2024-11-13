using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionController : BlockController
{
    protected override void OnBeginDragAction()
    {
        Debug.Log("OnBeginDragAction");
        EventManager.onBlockExit(this);
    }

    protected override void OnEndDragAction()
    {
        Debug.Log("OnEndDragAction");
        EventManager.onBlockEnter(this, colliding);
    }

    protected override bool OnValidTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnValidTriggerEnter2D");
        return other.CompareTag("Line");
    }

    protected override bool OnValidTriggerExit2D(Collider2D other)
    {
        Debug.Log("OnValidTriggerExit2D");
        return other.CompareTag("Line");
    }
}
