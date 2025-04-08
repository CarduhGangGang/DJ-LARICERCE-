using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Adicionar este import

public class CardsManager : MonoBehaviour
{
    // Refer�ncia para o card atualmente ativo
    private RoomCardController activeCard;

    // M�todo para ativar um card e desativar o anterior
    public void SetActiveCard(RoomCardController newActiveCard)
    {
        // Se temos um card ativo e � diferente do novo, desativa-o
        if (activeCard != null && activeCard != newActiveCard)
        {
            activeCard.CloseOverlay();
        }
        // Define o novo card como ativo
        activeCard = newActiveCard;
    }

    // M�todo para limpar quando clicamos fora dos cards
    public void ClearActiveCard()
    {
        if (activeCard != null)
        {
            activeCard.CloseOverlay();
            activeCard = null;
        }
    }

    // Adicionar este m�todo para verificar cliques
    void Update()
    {
        // S� verifica se houver um card ativo e se o utilizador clicar
        if (activeCard != null && Input.GetMouseButtonDown(0))
        {
            // Lan�a um raio a partir da posi��o do rato
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            bool clickedOnActiveCard = false;

            // Verifica se o clique foi no card ativo ou em algum dos seus filhos
            foreach (RaycastResult result in results)
            {
                if (result.gameObject == activeCard.gameObject ||
                    result.gameObject.transform.IsChildOf(activeCard.transform))
                {
                    clickedOnActiveCard = true;
                    break;
                }
            }

            // Se o clique n�o foi no card ativo, fecha o overlay
            if (!clickedOnActiveCard)
            {
                ClearActiveCard();
            }
        }
    }
}