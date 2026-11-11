using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConcept", menuName = "Concepts/newConcept")]
public class ConceptInfo : ScriptableObject
{
    public string idTheory;
    [TextArea] public string description;
    public string theory;
    public string videoUrl;
}
