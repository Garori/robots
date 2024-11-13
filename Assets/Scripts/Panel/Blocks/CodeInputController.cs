using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CodeInputController : MonoBehaviour
{
	private TMPro.TMP_InputField inputField;
	public void Start()
	{
		inputField = GetComponent<TMPro.TMP_InputField>();
		inputField.onValueChanged.AddListener((_) => OnValueChanged());
	}

	public void OnValueChanged()
	{
		gameObject.transform.parent.GetChild(0).GetComponent<CodeController>().CodeWithin = inputField.text;
		// Debug.Log(gameObject.transform.parent.GetChild(0).GetComponent<CodeController>().CodeWithin);
	}
}
