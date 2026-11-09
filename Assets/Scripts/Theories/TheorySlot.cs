using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class TheorySlot : MonoBehaviour, IDropHandler
{
    public DraggableConcept currentConcept { get; private set; }
    public int slotCount = 5;
    [SerializeField] private TheoryManager theoryManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableConcept draggedConcept = eventData.pointerDrag.GetComponent<DraggableConcept>();

        if (draggedConcept != null)
        {
            if (transform.childCount > 0)
            {
                return;
            }

            // Asignar el nuevo padre al slot
            draggedConcept.parentToReturnTo = transform;
            currentConcept = draggedConcept;
            Debug.Log($"Concepto colocado");

            // Notificar al gestor de la teoría
            theoryManager.ValidateTheory();
        }
    }

    public void ClearSlot()
    {
        currentConcept = null;
    }
}