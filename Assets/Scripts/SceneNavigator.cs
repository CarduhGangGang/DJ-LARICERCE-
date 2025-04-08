using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class SceneNavigator : MonoBehaviour
{
    // Nome da cena a carregar quando este botão for clicado
    public string targetSceneName;

    // Método que será chamado pelo botão 
    public void NavigateToScene()
    {
        // Verifica se o nome da cena está definido
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            // Carrega a cena especificada
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("Nome da cena não definido para o botão Ver Mais");
        }
    }
}