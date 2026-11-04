using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableConcept : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentToReturnTo = null;
    public Transform parent = null;
    public string conceptId;
    public string fullTheory;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private GameObject placeholder = null;
    [HideInInspector] public int slotIndex;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private bool isLocked = false;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        parent = transform.parent;
        slotIndex = transform.GetSiblingIndex();
    }

    public void SetUp(ConceptInfo concept)
    {
        conceptId = concept.idTheory;
        descriptionText.text = concept.description;
        fullTheory = concept.theory;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(isLocked) return;
        parentToReturnTo = transform.parent;

        // Crear un placeholder vacío para mantener el espacio dentro del GridLayoutGroup
        placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(parentToReturnTo);
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = rectTransform.rect.width;
        le.preferredHeight = rectTransform.rect.height;
        placeholder.transform.SetSiblingIndex(transform.GetSiblingIndex());

        // Mover el concepto al Canvas raíz durante el arrastre
        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(isLocked) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(isLocked) return;
        canvasGroup.blocksRaycasts = true;

        // Si se regresa al menú original, toma la posición del placeholder
        if (parentToReturnTo == placeholder.transform.parent)
        {
            transform.SetParent(parentToReturnTo, false);
            transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        }
        else
        {
            // Cayó en un TheorySlot
            transform.SetParent(parentToReturnTo, false);
            rectTransform.anchoredPosition = Vector2.zero;
            isLocked = true; // Bloquear el concepto para que no pueda ser arrastrado nuevamente
        }

        Destroy(placeholder);
    }
}