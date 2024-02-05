using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
	public bool isEnabled { get; private set; }

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

		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		boxCollider2D = GetComponent<BoxCollider2D>();
		image = GetComponent<Image>();
	}

	private void Start()
	{
		canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
		panelManager = GameObject.FindGameObjectWithTag("PanelManager").GetComponent<PanelManager>();

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
		if (newBlock)
		{
			GameObject createdBlock = Instantiate(gameObject, gameObject.transform.parent);
			createdBlock.name = gameObject.name;
			createdBlock.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex());
			createdBlock.GetComponent<BlockController>().isEnabled = isEnabled;
			newBlock = false;
			TooltipTrigger tooltipTrigger = createdBlock.GetComponent<TooltipTrigger>();
			if (tooltipTrigger != null)
			{
				tooltipTrigger.isTooltipEnabled = false;
			}
		}
		isEnabled = true;
		canvasGroup.alpha = .8f;
		ResetParent();
		OnBeginDragAction();
	}

	protected virtual void OnBeginDragAction()
	{

	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
		boxCollider2D.enabled = (rectTransform.anchoredPosition.x + scaledWidth) >= panelManager.panelX;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		canvasGroup.alpha = 1f;
		if (colliding == null)
		{
			Destroy(gameObject);
			return;
		}
		Debug.Log("Colidiu com " + colliding.name);
		OnEndDragAction();
	}

	protected virtual void OnEndDragAction()
	{

	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!OnValidTriggerEnter2D(other)) return;
		colliding = other.gameObject;
	}

	private void OnTriggerExit2D(Collider2D other)
	{
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
