using UnityEngine;
using UnityEngine.UI;

public class VideoBackgroundManager : MonoBehaviour
{
    [Header("Las RawImages de los 4 Fondos")]
    // Arrastra aquí las RawImages de Fondo_L1, Fondo_L2, Fondo_L3 y Fondo_L4
    public RawImage[] imagenesFondo = new RawImage[4];

    private int ultimoNivel = -1;

    private void Start()
    {
        // Al arrancar mostramos solo la imagen del Nivel 1
        MostrarSoloFondo(0);
    }

    private void Update()
    {
        // Leemos la capa actual del GameManager
        int nivelActual = GameManager.Instance.depthLevel;
        int indice = Mathf.Clamp(nivelActual - 1, 0, imagenesFondo.Length - 1);

        if (indice != ultimoNivel)
        {
            MostrarSoloFondo(indice);
            ultimoNivel = indice;
        }
    }

    private void MostrarSoloFondo(int indiceActivo)
    {
        for (int i = 0; i < imagenesFondo.Length; i++)
        {
            if (imagenesFondo[i] != null)
            {
                // NO apaga el GameObject (el video sigue corriendo en RAM),
                // solo oculta la imagen visible en pantalla.
                imagenesFondo[i].enabled = (i == indiceActivo);
            }
        }
    }
}