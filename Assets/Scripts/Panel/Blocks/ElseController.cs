using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElseController : BlockController {
    protected override void OnBeginDragAction() {
        EventManager.onBlockExit(gameObject);
    }

    protected override void OnEndDragAction() {
        EventManager.onBlockEnter(gameObject, colliding);
    }

    protected override bool OnValidTriggerEnter2D(Collider2D other) {
        return other.CompareTag("Line");
    }

    protected override bool OnValidTriggerExit2D(Collider2D other) {
        return other.CompareTag("Line");
    }
}
