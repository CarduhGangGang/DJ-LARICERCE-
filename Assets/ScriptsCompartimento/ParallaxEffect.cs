using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ParallaxEffect : MonoBehaviour
{
    [Header("Configurações de Parallax")]
    [SerializeField] private float parallaxStrength = 0.2f;
    [SerializeField] private float rotationStrength = 5f;
    
    private ScrollRect scrollRect;
    private RectTransform contentRect;
    private float lastScrollPos = 0f;
    private float scrollDelta = 0f;
    
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentRect = scrollRect.content;
    }
    
    private void Update()
    {
        // Calcular a mudança de posição de scroll
        float currentScrollPos = scrollRect.horizontalNormalizedPosition;
        scrollDelta = currentScrollPos - lastScrollPos;
        lastScrollPos = currentScrollPos;
        
        // Aplicar efeito parallax às imagens
        ApplyParallaxToChildren();
    }
    
    private void ApplyParallaxToChildren()
    {
        if (contentRect == null) return;
        
        // Para cada filho (imagem) no content
        foreach (RectTransform child in contentRect)
        {
            // Calcular a posição relativa ao centro do viewport
            float childCenterX = child.position.x;
            float viewportCenterX = transform.position.x;
            float distanceFromCenter = (childCenterX - viewportCenterX) / (Screen.width * 0.5f);
            
            // Aplicar uma pequena rotação baseada na distância do centro
            Quaternion targetRotation = Quaternion.Euler(0, 0, -distanceFromCenter * rotationStrength);
            child.rotation = Quaternion.Slerp(child.rotation, targetRotation, Time.deltaTime * 5f);
            
            // Aplicar um movimento parallax baseado na velocidade de scroll
            Vector3 parallaxOffset = new Vector3(scrollDelta * parallaxStrength * distanceFromCenter, 0, 0);
            child.position += parallaxOffset;
        }
    }
}