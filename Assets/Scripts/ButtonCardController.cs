using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonCardController : MonoBehaviour, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // O componente de texto do bot�o
    private TextMeshProUGUI textComponent;

    // Cores para diferentes estados
    public Color normalColor = new Color(0.5f, 0.27f, 0.12f); // Castanho
    public Color highlightedColor = Color.white;
    public Color pressedColor = new Color(0.5f, 0.27f, 0.12f); // Castanho

    //Controla se o rato est� sobre o bot�o
    private bool isPointerOver = false;

    void Awake()
    {
        //componente de texto
        textComponent = GetComponent<TextMeshProUGUI>();

        // Se n�o encontrou no pr�prio GameObject, procura nos filhos
        if (textComponent == null)
            textComponent = GetComponentInChildren<TextMeshProUGUI>();

        // Define a cor inicial
        if (textComponent != null)
            textComponent.color = normalColor;
    }

    

    // Quando o cursor sai do bot�o
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        if (textComponent != null)
            textComponent.color = normalColor;
    }

    // Quando o bot�o � pressionado
    public void OnPointerDown(PointerEventData eventData)
    {
        if (textComponent != null)
            textComponent.color = pressedColor;
    }

    // Quando o bot�o � solto
    public void OnPointerUp(PointerEventData eventData)
    {
        if (textComponent != null)
        {
            if (isPointerOver)
                textComponent.color = highlightedColor;
            else
                textComponent.color = normalColor;
        }
    }
}