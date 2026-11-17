using UnityEngine;
using System;
using System.Collections.Generic;

public class TheoryValidator : MonoBehaviour
{
    // Usando strings con comparación insensible a mayúsculas
    private readonly HashSet<string> _correctTheoryIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "La fuente cambia según tus pensamientos",
        "La fuente mantiene el equilibrio de la plaza",
        "La paloma es un dron espía",
        "Los puentes cambian de lugar durante la noche",
        "El WIFI se alimenta de las conversaciones",
        "Los gatos, mugrosos gatos",
        "Los puestos de comida sirven pa otra cosa",
        "Las alcantarillas respiran"
    };

    public bool IsTheoryValid(string playerTheory)
    {
        Debug.Log("Validating theory: " + playerTheory);
        if (string.IsNullOrWhiteSpace(playerTheory)) return false;
        return _correctTheoryIds.Contains(playerTheory.Trim());
    }
}
