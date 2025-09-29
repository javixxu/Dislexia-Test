using System.Collections;
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

    string targetWord;
    int missingIndex;
    List<string> options;

    private void Start()
    {
        targetWord = "EXAMPLE";

        missingIndex = 2;

        options = new List<string> { "A", "X", "I", "O", "U" };


        //TODO:: GENERATE OPTIONS
        SetupUI();
    }

    public override void Show(object dataObj)
    {
        //TODO : PROCCESS DTAOBJ TO GET THE EXERCISE 
        /*
        var data = dataObj as PackageRunner.ExerciseData;
        targetWord = data.word;
        missingIndex = data.missingIndex;
        options = data.options;
        SetupUI();*/
    }

    void SetupUI()
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
                text = " ";
            }

                letterSlot?.Init(text, i == missingIndex);
        }

       
        foreach (Transform t in optionsParent) Destroy(t.gameObject);
        foreach (var opt in options)
        {
            GameObject slot = Instantiate(letterSlotPrefab, optionsParent);
            LetterSlot letterSlot = slot.GetComponent<LetterSlot>();
            letterSlot.Init(opt, false);

            GameObject b = Instantiate(DragLetterPrefab, slot.transform);
            b.GetComponentInChildren<TextMeshProUGUI>().text = opt;
            var drag = b.GetComponent<DragLetter>();
            drag.Init(opt);
        }
    }

    void OnLetterDropped(string letter, Vector2 dropPosition)
    {
        // simple hit detection: if player drops near the index area -> accept.
        // For prototype: check if letter equals expected letter
        if (letter == targetWord[missingIndex].ToString())
        {
            // success
            // show filled word
            //char[] arr = targetWord.ToCharArray();
            //arr[missingIndex] = letter[0];
            //wordDisplay.text = new string(arr);
            // small delay then finish
            StartCoroutine(EndAfter(0.4f));
        }
        else
        {
            // mark error (shake), but allow next attempts or finish as wrong
            StartCoroutine(EndAfter(0.5f));
        }
    }
    System.Collections.IEnumerator EndAfter(float s)
    {
        yield return new WaitForSeconds(s);
        Finish();
    }

    public override void ForceStop()
    {
        // cleanup and finish
        Finish();
    }

}
