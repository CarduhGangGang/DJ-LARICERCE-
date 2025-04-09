using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneNavigator : MonoBehaviour
{
    // carrega cena especifica quando este bot�o for clicado
    public string targetSceneName;

    // M�todo que ser� chamado pelo bot�o 
    public void NavigateToScene()
    {
        // Verifica se o nome da cena est� definido
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            // Carrega a cena especificada
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("Nome da cena n�o definido para o bot�o Ver Mais");
        }
    }
}