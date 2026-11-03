using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class TheorySlot : MonoBehaviour, IDropHandler
{
    public DraggableConcept currentConcept { get; private set; }
    public int slotCount = 5;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        try
        {
            currentConcept.transform.SetParent(currentConcept.parent, false);
            currentConcept.transform.SetSiblingIndex(currentConcept.slotIndex);
            currentConcept = null;
        }catch(Exception ex)
        {
            Debug.Log("Error: "+ex.ToString());
        }

        DraggableConcept draggedConcept = eventData.pointerDrag.GetComponent<DraggableConcept>();

        if (draggedConcept != null)
        {
            

            // Asignar el nuevo padre al slot
            draggedConcept.parentToReturnTo = transform;
            currentConcept = draggedConcept;

            // Notificar al gestor de la teoría
            TheoryManager.Instance?.ValidateTheory();
        }
    }

    public void ClearSlot()
    {
        currentConcept = null;
    }
}