using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopTheoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button buyConceptButton;

    public void Setup(ConceptInfo concept, StoreTheories shopTheories)
    {
        descriptionText.text = concept.description;
        costText.text = "$ " + concept.cost.ToString();
        buyConceptButton.onClick.RemoveAllListeners();
        buyConceptButton.onClick.AddListener(()=> BuyConcept(concept, shopTheories));
    }

    private void BuyConcept(ConceptInfo concept, StoreTheories storeTheories)
    {
        storeTheories.TryBuyItem(concept);
    }
}
