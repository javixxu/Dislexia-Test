using UnityEngine;
using UnityEngine.UI;

public class ButtonSceneLoad : MonoBehaviour
{
    [SerializeField] private Button resetButton;

    [SerializeField] string levelToLoad = "MainMenu";

    private void Start()
    {
        if (resetButton == null)
        {
            resetButton = GetComponent<Button>();
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners(); // clean listeners
            resetButton.onClick.AddListener(() => GameManager.Instance.LoadScene(levelToLoad));
        }
        else
        {
            Debug.LogError("⚠️ ResetButton no encontrado en la escena.");
        }
    
    }
}
