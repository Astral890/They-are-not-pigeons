using UnityEngine;
using System;
using System.Collections.Generic;

public class TheoryValidator : MonoBehaviour
{
    // Usando strings con comparación insensible a mayúsculas
    private readonly HashSet<string> _correctTheoryIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "La paloma es un dron espía",
    };

    public bool IsTheoryValid(string playerTheory)
    {
        Debug.Log("Validating theory: " + playerTheory);
        if (string.IsNullOrWhiteSpace(playerTheory)) return false;
        return _correctTheoryIds.Contains(playerTheory.Trim());
    }
}
