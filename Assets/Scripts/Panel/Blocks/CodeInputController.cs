using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using Unity.Mathematics;
using TMPro;
using System;

public class CodeInputController : BlockController
{
	private TMPro.TMP_InputField inputField;
	public string CodeWithin;
	private bool toPreventfallback = false;
	bool lastKeyWasBackspace = false;
	public void Start()
	{
		inputField = GetComponent<TMPro.TMP_InputField>();
		inputField.onValueChanged.AddListener((_) => OnValueChanged());
	}
	private void OnGUI() 
	{
		Event e = Event.current;
		if (e.isKey)
		{
			if(e.keyCode == KeyCode.Backspace)
			{
				lastKeyWasBackspace = true;
			}
			else
			{
				lastKeyWasBackspace = false;
			}
			// Debug.Log(lastKeyWasBackspace);
		}
	}

	public void teste()
	{
		// inputField.text = inputField.text.Insert(1, " ");
		// UnityEventTools.RemovePersistentListener(inputField.onValueChanged, 0);
	}
	// void Update()
	// {
	// 	Debug.Log($"caret pos -> {inputField.caretPosition}");
	// }
	public void OnValueChanged()
	{
		gameObject.transform.GetChild(0).GetComponent<CodeController>().CodeWithin = inputField.text;
		this.CodeWithin = inputField.text;
		if (inputField.caretPosition == 0){return;}
		// int posAtual = inputField.caretPosition;
		// string toCompare = inputField.text;
		// inputField.text = Regex.Replace(inputField.text, @"<(\/)?color(?(1)|=#[ABCEDF0-9]{6,8})>", "", RegexOptions.Multiline);
		// string regexPattern = @"^[ ]*If\b";
		// // regexPattern = "If";
		// inputField.text = Regex.Replace(inputField.text, regexPattern, "<color=#FF0000>If</color>", RegexOptions.Multiline);
		// if(posAtual >=0 ){
		// 	inputField.caretPosition = posAtual +1;

		// }
		// if (toCompare != inputField.text){
		// 	print("entrei");
		// 	inputField.caretPosition += 21;
		// 	Debug.Log($"caret pos -> {inputField.caretPosition - 1}");
		// }
		// Debug.Log("" + inputField.text);
		// Debug.Log("caret -> "+ inputField.text[inputField.caretPosition-1]);
		// Debug.Log($"caret pos -> {inputField.caretPosition - 1}");
		// Debug.Log($"{inputField.caretPosition -1} {inputField.text.Length -1}");
		int posAtual = inputField.caretPosition - 1;
		int chaves_abertas = inputField.text.Substring(0,posAtual).Count(c => c == '{');
		int chaves_fechadas = inputField.text.Substring(0, posAtual).Count(c => c == '}');
		int balanco = math.max((chaves_abertas - chaves_fechadas) * 2, 0);
		// Debug.Log(balanco);
		if (inputField.text[posAtual] == '\n' && !lastKeyWasBackspace)
		{
			if (posAtual == inputField.text.Length - 1)
			{
				inputField.onValueChanged.RemoveAllListeners();
				inputField.text += new string(' ', balanco);
				inputField.caretPosition += balanco;
				inputField.selectionAnchorPosition += balanco;
				inputField.stringPosition += balanco;
				inputField.onValueChanged.AddListener((_) => OnValueChanged());
			}
			else
			{
				// Debug.Log("inputField.text[i] " + inputField.text[posAtual+1]);
				inputField.onValueChanged.RemoveAllListeners();
				inputField.text = inputField.text.Insert(posAtual+1, new string(' ', balanco));
				inputField.caretPosition += balanco;
				inputField.selectionAnchorPosition += balanco;
				inputField.stringPosition += balanco;
				inputField.onValueChanged.AddListener((_) => OnValueChanged());
			}
		}
		if (posAtual >= 2 && inputField.text[posAtual] == '}'&& inputField.text[posAtual-1] == ' ' && inputField.text[posAtual-2] == ' ' && balanco >= 2)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.caretPosition -= 2;
			inputField.selectionAnchorPosition -= 2;
			inputField.stringPosition -= 2;
			inputField.text = inputField.text.Remove(posAtual - 2,2);
			// inputField.text = inputField.text.Remove(posAtual - 1);
			inputField.onValueChanged.AddListener((_) => OnValueChanged());
		}
		

		// inputField.text.Contains()
		gameObject.transform.GetChild(0).GetComponent<CodeController>().CodeWithin = inputField.text;

		this.CodeWithin = inputField.text;
		// inputField.onValueChanged.re
		// Debug.Log(gameObject.transform.parent.GetChild(0).GetComponent<CodeController>().CodeWithin);
	}

	public void SetInputField(string code)
	{
		if (inputField == null)
		{
			inputField = GetComponent<TMPro.TMP_InputField>();
			inputField.onValueChanged.AddListener((_) => OnValueChanged());
		}
		inputField.text = code;
	}

	protected override void OnBeginDragAction()
	{
		Debug.Log("on begin drag de código input");
		EventManager.onCodeExit(this);
	}

	protected override void OnEndDragAction()
	{
		Debug.Log("acabou o drag do codigo");
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
