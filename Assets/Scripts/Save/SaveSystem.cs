using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string RutaArchivo => Path.Combine(Application.persistentDataPath, "saveData.json");

    public static void SaveGame(DatosGuardado datos)
    {
        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(RutaArchivo, json);
        Debug.Log($"Partida guardada en: {RutaArchivo}");
    }

    public static DatosGuardado LoadGame()
    {
        if (!File.Exists(RutaArchivo))
        {
            Debug.Log("No hay archivo de guardado");
            return new DatosGuardado();
        }
        string json = File.ReadAllText(RutaArchivo);
        return JsonUtility.FromJson<DatosGuardado>(json);
    }

    public static bool ExistsSave()
    {
        return File.Exists(RutaArchivo);
    }
}