using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TheoryManager : MonoBehaviour, InteractableE
{
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

    public void ValidateTheory()
    {
        //Debug.Log($"childCounts -> Slot1: {slot1.transform.childCount}, Slot2: {slot2.transform.childCount}, Slot3: {slot3.transform.childCount}");

        /*Debug.Log("Validando teoría");
        Debug.Log($"Slot 1: {slot1.currentConcept?.conceptId ?? "Vacío"}");
        Debug.Log($"Slot 2: {slot2.currentConcept?.conceptId ?? "Vacío"}");
        Debug.Log($"Slot 3: {slot3.currentConcept?.conceptId ?? "Vacío"}");*/
        // Verificar que los 3 slots tengan un concepto asignado
        if (slot1.currentConcept == null ||
            slot2.currentConcept == null ||
            slot3.currentConcept == null)
        {
            return;
        }

        Debug.Log("Todos los slots tienen conceptos asignados");

        /*DraggableConcept c1 = slot1.GetComponentInChildren<DraggableConcept>();
        DraggableConcept c2 = slot2.GetComponentInChildren<DraggableConcept>();
        DraggableConcept c3 = slot3.GetComponentInChildren<DraggableConcept>();*/

        Debug.Log("Obteniendo conceptos");
        if (slot1.currentConcept != null && slot2.currentConcept != null && slot3.currentConcept != null)
        {
            CheckCombination(slot1.currentConcept.conceptId, slot2.currentConcept.conceptId, slot3.currentConcept.conceptId);
        }
        else
        {
            Debug.Log("Uno o más slots no tienen un concepto asignado.");
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