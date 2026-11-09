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
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    private bool isPlayerInside = false;
    private bool isTheoryOpen = false;
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerRef.SetCurrentInteractable(null);
            playerRef.UpdatePromptUI();
            if (isTheoryOpen) playerRef.SetMenuState(false);
            playerRef.canOpenInventory = true;
            playerRef = null;
            isTheoryOpen = false;
            uiManager.SetTheoryVisibility(isTheoryOpen);
        }
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

        //Debug.Log("Todos los slots tienen conceptos asignados");

        /*DraggableConcept c1 = slot1.GetComponentInChildren<DraggableConcept>();
        DraggableConcept c2 = slot2.GetComponentInChildren<DraggableConcept>();
        DraggableConcept c3 = slot3.GetComponentInChildren<DraggableConcept>();*/

        //Debug.Log("Obteniendo conceptos");
        if (slot1.currentConcept != null && slot2.currentConcept != null && slot3.currentConcept != null)
        {
            CheckCombination(slot1.currentConcept.conceptId, slot2.currentConcept.conceptId, slot3.currentConcept.conceptId, slot1.currentConcept.fullTheory);
        }
        else
        {
            //Debug.Log("Uno o más slots no tienen un concepto asignado.");
        }
    }

    private void CheckCombination(string id1, string id2, string id3, string correctTheory)
    {
        // Lógica de comprobación de tu juego
        Debug.Log($"Evaluando teoría: {id1} + {id2} + {id3}");

        if ((id1 == id2) && (id2==id3))
        {
            Debug.Log("¡Teoría correcta descubierta!");
            gameManager.player.SetCurrentTheory(correctTheory);
        }
        else
        {
            Debug.Log("Teoría incorrecta");
            gameManager.player.SetCurrentTheory("");
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
        Debug.Log("Interacting with the theory manager");
        if(isPlayerInside && !isTheoryOpen)
        {
            isTheoryOpen = true;
            uiManager.SetTheoryVisibility(isTheoryOpen);
            playerRef.SetMenuState(isTheoryOpen);
            playerRef.canOpenInventory = false;
        }
        else
        {
            isTheoryOpen = false;
            uiManager.SetTheoryVisibility(isTheoryOpen);
            playerRef.SetMenuState(isTheoryOpen);
            playerRef.canOpenInventory = true;
        }
    }

    public string GetPromptMsg()
    {
        return "Presiona E para abrir el panel de teorías";
    }
}