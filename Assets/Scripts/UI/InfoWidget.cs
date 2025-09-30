using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct TypeDescription
{
    public int TypeId;
    public string Desc;
}

public class InfoWidget : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI infoTest;

    [SerializeField] private List<TypeDescription> TypeDesc;

    private void Start()
    {
           GameManager.Instance.exercisesSystem.OnPackageChanged += SetText;

           SetText(GameManager.Instance.exercisesSystem.GetCurrentPackage());
    }
    public void SetText(Package newPackage)
    {
        var desc = TypeDesc.FirstOrDefault(t => t.TypeId == newPackage.typeId).Desc;
        infoTest.text = string.IsNullOrEmpty(desc) ? $"Unknown TypeId: {newPackage.typeId}" : desc;
    }
}
