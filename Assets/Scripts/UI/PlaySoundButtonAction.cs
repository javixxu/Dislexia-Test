using UnityEngine;
using UnityEngine.UI;

public class PlaySoundButtonAction : MonoBehaviour
{
    [SerializeField] private Button resetButton;

    private void Start()
    {
        if (resetButton == null)
        {
            resetButton = GetComponent<Button>();
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners(); // clean listeners
            resetButton.onClick.AddListener(() => GameManager.Instance.PlayPackageInstruction());
        }
        else
        {
            Debug.LogError("⚠️ ResetButton no encontrado en la escena.");
        }

    }
}
