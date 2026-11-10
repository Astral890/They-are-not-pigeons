using UnityEngine;

[CreateAssetMenu(fileName = "NewTheory", menuName = "Theories/New theory")]
public class FullTheoryInfo : ScriptableObject
{
    [SerializeField] private string theoryId;
    [TextArea] [SerializeField] private string theoryTitle;

    public string TheoryId => theoryId;
}
