using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollision : MonoBehaviour {
    private BlockController parentScript;

    private void Start() {
        parentScript = gameObject.GetComponentInParent<BlockController>();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        parentScript.OnChildCollisionEnter(gameObject, other);
    }

    private void OnCollisionExit2D(Collision2D other) {
        parentScript.OnChildCollisionExit(gameObject, other);
    }
}
