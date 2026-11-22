using UnityEngine;

public class SwapTrees : MonoBehaviour
{
    [SerializeField] private Transform arbolConTelefono;
    [SerializeField] private Transform arbolSinTelefono;

    public void SwapPlaces()
    {
        Vector3 posicionOriginal = arbolConTelefono.position;
        Quaternion rotacionOriginal = arbolConTelefono.rotation;

        arbolConTelefono.SetPositionAndRotation(
            arbolSinTelefono.position, arbolSinTelefono.rotation);

        arbolSinTelefono.SetPositionAndRotation(
            posicionOriginal, rotacionOriginal);
    }
}