using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

public class BlockController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private PanelManager panelManager;
	private Canvas canvas;
	private RectTransform canvasTransform;

	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	private BoxCollider2D boxCollider2D;
	private Image image;

	private float scaledWidth;
	public bool isEnabled { get; set; }

	protected GameObject colliding;
	private bool newBlock;
	public bool isInPanel;

	public bool canDisable;

	[Header("Enum")]
	public Commands commandName;

	void Awake()
	{
		newBlock = true;
		isInPanel = false;
		isEnabled = true;
		// if(gameObject.tag == "CodeInputBlock")
		// {
		// 	rectTransform = gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
		// }
		// else
		// {
		rectTransform = GetComponent<RectTransform>();
	// }
		canvasGroup = GetComponent<CanvasGroup>();
		boxCollider2D = GetComponent<BoxCollider2D>();
		image = GetComponent<Image>();
		canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
		panelManager = canvas.gameObject.transform.Find("Panel").GetComponent<PanelManager>();

		// panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();

		canvasTransform = canvas.GetComponent<RectTransform>();
		scaledWidth = rectTransform.sizeDelta.x * rectTransform.localScale.x / 2f;
	}

	private void Start()
	{
		canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
		panelManager = canvas.gameObject.transform.Find("Panel").GetComponent<PanelManager>();

		// panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();

		canvasTransform = canvas.GetComponent<RectTransform>();
		scaledWidth = rectTransform.sizeDelta.x * rectTransform.localScale.x / 2f;
	}

	void Update()
	{
		if (isEnabled)
		{
			image.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			image.color = new Color(1f, 1f, 1f, 0.5f);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Right) return;
		if (isInPanel) return;
		if (!canDisable) return;

		isEnabled = !isEnabled;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{	
		if (eventData.button != PointerEventData.InputButton.Left) return;
		if (!isInPanel)
		{
			// GameObject[] gos = {gameObject};
			// Selection.objects = gos;
			// Unsupported.DuplicateGameObjectsUsingPasteboard();
			GameObject duplicate = Instantiate(gameObject);
			duplicate.transform.SetParent(transform.parent, false);
			duplicate.name = name;
		}
		// try
		// {
		canvasGroup.blocksRaycasts = false;
		// }
		// catch (Exception e)
		// {
		// 	canvasGroup = gameObject.transform.parent.GetChild(0).gameObject.GetComponent<CanvasGroup>();
		// 	canvasGroup.blocksRaycasts = false;
		// }
		isEnabled = true;
		canvasGroup.alpha = .5f;
		ResetParent();
		OnBeginDragAction();
	}

	protected virtual void OnBeginDragAction()
	{

	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		// if (gameObject.tag == "CodeBlock")
		// {
			
		// }
		// Debug.Log(rectTransform);
		// Debug.Log(eventData.delta);
		// Debug.Log(canvas);
		
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
		// Debug.Log(transform.parent.tag);
		// boxCollider2D.enabled = (rectTransform.anchoredPosition.x + scaledWidth) >= panelManager.panelX;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
		Debug.Log("on end drag block controller obj: "+ gameObject.tag);
		Debug.Log("on end drag block controller caiu em:" + transform.parent.tag);
		if ((gameObject.tag == "CodeInputBlock" ||  gameObject.tag == "CodeBlock" || gameObject.tag == "ActionBlock" || gameObject.tag == "StructureBlock" || gameObject.tag == "EndBlock" || gameObject.tag == "ElseBlock" || gameObject.tag == "BreakBlock") && transform.parent.tag != "Line" || (gameObject.tag == "VariableBlock") && !(transform.parent.tag == "VariableCollider" || transform.parent.tag == "ForCondition") || (gameObject.tag == "ComparatorBlock") && transform.parent.tag != "ComparatorCollider")
		{
			Destroy(gameObject);
		}
		OnEndDragAction();
	}

	protected virtual void OnEndDragAction()
	{

	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("OnTriggerEnter2D");
		if (!OnValidTriggerEnter2D(other)) return;
		colliding = other.gameObject;
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		Debug.Log("OnTriggerExit2D");
		if (!OnValidTriggerExit2D(other)) return;
		colliding = null;
	}

	protected virtual bool OnValidTriggerEnter2D(Collider2D other)
	{
		return true;
	}

	protected virtual bool OnValidTriggerExit2D(Collider2D other)
	{
		return true;
	}

	public void SetParent(Transform parent)
	{
		// Debug.Log("parent = " + parent);
		// Debug.Log("rect = " + rectTransform);
		rectTransform = GetComponent<RectTransform>();
		rectTransform.SetParent(parent);
		rectTransform.anchoredPosition = Vector2.zero;
	}

	protected void ResetParent()
	{
		rectTransform.SetParent(canvasTransform);
	}

	public void SetInPanel()
	{
		this.newBlock = false;
		this.isInPanel = true;
	}
}
