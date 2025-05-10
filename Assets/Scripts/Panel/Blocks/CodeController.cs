using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CodeController : BlockController
{
	public string CodeWithin;
	protected override void OnBeginDragAction()
	{
		// Debug.Log("on begin drag de code");
		EventManager.onCodeExit(this);
	}

	protected override void OnEndDragAction()
	{
		// Debug.Log("aaaaaa");
		// EventManager.onCodeEnter(this, colliding);
	}

	protected override bool OnValidTriggerEnter2D(Collider2D other)
	{
		// Debug.Log("bbbbbb");
		return other.CompareTag("Line");
	}

	protected override bool OnValidTriggerExit2D(Collider2D other)
	{
		// Debug.Log("cccc");
		return other.CompareTag("Line");
	}
}
