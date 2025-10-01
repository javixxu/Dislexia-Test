using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CutSentenceExercise : ExerciseBase, IPointerDownHandler
{
    [Header("UI References")]
    [SerializeField] private RectTransform container; // panel con HorizontalLayoutGroup
    [SerializeField] private TextMeshProUGUI textPrefab; // prefab del TMP segment
    [SerializeField] private GameObject spacePrefab; // prefab de la “imagen espacio” clicable

    private string sentence;
    private string correctSentence;

    private List<GameObject> allSegments = new List<GameObject>(); // TMPs o imagenes

    public override void Show(Exercise data)
    {
        correctSentence = data.sentence;
        sentence = correctSentence.Replace(" ", "");

        SetupUI();
    }

    public override void CheckSolution(int index)
    {
        string userSentence = "";

        foreach (var segment in allSegments)
        {
            TextMeshProUGUI tmp = segment.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                userSentence += tmp.text;
            else userSentence += " ";
        }

        if (userSentence == correctSentence)
        {
            Debug.Log("¡CORRECTO! " + userSentence);
            GameManager.Instance.UpdateAnswersCounter(true);
            StartCoroutine(EndAfter(0.7f));
        }
    }
    protected override void SetupUI()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // Crear un TMP por cada letra
        for (int i = 0; i < sentence.Length; i++)
        {
            var tmp = Instantiate(textPrefab, container);
            tmp.text = sentence[i].ToString();
            tmp.raycastTarget = true; // importante para detectar clicks
            allSegments.Add(tmp.gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObj = eventData.pointerCurrentRaycast.gameObject;
        if (clickedObj == null) return;

        int globalIndex = allSegments.IndexOf(clickedObj);
        if (globalIndex < 0) return;

        AudioManager.Instance.PlaySFX("fx_click", 0.8f);

        RectTransform tmpRect = clickedObj.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            tmpRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        UpdateSegments(clickedObj, localPoint);

        CheckSolution(-1); // comprobar solucion tras cada cambio
    }

    private void UpdateSegments(GameObject clickedSegment, Vector2 localPoint)
    {
        int globalIndex = allSegments.IndexOf(clickedSegment);
        if (globalIndex < 0) return;

        bool insertRight = localPoint.x >= 0;

        // Si es un espacio (imagen), eliminar
        if (clickedSegment.GetComponent<Image>())
        {
            allSegments.RemoveAt(globalIndex);
            Destroy(clickedSegment);
            Debug.Log($"Removed space at index {globalIndex}");
            return;
        }

        // Crear un nuevo espacio como imagen
        var spaceGO = Instantiate(spacePrefab, container);

        int insertIndex = insertRight ? globalIndex + 1 : globalIndex;
        spaceGO.transform.SetSiblingIndex(insertIndex);

        allSegments.Insert(insertIndex, spaceGO);

        if (!insertRight)
            clickedSegment.transform.SetSiblingIndex(insertIndex + 1);
        else
            clickedSegment.transform.SetSiblingIndex(insertIndex - 1);

        Debug.Log($"Inserted image space {(insertRight ? "after" : "before")} letter at index {globalIndex}");
    }
}