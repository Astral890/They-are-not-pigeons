using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroComic : MonoBehaviour
{
    [SerializeField] private GameObject menuContent;
    [SerializeField] private GameObject popup;
    [SerializeField] private Image imagenVineta;
    [SerializeField] private TMP_Text contador;
    [SerializeField] private TMP_Text textoBoton;
    [SerializeField] private Sprite[] vinetas;
    [SerializeField] private string escenaJuego = "Plaza";

    private int indice;

    public void Mostrar()
    {
        indice = 0;
        ActualizarVineta();
        menuContent.SetActive(false);
        popup.SetActive(true);
    }

    public void Siguiente()
    {
        if (indice == vinetas.Length - 1)
        {
            popup.SetActive(false);
            return;
        }

        indice++;
        ActualizarVineta();
    }

    private void ActualizarVineta()
    {
        imagenVineta.sprite = vinetas[indice];
        contador.text = $"{indice + 1} / {vinetas.Length}";
        textoBoton.text = indice == vinetas.Length - 1 ? "Jugar" : "Siguiente";
    }
}