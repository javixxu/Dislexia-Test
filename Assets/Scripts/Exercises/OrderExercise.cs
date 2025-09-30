using System.Diagnostics.Tracing;
using System.Linq;
using TMPro;
using UnityEngine;

public class OrderExercise : ExerciseBase
{
    [Header("UI References")]
    [SerializeField] private Transform wordParent;       // Slots donde va la palabra
    [SerializeField] private GameObject dragLetterPrefab;

    private string targetWord;
    private string[] correctChunks;

    int restMovements = -1;

    public override void Show(Exercise data)
    {
        if (data == null)
        {
            Debug.LogError("Exercise data is null!");
            return;
        }
        restMovements = data.min_movements;
        // Guardar palabra objetivo y sílabas
        targetWord = data.target;
        correctChunks = data.chunks.Split(' ');

        SetupUI();
    }

    protected override void SetupUI()
    {
        // Limpiar UI previa
        foreach (Transform t in wordParent) Destroy(t.gameObject);

        // Crear drags desordenados
        foreach (var chunk in correctChunks)
        {

            GameObject dragGO = Instantiate(dragLetterPrefab, wordParent);
            DragLetter drag = dragGO.GetComponent<DragLetter>();
            drag.Init(chunk, true);
        }
    }

    public override void CheckSolution(int index = -1)
    {
        restMovements--;
        // Construir la palabra final desde los slots
        string result = "";
        foreach (Transform slot in wordParent)
        {
            DragLetter drag = slot.GetComponentInChildren<DragLetter>();
            if (drag != null)
                result += drag.GetCurrentLetter();
            else
                result += ""; // slot vacío, no sumar nada
        }

        // Comparar con la palabra objetivo
        bool isCorrect = result.Replace(" ", "") == targetWord.Replace(" ", "");

        // Actualizar contador de aciertos/errores
        if (isCorrect)
        {
            Debug.Log("¡CORRECTO! " + result);
            GameManager.Instance.UpdateAnswersCounter(true);

            // Finalizar ejercicio
            StartCoroutine(EndAfter(0.7f));
        }
        else if( restMovements <= 0)
        {
            Debug.Log("INCORRECTO: " + result);
            GameManager.Instance.UpdateAnswersCounter(false);

            // Finalizar ejercicio
            StartCoroutine(EndAfter(0.7f));
        }

       
    }
}
