using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InsertLetterExercise : ExerciseBase
{
    [SerializeField]
    private Transform optionsParent;

    [SerializeField]
    private Transform wordParent; // container for letter buttons

    [SerializeField]
    private GameObject letterSlotPrefab;

    [SerializeField]
    private GameObject DragLetterPrefab;

    [SerializeField]
    private Image emptyLetterImagePrefab;

    string targetWord;
    int missingIndex;
    List<string> options;

    [SerializeField]
    Color MissingColorLetter;
    public override void Show(Exercise data)
    {
        if (data == null)
        {
            Debug.LogError("Exercise data is null!");
            return;
        }

        // keep correct Word
        targetWord = data.word;

        // Identify the index of the missing letter by comparing display with word
        missingIndex = -1;
        for (int i = 0; i < targetWord.Length && i < data.display.Length; i++)
        {
            if (targetWord[i] != data.display[i])
            {
                missingIndex = i;
                break;
            }
        }

        if (missingIndex == -1)
        {
            Debug.LogWarning("No missing letter found in display word.");
            missingIndex = targetWord.Length - 1; // fallback
        }

        // load options (distractors + target)
        options = new List<string>();

        // Add distractors
        if (!string.IsNullOrEmpty(data.distractors))
        {
            var distractors = data.distractors.Split(' ');
            options.AddRange(distractors);
        }

       
        options.Add(data.target);

        //Mix the options
        for (int i = 0; i < options.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(0, options.Count);
            var temp = options[i];
            options[i] = options[rnd];
            options[rnd] = temp;
        }

        // Generate ui
        SetupUI();
    }

    protected override void SetupUI()
    {
        // limpiar palabra previa
        foreach (Transform t in wordParent) Destroy(t.gameObject);

        // generar slots de la palabra
        for (int i = 0; i < targetWord.Length; i++)
        {
            GameObject slot = Instantiate(letterSlotPrefab, wordParent);
            LetterSlot letterSlot = slot.GetComponent<LetterSlot>();

            string text = targetWord[i].ToString();
            if (i == missingIndex)
            {
                slot.name = "MissingSlot";
                var img = slot.GetComponent<Image>();
                img.color = MissingColorLetter;

                Instantiate(emptyLetterImagePrefab, slot.transform);

            }

            letterSlot?.Init(text, i, i == missingIndex);
        }

       
        foreach (Transform t in optionsParent) Destroy(t.gameObject);
        foreach (var opt in options)
        {
            GameObject slot = Instantiate(letterSlotPrefab, optionsParent);
            LetterSlot letterSlot = slot.GetComponent<LetterSlot>();
            letterSlot.Init("", -1,false);

            GameObject b = Instantiate(DragLetterPrefab, slot.transform);
            b.GetComponentInChildren<TextMeshProUGUI>().text = opt;
            var drag = b.GetComponent<DragLetter>();
            drag.Init(opt);
        }
    }

    public override void CheckSolution(int index)
    {
        GameManager.Instance.UpdateAnswersCounter(index == missingIndex);

        GameManager.Instance.SetCanTakeObject(false);

        // End Exercise
        StartCoroutine(EndAfter(0.7f));
    }
}
