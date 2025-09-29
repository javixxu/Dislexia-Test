using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    GameManager gameManager;
    public Canvas mainCanvas;

    private void Awake()
    {
        Debug.Log("Initializasing UI MANAGER");
        Instance = this;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    public Canvas GetMainCanvas() { return mainCanvas; }
}
