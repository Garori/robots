using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComparatorController : BlockController {
    protected override void OnBeginDragAction() {
        EventManager.onComparatorExit(gameObject);
    }

    protected override void OnEndDragAction() {
        EventManager.onComparatorEnter(gameObject, colliding);
    }

    protected override bool OnValidTriggerEnter2D(Collider2D other) {
        return other.CompareTag("ComparatorCollider");
    }

    protected override bool OnValidTriggerExit2D(Collider2D other) {
        return other.CompareTag("ComparatorCollider");
    }
}
