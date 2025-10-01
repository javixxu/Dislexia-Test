using DG.Tweening.Core.Easing;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LetterSlot : MonoBehaviour, IDropHandler
{
    bool bCanPlace = true; 

    string correctLetter;
    int index;

    DragLetter currentDragLetter = null;

    [SerializeField]Color letterColor;

    public void Init(string correctLetter,int index = -1, bool bCanPlace = true)
    {
        this.correctLetter = correctLetter;
        this.bCanPlace = bCanPlace;
        this.index = index;

        if (!bCanPlace && correctLetter!="")
        {
            GameObject obj = new GameObject("Text");
            obj.transform.SetParent(transform);
            TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
            text.text = correctLetter;
            text.color = letterColor;
            text.enableAutoSizing = true;
            text.alignment = TextAlignmentOptions.Center;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!bCanPlace) return;

        // Comprobar si lo que se dropea tiene un DragLetter
        DragLetter dragLetter = eventData.pointerDrag?.GetComponent<DragLetter>();
        if (dragLetter == null) return;

        if (dragLetter.GetCurrentLetter() != correctLetter)
        {
            GameManager.Instance.currentExercise.CheckSolution(-1);

            Debug.Log($"Slot: Incorrecto");
            return;
        }

        Destroy(transform.GetChild(0)?.gameObject);

        //dragLetter.transform.position = transform.position;
        dragLetter.SetParentAfterDrag((RectTransform)transform);
        currentDragLetter = dragLetter;

        GameManager.Instance.currentExercise.CheckSolution(index);

        Debug.Log($"Slot: ¡Correcto!");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, mousePos))
            {
                Debug.Log($"Mouse clicked on slot: {correctLetter}");
            }
        }
    }
}
