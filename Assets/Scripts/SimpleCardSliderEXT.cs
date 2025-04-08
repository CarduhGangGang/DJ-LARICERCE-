using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InfiniteCardSlider : MonoBehaviour
{
    // Lista de cartões prefab
    public List<GameObject> cardPrefabs = new List<GameObject>();

    // Transform pai onde os cartões serão exibidos
    public Transform displayPoint;

    // Área de visualização - referência ao RectTransform do container
    public RectTransform viewportRect;

    // Botões de navegação
    public Button prevButton;
    public Button nextButton;

    // Índice do cartão central
    private int currentCardIndex = 0;

    // Referências aos cartões visíveis
    private GameObject leftCard, centerCard, rightCard;

    // Configurações do layout
    [Header("Layout")]
    public float cardWidth = 400f;          // Largura do card central
    public float sideCardWidth = 300f;      // Largura dos cards laterais
    public float sideCardVisibleAmount = 20f;  // Quanto do card lateral fica visível (REDUZIDO)
    public float cardDepth = 10f;           // Diferença de profundidade (z) entre os cards
    [Range(0f, 1f)]
    public float sideCardOpacity = 0.6f;    // Opacidade dos cards laterais
    [Range(0f, 30f)]
    public float sideCardTilt = 15f;        // Inclinação dos cards laterais em graus
    
    // Referência para o tamanho real dos prefabs
    [Tooltip("Ative para considerar o tamanho real do prefab e não apenas o RectTransform")]
    public bool useRealPrefabSize = true;
    
    // Configurações da animação
    [Header("Animation")]
    public float transitionDuration = 0.3f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool useRotationEffect = true;
    public bool usePerspectiveEffect = true;

    // Largura da viewport
    private float viewportWidth;
    
    // Dicionário para armazenar tamanhos reais dos prefabs
    private Dictionary<int, Vector2> prefabRealSizes = new Dictionary<int, Vector2>();

    void Start()
    {
        // Configurar listeners dos botões
        prevButton.onClick.AddListener(ShowPreviousCard);
        nextButton.onClick.AddListener(ShowNextCard);

        // Obter a largura da área de visualização
        if (viewportRect == null)
            viewportRect = displayPoint.parent as RectTransform;
        
        viewportWidth = viewportRect.rect.width;
        
        // Calcular e armazenar os tamanhos reais dos prefabs
        if (useRealPrefabSize) {
            PreCalculatePrefabSizes();
        }

        // Inicializar cards
        SetupInitialCards();
    }
    
    void PreCalculatePrefabSizes()
    {
        // Temporariamente instanciar cada prefab para medir seu tamanho real
        for (int i = 0; i < cardPrefabs.Count; i++)
        {
            GameObject tempCard = Instantiate(cardPrefabs[i], displayPoint);
            
            // Obter o tamanho do prefab considerando todos os seus elementos
            Rect bounds = CalculateTotalBounds(tempCard);
            prefabRealSizes[i] = new Vector2(bounds.width, bounds.height);
            
            // Destruir o objeto temporário
            Destroy(tempCard);
        }
    }
    
    Rect CalculateTotalBounds(GameObject card)
    {
        Rect totalBounds = new Rect();
        bool firstElement = true;
        
        // Obter todos os RectTransforms do card
        RectTransform[] rectTransforms = card.GetComponentsInChildren<RectTransform>();
        
        foreach (RectTransform rt in rectTransforms)
        {
            // Converter posição local para posição em relação ao card
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            
            // Converter posições mundiais para locais em relação ao card
            for (int i = 0; i < 4; i++)
            {
                corners[i] = card.transform.InverseTransformPoint(corners[i]);
            }
            
            // Calcular os limites deste elemento
            float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
            float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
            float minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
            float maxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
            
            Rect elementBounds = new Rect(minX, minY, maxX - minX, maxY - minY);
            
            // Inicializar ou expandir os limites totais
            if (firstElement)
            {
                totalBounds = elementBounds;
                firstElement = false;
            }
            else
            {
                // Expandir os limites para incluir este elemento
                float newMinX = Mathf.Min(totalBounds.xMin, elementBounds.xMin);
                float newMinY = Mathf.Min(totalBounds.yMin, elementBounds.yMin);
                float newMaxX = Mathf.Max(totalBounds.xMax, elementBounds.xMax);
                float newMaxY = Mathf.Max(totalBounds.yMax, elementBounds.yMax);
                
                totalBounds = new Rect(newMinX, newMinY, newMaxX - newMinX, newMaxY - newMinY);
            }
        }
        
        return totalBounds;
    }

    void SetupInitialCards()
    {
        // Calcular posições dos cards - agora usando a quantidade visível reduzida
        float leftEdgeX = -viewportWidth / 2 + sideCardVisibleAmount;  
        float rightEdgeX = viewportWidth / 2 - sideCardVisibleAmount;  

        // Criar card central
        centerCard = Instantiate(cardPrefabs[currentCardIndex], displayPoint);
        RectTransform centerRect = centerCard.GetComponent<RectTransform>();
        
        // Usar o tamanho real ou apenas redimensionar o RectTransform
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(currentCardIndex))
        {
            float scaleRatio = cardWidth / prefabRealSizes[currentCardIndex].x;
            centerCard.transform.localScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        else if (centerRect)
        {
            centerRect.sizeDelta = new Vector2(cardWidth, centerRect.sizeDelta.y);
        }
        
        centerCard.transform.localPosition = new Vector3(0, 0, 0);  // Na frente
        centerCard.transform.SetAsLastSibling();  // Garantir que está na frente (ordem de rendering)

        // Criar card esquerdo
        int leftIndex = (currentCardIndex - 1 + cardPrefabs.Count) % cardPrefabs.Count;
        leftCard = Instantiate(cardPrefabs[leftIndex], displayPoint);
        RectTransform leftRect = leftCard.GetComponent<RectTransform>();
        
        // Usar o tamanho real ou apenas redimensionar o RectTransform
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(leftIndex))
        {
            float scaleRatio = sideCardWidth / prefabRealSizes[leftIndex].x;
            leftCard.transform.localScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        else if (leftRect)
        {
            leftRect.sizeDelta = new Vector2(sideCardWidth, leftRect.sizeDelta.y);
        }
        
        leftCard.transform.localPosition = new Vector3(leftEdgeX, 0, cardDepth);  // Atrás do central
        
        // Aplicar opacidade e rotação ao card esquerdo
        ApplySideCardEffects(leftCard, true);
        
        leftCard.transform.SetAsFirstSibling();  // Garantir que está atrás

        // Criar card direito
        int rightIndex = (currentCardIndex + 1) % cardPrefabs.Count;
        rightCard = Instantiate(cardPrefabs[rightIndex], displayPoint);
        RectTransform rightRect = rightCard.GetComponent<RectTransform>();
        
        // Usar o tamanho real ou apenas redimensionar o RectTransform
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(rightIndex))
        {
            float scaleRatio = sideCardWidth / prefabRealSizes[rightIndex].x;
            rightCard.transform.localScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        else if (rightRect)
        {
            rightRect.sizeDelta = new Vector2(sideCardWidth, rightRect.sizeDelta.y);
        }
        
        rightCard.transform.localPosition = new Vector3(rightEdgeX, 0, cardDepth);  // Atrás do central
        
        // Aplicar opacidade e rotação ao card direito
        ApplySideCardEffects(rightCard, false);
        
        rightCard.transform.SetAsFirstSibling();  // Garantir que está atrás
    }
    
    // Método para aplicar efeitos visuais aos cards laterais
    void ApplySideCardEffects(GameObject card, bool isLeftSide)
    {
        // Aplicar opacidade
        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = card.AddComponent<CanvasGroup>();
        
        canvasGroup.alpha = sideCardOpacity;
        
        // Aplicar rotação (inclinação) se ativado
        if (useRotationEffect)
        {
            float rotationAngle = isLeftSide ? sideCardTilt : -sideCardTilt;
            card.transform.localRotation = Quaternion.Euler(0, rotationAngle, 0);
        }
        
        // Aplicar efeito de perspectiva se ativado
        if (usePerspectiveEffect)
        {
            // Adicionar um pequeno deslocamento vertical para simular perspectiva
            float verticalOffset = -5f;
            Vector3 currentPos = card.transform.localPosition;
            card.transform.localPosition = new Vector3(currentPos.x, verticalOffset, currentPos.z);
        }
    }
    
    // Método para restaurar um card para o estado de card central
    void RestoreCenterCardEffects(GameObject card)
    {
        // Restaurar opacidade
        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
        
        // Restaurar rotação
        if (useRotationEffect)
        {
            card.transform.localRotation = Quaternion.identity;
        }
        
        // Restaurar posição vertical
        if (usePerspectiveEffect)
        {
            Vector3 currentPos = card.transform.localPosition;
            card.transform.localPosition = new Vector3(currentPos.x, 0, currentPos.z);
        }
    }

    public void ShowNextCard()
    {
        // Desativar botões durante animação
        SetButtonsInteractive(false);
        StartCoroutine(AnimateToNext());
    }

    public void ShowPreviousCard()
    {
        // Desativar botões durante animação
        SetButtonsInteractive(false);
        StartCoroutine(AnimateToPrevious());
    }

    void SetButtonsInteractive(bool interactive)
    {
        prevButton.interactable = interactive;
        nextButton.interactable = interactive;
    }

    IEnumerator AnimateToNext()
    {
        float leftEdgeX = -viewportWidth / 2 + sideCardVisibleAmount;
        float rightEdgeX = viewportWidth / 2 - sideCardVisibleAmount;
        
        // O card da direita vai se tornar o central
        GameObject newCenter = rightCard;
        
        // O card central vai se tornar o da esquerda
        GameObject newLeft = centerCard;
        
        // Destruir o card da esquerda atual
        Destroy(leftCard);
        
        // Criar o novo card da direita
        int newRightIndex = (currentCardIndex + 2) % cardPrefabs.Count;
        GameObject newRight = Instantiate(cardPrefabs[newRightIndex], displayPoint);
        RectTransform rightRect = newRight.GetComponent<RectTransform>();
        
        // Usar o tamanho real ou apenas redimensionar o RectTransform
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(newRightIndex))
        {
            float scaleRatio = sideCardWidth / prefabRealSizes[newRightIndex].x;
            newRight.transform.localScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        else if (rightRect)
        {
            rightRect.sizeDelta = new Vector2(sideCardWidth, rightRect.sizeDelta.y);
        }
        
        // Posicionar fora da tela à direita e aplicar efeitos de card lateral
        newRight.transform.localPosition = new Vector3(rightEdgeX + viewportWidth, 0, cardDepth);
        ApplySideCardEffects(newRight, false);
        newRight.transform.SetAsFirstSibling();
        
        // Preparar para animação
        Vector3 newLeftTarget = new Vector3(leftEdgeX, 0, cardDepth);
        Vector3 newCenterTarget = new Vector3(0, 0, 0);
        Vector3 newRightTarget = new Vector3(rightEdgeX, 0, cardDepth);
        
        // Variáveis para animação de escala
        Vector3 centerStartScale = newLeft.transform.localScale;
        Vector3 centerTargetScale = Vector3.one;
        int leftIndex = (currentCardIndex + cardPrefabs.Count) % cardPrefabs.Count;
        
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(leftIndex))
        {
            float scaleRatio = sideCardWidth / prefabRealSizes[leftIndex].x;
            centerTargetScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        
        Vector3 newCenterStartScale = newCenter.transform.localScale;
        Vector3 newCenterTargetScale = Vector3.one;
        int centerIndex = (currentCardIndex + 1) % cardPrefabs.Count;
        
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(centerIndex))
        {
            float scaleRatio = cardWidth / prefabRealSizes[centerIndex].x;
            newCenterTargetScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        
        // Se não estiver usando tamanho real, usar os RectTransforms
        RectTransform centerRect = newLeft.GetComponent<RectTransform>();
        Vector2 centerStartSize = centerRect ? centerRect.sizeDelta : new Vector2(cardWidth, 0);
        Vector2 centerTargetSize = new Vector2(sideCardWidth, centerStartSize.y);
        
        RectTransform newCenterRect = newCenter.GetComponent<RectTransform>();
        Vector2 newCenterStartSize = newCenterRect ? newCenterRect.sizeDelta : new Vector2(sideCardWidth, 0);
        Vector2 newCenterTargetSize = new Vector2(cardWidth, newCenterStartSize.y);
        
        // Restaurar efeitos do card central
        RestoreCenterCardEffects(newCenter);
        
        // Aplicar efeitos ao card que está se tornando lateral
        ApplySideCardEffects(newLeft, true);
        
        // Atualizar ordem dos elementos na hierarquia
        newCenter.transform.SetAsLastSibling();
        
        // Guardar rotações iniciais para interpolação
        Quaternion leftStartRotation = newLeft.transform.localRotation;
        Quaternion centerStartRotation = newCenter.transform.localRotation;
        Quaternion rightStartRotation = newRight.transform.localRotation;
        
        // Rotação alvo para o card esquerdo (se useRotationEffect estiver ativado)
        Quaternion leftTargetRotation = useRotationEffect ? 
            Quaternion.Euler(0, sideCardTilt, 0) : Quaternion.identity;
        
        // Rotação alvo para o card central
        Quaternion centerTargetRotation = Quaternion.identity;
        
        // Guardar valores de opacidade iniciais
        CanvasGroup leftCanvasGroup = newLeft.GetComponent<CanvasGroup>();
        CanvasGroup centerCanvasGroup = newCenter.GetComponent<CanvasGroup>();
        CanvasGroup rightCanvasGroup = newRight.GetComponent<CanvasGroup>();
        
        float leftStartAlpha = leftCanvasGroup ? leftCanvasGroup.alpha : 1f;
        float centerStartAlpha = centerCanvasGroup ? centerCanvasGroup.alpha : sideCardOpacity;
        float rightStartAlpha = rightCanvasGroup ? rightCanvasGroup.alpha : sideCardOpacity;
        
        // Animação
        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float curvedT = transitionCurve.Evaluate(t);  // Usar curva de animação personalizada
            
            // Animar posições
            newLeft.transform.localPosition = Vector3.Lerp(
                newLeft.transform.localPosition, 
                newLeftTarget, 
                curvedT
            );
            
            newCenter.transform.localPosition = Vector3.Lerp(
                newCenter.transform.localPosition, 
                newCenterTarget, 
                curvedT
            );
            
            newRight.transform.localPosition = Vector3.Lerp(
                newRight.transform.localPosition, 
                newRightTarget, 
                curvedT
            );
            
            // Animar rotações, se o efeito estiver ativado
            if (useRotationEffect)
            {
                newLeft.transform.localRotation = Quaternion.Slerp(
                    leftStartRotation,
                    leftTargetRotation,
                    curvedT
                );
                
                newCenter.transform.localRotation = Quaternion.Slerp(
                    centerStartRotation,
                    centerTargetRotation,
                    curvedT
                );
            }
            
            // Animar opacidade
            if (leftCanvasGroup)
                leftCanvasGroup.alpha = Mathf.Lerp(leftStartAlpha, sideCardOpacity, curvedT);
            
            if (centerCanvasGroup)
                centerCanvasGroup.alpha = Mathf.Lerp(centerStartAlpha, 1f, curvedT);
            
            // Animar tamanhos ou escalas
            if (useRealPrefabSize)
            {
                newLeft.transform.localScale = Vector3.Lerp(centerStartScale, centerTargetScale, curvedT);
                newCenter.transform.localScale = Vector3.Lerp(newCenterStartScale, newCenterTargetScale, curvedT);
            }
            else 
            {
                if (centerRect)
                    centerRect.sizeDelta = Vector2.Lerp(centerStartSize, centerTargetSize, curvedT);
                
                if (newCenterRect)
                    newCenterRect.sizeDelta = Vector2.Lerp(newCenterStartSize, newCenterTargetSize, curvedT);
            }
            
            // Aplicar efeito de perspectiva (deslocamento vertical)
            if (usePerspectiveEffect)
            {
                // Interpolar a posição Y para simular perspectiva
                Vector3 leftPos = newLeft.transform.localPosition;
                newLeft.transform.localPosition = new Vector3(
                    leftPos.x, 
                    Mathf.Lerp(leftPos.y, -5f, curvedT), 
                    leftPos.z
                );
                
                Vector3 centerPos = newCenter.transform.localPosition;
                newCenter.transform.localPosition = new Vector3(
                    centerPos.x, 
                    Mathf.Lerp(centerPos.y, 0f, curvedT), 
                    centerPos.z
                );
            }
            
            yield return null;
        }
        
        // Finalizar animação - garantir posições finais
        newLeft.transform.localPosition = new Vector3(newLeftTarget.x, usePerspectiveEffect ? -5f : 0f, newLeftTarget.z);
        newCenter.transform.localPosition = newCenterTarget;
        newRight.transform.localPosition = new Vector3(newRightTarget.x, usePerspectiveEffect ? -5f : 0f, newRightTarget.z);
        
        if (useRealPrefabSize)
        {
            newLeft.transform.localScale = centerTargetScale;
            newCenter.transform.localScale = newCenterTargetScale;
        }
        else
        {
            if (centerRect)
                centerRect.sizeDelta = centerTargetSize;
            
            if (newCenterRect)
                newCenterRect.sizeDelta = newCenterTargetSize;
        }
        
        // Atualizar referências e índice
        leftCard = newLeft;
        centerCard = newCenter;
        rightCard = newRight;
        
        currentCardIndex = (currentCardIndex + 1) % cardPrefabs.Count;
        
        // Reativar botões
        SetButtonsInteractive(true);
    }

    IEnumerator AnimateToPrevious()
    {
        float leftEdgeX = -viewportWidth / 2 + sideCardVisibleAmount;
        float rightEdgeX = viewportWidth / 2 - sideCardVisibleAmount;
        
        // O card da esquerda vai se tornar o central
        GameObject newCenter = leftCard;
        
        // O card central vai se tornar o da direita
        GameObject newRight = centerCard;
        
        // Destruir o card da direita atual
        Destroy(rightCard);
        
        // Criar o novo card da esquerda
        int newLeftIndex = (currentCardIndex - 2 + cardPrefabs.Count) % cardPrefabs.Count;
        GameObject newLeft = Instantiate(cardPrefabs[newLeftIndex], displayPoint);
        RectTransform leftRect = newLeft.GetComponent<RectTransform>();
        
        // Usar o tamanho real ou apenas redimensionar o RectTransform
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(newLeftIndex))
        {
            float scaleRatio = sideCardWidth / prefabRealSizes[newLeftIndex].x;
            newLeft.transform.localScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        else if (leftRect)
        {
            leftRect.sizeDelta = new Vector2(sideCardWidth, leftRect.sizeDelta.y);
        }
        
        // Posicionar fora da tela à esquerda e aplicar efeitos de card lateral
        newLeft.transform.localPosition = new Vector3(leftEdgeX - viewportWidth, 0, cardDepth);
        ApplySideCardEffects(newLeft, true);
        newLeft.transform.SetAsFirstSibling();
        
        // Preparar para animação
        Vector3 newRightTarget = new Vector3(rightEdgeX, 0, cardDepth);
        Vector3 newCenterTarget = new Vector3(0, 0, 0);
        Vector3 newLeftTarget = new Vector3(leftEdgeX, 0, cardDepth);
        
        // Variáveis para animação de escala
        Vector3 centerStartScale = newRight.transform.localScale;
        Vector3 centerTargetScale = Vector3.one;
        int rightIndex = (currentCardIndex + cardPrefabs.Count) % cardPrefabs.Count;
        
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(rightIndex))
        {
            float scaleRatio = sideCardWidth / prefabRealSizes[rightIndex].x;
            centerTargetScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        
        Vector3 newCenterStartScale = newCenter.transform.localScale;
        Vector3 newCenterTargetScale = Vector3.one;
        int centerIndex = (currentCardIndex - 1 + cardPrefabs.Count) % cardPrefabs.Count;
        
        if (useRealPrefabSize && prefabRealSizes.ContainsKey(centerIndex))
        {
            float scaleRatio = cardWidth / prefabRealSizes[centerIndex].x;
            newCenterTargetScale = new Vector3(scaleRatio, scaleRatio, 1f);
        }
        
        // Se não estiver usando tamanho real, usar os RectTransforms
        RectTransform centerRect = newRight.GetComponent<RectTransform>();
        Vector2 centerStartSize = centerRect ? centerRect.sizeDelta : new Vector2(cardWidth, 0);
        Vector2 centerTargetSize = new Vector2(sideCardWidth, centerStartSize.y);
        
        RectTransform newCenterRect = newCenter.GetComponent<RectTransform>();
        Vector2 newCenterStartSize = newCenterRect ? newCenterRect.sizeDelta : new Vector2(sideCardWidth, 0);
        Vector2 newCenterTargetSize = new Vector2(cardWidth, newCenterStartSize.y);
        
        // Restaurar efeitos do card central
        RestoreCenterCardEffects(newCenter);
        
        // Aplicar efeitos ao card que está se tornando lateral
        ApplySideCardEffects(newRight, false);
        
        // Atualizar ordem dos elementos na hierarquia
        newCenter.transform.SetAsLastSibling();
        
        // Guardar rotações iniciais para interpolação
        Quaternion leftStartRotation = newLeft.transform.localRotation;
        Quaternion centerStartRotation = newCenter.transform.localRotation;
        Quaternion rightStartRotation = newRight.transform.localRotation;
        
        // Rotação alvo para o card direito (se useRotationEffect estiver ativado)
        Quaternion rightTargetRotation = useRotationEffect ? 
            Quaternion.Euler(0, -sideCardTilt, 0) : Quaternion.identity;
        
        // Rotação alvo para o card central
        Quaternion centerTargetRotation = Quaternion.identity;
        
        // Guardar valores de opacidade iniciais
        CanvasGroup leftCanvasGroup = newLeft.GetComponent<CanvasGroup>();
        CanvasGroup centerCanvasGroup = newCenter.GetComponent<CanvasGroup>();
        CanvasGroup rightCanvasGroup = newRight.GetComponent<CanvasGroup>();
        
        float leftStartAlpha = leftCanvasGroup ? leftCanvasGroup.alpha : sideCardOpacity;
        float centerStartAlpha = centerCanvasGroup ? centerCanvasGroup.alpha : sideCardOpacity;
        float rightStartAlpha = rightCanvasGroup ? rightCanvasGroup.alpha : 1f;
        
        // Animação
        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float curvedT = transitionCurve.Evaluate(t);  // Usar curva de animação personalizada
            
            // Animar posições
            newRight.transform.localPosition = Vector3.Lerp(
                newRight.transform.localPosition, 
                newRightTarget, 
                curvedT
            );
            
            newCenter.transform.localPosition = Vector3.Lerp(
                newCenter.transform.localPosition, 
                newCenterTarget, 
                curvedT
            );
            
            newLeft.transform.localPosition = Vector3.Lerp(
                newLeft.transform.localPosition, 
                newLeftTarget, 
                curvedT
            );
            
            // Animar rotações, se o efeito estiver ativado
            if (useRotationEffect)
            {
                newRight.transform.localRotation = Quaternion.Slerp(
                    rightStartRotation,
                    rightTargetRotation,
                    curvedT
                );
                
                newCenter.transform.localRotation = Quaternion.Slerp(
                    centerStartRotation,
                    centerTargetRotation,
                    curvedT
                );
            }
            
            // Animar opacidade
            if (rightCanvasGroup)
                rightCanvasGroup.alpha = Mathf.Lerp(rightStartAlpha, sideCardOpacity, curvedT);
            
            if (centerCanvasGroup)
                centerCanvasGroup.alpha = Mathf.Lerp(centerStartAlpha, 1f, curvedT);
            
            // Animar tamanhos ou escalas
            if (useRealPrefabSize)
            {
                newRight.transform.localScale = Vector3.Lerp(centerStartScale, centerTargetScale, curvedT);
                newCenter.transform.localScale =newCenter.transform.localScale = Vector3.Lerp(newCenterStartScale, newCenterTargetScale, curvedT);
            }
            else 
            {
                if (centerRect)
                    centerRect.sizeDelta = Vector2.Lerp(centerStartSize, centerTargetSize, curvedT);
                
                if (newCenterRect)
                    newCenterRect.sizeDelta = Vector2.Lerp(newCenterStartSize, newCenterTargetSize, curvedT);
            }
            
            // Aplicar efeito de perspectiva (deslocamento vertical)
            if (usePerspectiveEffect)
            {
                // Interpolar a posição Y para simular perspectiva
                Vector3 rightPos = newRight.transform.localPosition;
                newRight.transform.localPosition = new Vector3(
                    rightPos.x, 
                    Mathf.Lerp(rightPos.y, -5f, curvedT), 
                    rightPos.z
                );
                
                Vector3 centerPos = newCenter.transform.localPosition;
                newCenter.transform.localPosition = new Vector3(
                    centerPos.x, 
                    Mathf.Lerp(centerPos.y, 0f, curvedT), 
                    centerPos.z
                );
            }
            
            yield return null;
        }
        
        // Finalizar animação - garantir posições finais
        newRight.transform.localPosition = new Vector3(newRightTarget.x, usePerspectiveEffect ? -5f : 0f, newRightTarget.z);
        newCenter.transform.localPosition = newCenterTarget;
        newLeft.transform.localPosition = new Vector3(newLeftTarget.x, usePerspectiveEffect ? -5f : 0f, newLeftTarget.z);
        
        if (useRealPrefabSize)
        {
            newRight.transform.localScale = centerTargetScale;
            newCenter.transform.localScale = newCenterTargetScale;
        }
        else
        {
            if (centerRect)
                centerRect.sizeDelta = centerTargetSize;
            
            if (newCenterRect)
                newCenterRect.sizeDelta = newCenterTargetSize;
        }
        
        // Atualizar referências e índice
        leftCard = newLeft;
        centerCard = newCenter;
        rightCard = newRight;
        
        currentCardIndex = (currentCardIndex - 1 + cardPrefabs.Count) % cardPrefabs.Count;
        
        // Reativar botões
        SetButtonsInteractive(true);
    }
    
    // Método para permitir ajustes dinâmicos da aparência dos cards
    public void UpdateCardAppearance()
    {
        // Atualizar cards laterais com novas configurações
        if (leftCard != null)
        {
            ApplySideCardEffects(leftCard, true);
        }
        
        if (rightCard != null)
        {
            ApplySideCardEffects(rightCard, false);
        }
        
        // Recalcular posições
        float leftEdgeX = -viewportWidth / 2 + sideCardVisibleAmount;
        float rightEdgeX = viewportWidth / 2 - sideCardVisibleAmount;
        
        if (leftCard != null)
        {
            Vector3 leftPos = leftCard.transform.localPosition;
            leftCard.transform.localPosition = new Vector3(leftEdgeX, leftPos.y, leftPos.z);
        }
        
        if (rightCard != null)
        {
            Vector3 rightPos = rightCard.transform.localPosition;
            rightCard.transform.localPosition = new Vector3(rightEdgeX, rightPos.y, rightPos.z);
        }
    }
    
    // Método para permitir acesso e controle programático do card atual
    public int GetCurrentCardIndex()
    {
        return currentCardIndex;
    }
    
    // Método para pular diretamente para um card específico
    public void JumpToCard(int targetIndex)
    {
        // Validar índice
        if (targetIndex < 0 || targetIndex >= cardPrefabs.Count)
            return;
            
        // Se for o mesmo card, não fazer nada
        if (targetIndex == currentCardIndex)
            return;
            
        // Limpar cards atuais
        if (leftCard != null)
            Destroy(leftCard);
            
        if (centerCard != null)
            Destroy(centerCard);
            
        if (rightCard != null)
            Destroy(rightCard);
            
        // Atualizar índice
        currentCardIndex = targetIndex;
        
        // Recriar os cards
        SetupInitialCards();
    }
}