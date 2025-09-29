using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

public class DragLetter : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("DragColors")]

    [SerializeField]
    UnityEngine.Color ColorOnDrag;

    [SerializeField]
    UnityEngine.Color ColorOnDragEnd;

    private Canvas canvas;
    private RectTransform rect;
    private TextMeshProUGUI label;
    private Image background;

    RectTransform parentAfterDrag; // Set Parent After Drag


    public string letter;//TODO TEMP PONER EN PRIV

    UIManager uIManager;

    private void Awake()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        background = GetComponent<Image>();
    }

    private void Start()
    {
        uIManager = UIManager.Instance;

    }

    public void Init(string letter)
    {
        this.letter = letter;

        if (label != null) label.text = letter;
    }
    public void OnPointerDown(PointerEventData eventData) {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag " + letter);

        parentAfterDrag = (RectTransform)transform.parent;

        transform.SetParent(uIManager.GetMainCanvas().transform);
        transform.SetAsLastSibling();

        background.color = ColorOnDrag;
        background.raycastTarget = false;
        GetComponentInParent<LetterSlot>().GetComponent<Image>().raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag " + letter);
        background.color = ColorOnDragEnd;
        background.raycastTarget = true;
        rect.SetParent(parentAfterDrag);
        transform.localPosition = new Vector3();
    }

    public void SetParentAfterDrag(RectTransform NewParent)
    {
        parentAfterDrag = NewParent;
        GetComponentInParent<LetterSlot>().GetComponent<Image>().raycastTarget = true;
    }

}