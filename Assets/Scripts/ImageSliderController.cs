using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageSliderController : MonoBehaviour
{
    [Header("Imagens do Slider")]
    [SerializeField] private List<RectTransform> imagePanels;
    
    [Header("Configurações de Navegação")]
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private List<Button> indicatorButtons;
    [SerializeField] private Color activeIndicatorColor = Color.white;
    [SerializeField] private Color inactiveIndicatorColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    
    [Header("Configurações de Animação")]
    [SerializeField] private float transitionSpeed = 10f;
    [SerializeField] private float snapThreshold = 0.5f;
    [SerializeField] private float inactiveAlpha = 0.5f;
    [SerializeField] private float inactiveScale = 0.85f;
    
    [Header("Configurações de Áudio")]
    [SerializeField] private AudioClip transitionSound;
    [SerializeField] private float volume = 1.0f;
    
    private ScrollRect scrollRect;
    private int currentIndex = 0;
    private int totalImages;
    private bool isDragging = false;
    private bool isAnimating = false;
    private float targetNormalizedPos = 0f;
    private AudioSource audioSource;
    private bool soundPlayed = false;
    
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            Debug.LogError("Este script requer um componente ScrollRect!");
            return;
        }
        
        // Configurar AudioSource
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        totalImages = imagePanels.Count;
        
        // Configurar botões de navegação
        if (prevButton != null) prevButton.onClick.AddListener(GoPrevious);
        if (nextButton != null) nextButton.onClick.AddListener(GoNext);
        
        // Configurar botões indicadores
        for (int i = 0; i < indicatorButtons.Count; i++)
        {
            int index = i; // Necessário para a captura de closure
            indicatorButtons[i].onClick.AddListener(() => GoToIndex(index));
        }
        
        // Adicionar eventos de drag
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = gameObject.AddComponent<EventTrigger>();
            
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        beginDragEntry.callback.AddListener((data) => { OnBeginDrag(); });
        trigger.triggers.Add(beginDragEntry);
        
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener((data) => { OnEndDrag(); });
        trigger.triggers.Add(endDragEntry);
    }
    
    private void Start()
    {
        // Posicionar inicialmente no primeiro slide
        UpdateSliderState(0, true);
    }
    
    private void Update()
    {
        if (isAnimating)
        {
            // Animar transição suave para a posição alvo
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition, 
                targetNormalizedPos, 
                Time.deltaTime * transitionSpeed);
                
            // Reproduza o som quando a animação estiver 50% completa
            float distanceToTarget = Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetNormalizedPos);
            float totalDistance = 1.0f / (totalImages - 1); // Distância entre duas imagens
            
            if (distanceToTarget < totalDistance * 0.25f && !soundPlayed)
            {
                PlayTransitionSound();
                soundPlayed = true;
            }
                
            // Verificar se a animação está concluída
            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetNormalizedPos) < 0.001f)
            {
                scrollRect.horizontalNormalizedPosition = targetNormalizedPos;
                isAnimating = false;
            }
            
            // ADICIONADO: Aplicar efeitos visuais mesmo durante a animação
            UpdateVisualEffects();
        }
        else if (!isDragging)
        {
            // Determinar o índice baseado na posição de rolagem atual
            float normalizedPos = scrollRect.horizontalNormalizedPosition;
            int newIndex = Mathf.RoundToInt(normalizedPos * (totalImages - 1));
            
            if (newIndex != currentIndex)
            {
                UpdateSliderState(newIndex, false);
            }
            
            // Aplicar o efeito visual (transparência e zoom) baseado na posição
            UpdateVisualEffects();
        }
        else
        {
            // ADICIONADO: Aplicar efeitos visuais também durante o arrasto
            UpdateVisualEffects();
        }
    }
    
    private void UpdateVisualEffects()
    {
        float normalizedPos = scrollRect.horizontalNormalizedPosition;
        
        for (int i = 0; i < totalImages; i++)
        {
            float targetPos = (float)i / (totalImages - 1);
            float distance = Mathf.Abs(normalizedPos - targetPos);
            
            // Obter a imagem e seu componente CanvasGroup
            RectTransform imagePanel = imagePanels[i];
            CanvasGroup canvasGroup = imagePanel.GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                canvasGroup = imagePanel.gameObject.AddComponent<CanvasGroup>();
            }
            
            // Calcular a transparência baseada na distância do centro
            canvasGroup.alpha = Mathf.Lerp(1f, inactiveAlpha, distance * 2f);
            
            // Aplicar efeito de escala nas imagens não centrais
            float scale = Mathf.Lerp(1f, inactiveScale, distance * 2f);
            imagePanel.localScale = new Vector3(scale, scale, 1f);
        }
    }
    
    private void UpdateSliderState(int newIndex, bool immediate)
    {
        if (newIndex < 0 || newIndex >= totalImages)
            return;
            
        currentIndex = newIndex;
        targetNormalizedPos = (float)currentIndex / (totalImages - 1);
        
        if (immediate)
        {
            scrollRect.horizontalNormalizedPosition = targetNormalizedPos;
            isAnimating = false;
        }
        else
        {
            isAnimating = true;
            soundPlayed = false; // Resetar o controle de som para a nova transição
        }
        
        // Atualizar a aparência dos indicadores
        for (int i = 0; i < indicatorButtons.Count; i++)
        {
            Image indicatorImage = indicatorButtons[i].GetComponent<Image>();
            if (indicatorImage != null)
            {
                indicatorImage.color = (i == currentIndex) ? activeIndicatorColor : inactiveIndicatorColor;
            }
        }
    }
    
    private void PlayTransitionSound()
    {
        if (transitionSound != null && audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(transitionSound);
        }
    }
    
    private void OnBeginDrag()
    {
        isDragging = true;
        isAnimating = false;
    }
    
    private void OnEndDrag()
    {
        isDragging = false;
        
        // Determinar para qual slide fazer o snap
        float normalizedPos = scrollRect.horizontalNormalizedPosition;
        int closestIndex = Mathf.RoundToInt(normalizedPos * (totalImages - 1));
        
        UpdateSliderState(closestIndex, false);
    }
    
    public void GoToIndex(int index)
    {
        if (index < 0 || index >= totalImages)
            return;
            
        UpdateSliderState(index, false);
    }
    
    public void GoNext()
    {
        GoToIndex(currentIndex + 1);
    }
    
    public void GoPrevious()
    {
        GoToIndex(currentIndex - 1);
    }
}