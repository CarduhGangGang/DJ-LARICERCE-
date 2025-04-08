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

    [Header("Informa��es da Divis�o")]
    [TextArea(4, 6)]
    public string roomInfo = "� �rea: 10m�\n� Banheira de Hidromassagem\n� Aquecimento de Pavimento\n� Revestimento em Madeira";

    // Refer�ncia ao gestor de cards
    private CardsManager cardsManager;

    // Vari�vel para verificar o estado do overlay
    private bool isOverlayActive = false;

    void Start()
    {
        // Procura o CardsManager na cena
        cardsManager = Object.FindFirstObjectByType<CardsManager>();

        //O overlay come�a desativado
        if (infoOverlay != null)
            infoOverlay.SetActive(false);
    }

    // M�todo chamado quando o card � clicado
    public void ToggleInfoOverlay()
    {
        // Inverte o estado do overlay
        isOverlayActive = !isOverlayActive;

        if (isOverlayActive)
        {
            /* Atualiza o conte�do e ativa o overlay
            if (infoContent != null)
                infoContent.text = roomInfo;*/

            if (infoOverlay != null)
                infoOverlay.SetActive(true);

            // Informa o CardsManager que este card est� ativo
            if (cardsManager != null)
                cardsManager.SetActiveCard(this);
        }
        else
        {
            // Desativa o overlay
            CloseOverlay();
        }
    }

    // M�todo para fechar o overlay 
    public void CloseOverlay()
    {
        isOverlayActive = false;
        if (infoOverlay != null)
            infoOverlay.SetActive(false);
    }

    // M�todo para abrir o painel de detalhes
    public void OpenDetailPanel()
    {
        Debug.Log("Abrir detalhes completos desta divis�o");

        // Desativa o overlay ap�s clicar em "Ver Mais"
        CloseOverlay();
    }
}
