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
   [SerializeField] private TextMeshProUGUI label;
    private Image background;

    //Parent And Transform
    bool CheckOrderReparent;
    [SerializeField] 
    private GameObject placeholderPrefab; // asignar en inspector
    private GameObject placeholderInstance;
    private int originalIndex;
    RectTransform parentAfterDrag; // Set Parent After Drag

    string letter;
    UIManager uIManager;


    private void Start()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        background = GetComponent<Image>();

        uIManager = UIManager.Instance;
        label.raycastTarget = false;
    }

    public void Init(string letter,bool checkOrderReparent = false)
    {
        this.letter = letter;
        if (label != null) label.text = letter;
        else Debug.LogError("Label is null in DragLetter");
        this.CheckOrderReparent = checkOrderReparent;
    }
    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX("fx_click");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag " + letter);

        parentAfterDrag = (RectTransform)transform.parent;

        if (CheckOrderReparent)
        {
            ManagePlaceHolder();
        }

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        background.color = ColorOnDrag;
        background.raycastTarget = false;
      
    }
    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //transform.position = Input.mousePosition;

        if (!CheckOrderReparent || placeholderInstance == null) return;

        // Reordenar placeholder según posición X
        int newIndex = parentAfterDrag.childCount;

        for (int i = 0; i < parentAfterDrag.childCount; i++)
        {
            Transform sibling = parentAfterDrag.GetChild(i);
            if (sibling == transform || sibling == placeholderInstance.transform) continue;

            RectTransform siblingRect = sibling as RectTransform;

            // Calcular centro real del drag y del slot vecino
            float dragCenterX = rect.anchoredPosition.x;
            float siblingCenterX = sibling.position.x;

            //Debug.Log($"DragCenterX: {dragCenterX}, SiblingCenterX: {siblingCenterX}");

            if (dragCenterX <= siblingCenterX)
            {
                newIndex = i;
                break;
            }
        }

        placeholderInstance.transform.SetSiblingIndex(newIndex);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag " + letter);
        background.raycastTarget = true;
        background.color = ColorOnDragEnd;

        if (CheckOrderReparent)
        {
            int newIndex = placeholderInstance.transform.GetSiblingIndex();

            rect.SetParent(parentAfterDrag);
            rect.SetSiblingIndex(newIndex);
            rect.localPosition = Vector3.zero;

            placeholderInstance.SetActive(false);

            // If the item moved call CheckSolution
            if (newIndex != originalIndex)
            {
                GameManager.Instance.currentExercise?.CheckSolution(-1);
            }
        }
        else
        {
            rect.SetParent(parentAfterDrag);
            rect.localPosition = Vector3.zero;
        }
    }

    public void SetParentAfterDrag(RectTransform NewParent)
    {
        parentAfterDrag = NewParent;
    }

    public string GetCurrentLetter() { return letter; }

    void ManagePlaceHolder()
    {
        originalIndex = transform.GetSiblingIndex();

        if (placeholderInstance == null)
        {
            placeholderInstance = Instantiate(placeholderPrefab, parentAfterDrag);
        }

        placeholderInstance.SetActive(true);
        placeholderInstance.transform.SetParent(parentAfterDrag);
        placeholderInstance.transform.SetSiblingIndex(originalIndex);

        // Ajustar tamaño
        LayoutElement le = placeholderInstance.GetComponent<LayoutElement>();
        if (le != null)
        {
            le.preferredWidth = rect.rect.width;
            le.preferredHeight = rect.rect.height;
        }
    }
}