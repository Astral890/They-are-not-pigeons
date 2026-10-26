
using UnityEngine;
using UnityEditor;

public static class SnapToGround
{
    [MenuItem("Tools/Plaza/Colocar sobre suelo")]
    public static void ColocarSobreSuelo()
    {
        GameObject ground = GameObject.Find("ground");

        if (ground == null)
        {
            Debug.LogWarning("No se encontró el objeto ground.");
            return;
        }

        Collider suelo = ground.GetComponent<Collider>();

        if (suelo == null)
        {
            Debug.LogWarning("El ground necesita un Collider.");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj == ground) continue;

            Renderer[] renderers =
                obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0) continue;

            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 origen = new Vector3(
                bounds.center.x,
                suelo.bounds.max.y + 1000f,
                bounds.center.z
            );

            if (suelo.Raycast(
                new Ray(origen, Vector3.down),
                out RaycastHit hit,
                2000f))
            {
                Undo.RecordObject(obj.transform, "Colocar sobre suelo");

                float diferencia = hit.point.y - bounds.min.y;

                obj.transform.position +=
                    Vector3.up * diferencia;
            }
        }
    }
}
