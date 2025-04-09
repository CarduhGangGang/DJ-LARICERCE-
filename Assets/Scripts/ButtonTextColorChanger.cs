using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTextColorChanger : MonoBehaviour, IPointerDownHandler
{
    // Texto do botão
    private TextMeshProUGUI buttonText;

    // Cores para diferentes estados
    public Color normalColor = new Color(0.5f, 0.27f, 0.12f); // Castanho
    public Color activeColor = Color.white; // Branco

    void Start()
    {
        //componente de texto do botão
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Define a cor inicial
        if (buttonText != null)
        {
            buttonText.color = normalColor;
        }
    }

    // Quando o botão é clicado
    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = activeColor;
        }
    }
}