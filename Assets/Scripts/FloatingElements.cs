using UnityEngine;
using System.Collections.Generic;

public class FloatingElements : MonoBehaviour
{
    // Lista dos elementos flutuantes
    private List<RectTransform> floatingElements = new List<RectTransform>();

    // Configurações de movimento
    [Range(0.01f, 0.5f)]
    public float speedFactor = 0.05f; // Muito lento

    [Range(10f, 100f)]
    public float movementRange = 30f; // Alcance do movimento

    // Posições originais
    private List<Vector2> originalPositions = new List<Vector2>();

    // Valores de offset para cada elemento
    private List<Vector2> offsetValues = new List<Vector2>();

    void Start()
    {
        // Encontra todos os elementos filhos
        foreach (Transform child in transform)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            if (rect != null)
            {
                floatingElements.Add(rect);
                originalPositions.Add(rect.anchoredPosition);

                // Cria offset aleatório para cada elemento
                offsetValues.Add(new Vector2(
                    Random.Range(0f, 100f),
                    Random.Range(0f, 100f)
                ));
            }
        }
    }

    void Update()
    {
        // Anima cada elemento
        for (int i = 0; i < floatingElements.Count; i++)
        {
            if (floatingElements[i] != null)
            {
                // Cria movimento fluido com funções de seno/cosseno
                float xOffset = Mathf.Sin(Time.time * speedFactor + offsetValues[i].x) * movementRange;
                float yOffset = Mathf.Cos(Time.time * speedFactor + offsetValues[i].y) * movementRange;

                // Aplica o movimento
                floatingElements[i].anchoredPosition = originalPositions[i] + new Vector2(xOffset, yOffset);
            }
        }
    }
}