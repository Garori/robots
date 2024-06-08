using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CodeController : BlockController
{
	// public 
	protected override void OnBeginDragAction()
	{
		EventManager.onCodeExit(this);
	}

	protected override void OnEndDragAction()
	{
		EventManager.onCodeEnter(this, colliding);
	}

	protected override bool OnValidTriggerEnter2D(Collider2D other)
	{
		return other.CompareTag("Line");
	}

	protected override bool OnValidTriggerExit2D(Collider2D other)
	{
		return other.CompareTag("Line");
	}
}
