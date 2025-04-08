using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomCardController : MonoBehaviour
{
    [Header("Overlay")]
    public GameObject infoOverlay;
    public TextMeshProUGUI infoContent;

    [Header("Informações da Divisão")]
    [TextArea(4, 6)]
    public string roomInfo = "• Área: 10m²\n• Banheira de Hidromassagem\n• Aquecimento de Pavimento\n• Revestimento em Madeira";

    // Referência ao gestor de cards
    private CardsManager cardsManager;

    // Variável para verificar o estado do overlay
    private bool isOverlayActive = false;

    void Start()
    {
        // Procura o CardsManager na cena
        cardsManager = Object.FindFirstObjectByType<CardsManager>();

        //O overlay começa desativado
        if (infoOverlay != null)
            infoOverlay.SetActive(false);
    }

    // Método chamado quando o card é clicado
    public void ToggleInfoOverlay()
    {
        // Inverte o estado do overlay
        isOverlayActive = !isOverlayActive;

        if (isOverlayActive)
        {
            /* Atualiza o conteúdo e ativa o overlay
            if (infoContent != null)
                infoContent.text = roomInfo;*/

            if (infoOverlay != null)
                infoOverlay.SetActive(true);

            // Informa o CardsManager que este card está ativo
            if (cardsManager != null)
                cardsManager.SetActiveCard(this);
        }
        else
        {
            // Desativa o overlay
            CloseOverlay();
        }
    }

    // Método para fechar o overlay 
    public void CloseOverlay()
    {
        isOverlayActive = false;
        if (infoOverlay != null)
            infoOverlay.SetActive(false);
    }

    // Método para abrir o painel de detalhes
    public void OpenDetailPanel()
    {
        Debug.Log("Abrir detalhes completos desta divisão");

        // Desativa o overlay após clicar em "Ver Mais"
        CloseOverlay();
    }
}
