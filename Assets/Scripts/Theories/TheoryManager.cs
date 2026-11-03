using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TheoryManager : MonoBehaviour, InteractableE
{
    public static TheoryManager Instance;

    [SerializeField] private TheorySlot slot1;
    [SerializeField] private TheorySlot slot2;
    [SerializeField] private TheorySlot slot3;
    [SerializeField] private List<ConceptInfo> conceptsList;
    [SerializeField] private Transform conceptsContainer;
    [SerializeField] private GameObject conceptSlotPrefab;
    private bool isPlayerInside = false;
    private marcoMovement playerRef;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerRef = other.GetComponent<marcoMovement>();
            playerRef.SetCurrentInteractable(this);
            playerRef.UpdatePromptUI();
        }
        populateConcepts(conceptsList);
        Debug.Log("Despues del populate");
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ValidateTheory()
    {
        // Verificar que los 3 slots tengan un concepto asignado
        if (slot1.transform.childCount == 0 ||
            slot2.transform.childCount == 0 ||
            slot3.transform.childCount == 0)
        {
            return;
        }

        DraggableConcept c1 = slot1.GetComponentInChildren<DraggableConcept>();
        DraggableConcept c2 = slot2.GetComponentInChildren<DraggableConcept>();
        DraggableConcept c3 = slot3.GetComponentInChildren<DraggableConcept>();

        if (c1 != null && c2 != null && c3 != null)
        {
            CheckCombination(c1.conceptId, c2.conceptId, c3.conceptId);
        }
    }

    private void CheckCombination(string id1, string id2, string id3)
    {
        // Lógica de comprobación de tu juego
        Debug.Log($"Evaluando teoría: {id1} + {id2} + {id3}");

        if ((id1 == id2) && (id2==id3))
        {
            Debug.Log("¡Teoría correcta descubierta!");
        }
    }

    private void populateConcepts(List<ConceptInfo> concepts)
    {
        foreach(Transform child in conceptsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach(var item in conceptsList)
        {
            GameObject slot = Instantiate(conceptSlotPrefab, conceptsContainer);
            slot.GetComponent<DraggableConcept>().SetUp(item);
        }
    }

    public void Interact()
    {
        Debug.Log("Interact");
    }

    public string GetPromptMsg()
    {
        Debug.Log("Prompt");
        return "1";
    }
}