using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LetterSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    bool bCanPlace = true; 
    private string correctLetter;  // letra que debe ir aquí

    DragLetter currentDragLetter = null;

    public void Init(string correctLetter, bool bCanPlace = true)
    {
        this.correctLetter = correctLetter;
        this.bCanPlace = bCanPlace;

        if (!bCanPlace)
        {
            GameObject obj = new GameObject("Text");
            obj.transform.SetParent(transform);
            TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
            text.text = correctLetter;
            text.enableAutoSizing = true;
            text.alignment = TextAlignmentOptions.Center;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount < 0 || !bCanPlace) return;

        // Comprobar si lo que se dropea tiene un DragLetter
        DragLetter dragLetter = eventData.pointerDrag?.GetComponent<DragLetter>();
        if (dragLetter == null) return;

        if (dragLetter.letter != correctLetter)
        {
            Debug.Log($"Slot: Incorrecto");
            return;
        }

        //dragLetter.transform.position = transform.position;
        dragLetter.SetParentAfterDrag((RectTransform)transform);
        currentDragLetter = dragLetter;

        Debug.Log($"Slot: ¡Correcto!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!bCanPlace) return;

        Debug.Log("ObjectEntrando");
    }
}
