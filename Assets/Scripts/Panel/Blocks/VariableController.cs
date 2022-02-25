using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VariableController : BlockController {
    protected override void OnBeginDragAction() {
        EventManager.onVariableExit(gameObject);
    }

    protected override void OnEndDragAction() {
        EventManager.onVariableEnter(gameObject, colliding);
    }

    protected override bool OnValidTriggerEnter2D(Collider2D other) {
        return other.CompareTag("VariableCollider");
    }

    protected override bool OnValidTriggerExit2D(Collider2D other) {
        return other.CompareTag("VariableCollider");
    }
}
