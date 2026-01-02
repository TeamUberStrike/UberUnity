using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MobileUIController : MonoBehaviour
{
    [Header("UI Button References")]
    public Button fireButton;
    public Button aimButton;
    public Button jumpButton;
    public Button crouchButton;
    public Button menuButton;
    public Button chatButton;
    public Button scoreboardButton;
    public Button statisticsButton;
    
    [Header("UI Images")]
    public Image fireButtonImage;
    public Image aimButtonImage;
    public Image jumpButtonImage;
    public Image crouchButtonImage;
    public Image menuButtonImage;
    public Image chatButtonImage;
    public Image scoreboardButtonImage;
    public Image statisticsButtonImage;
    
    private PlayerInput playerInput;
    private PlayerMotor playerMotor;
    
    // Position data from TouchInput SetupRects
    private void SetupButtonPositions()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // Position buttons based on TouchInput.cs positioning
        if (fireButton != null)
        {
            Vector2 firePos = new Vector2(screenWidth - 160, screenHeight - 170);
            SetButtonPosition(fireButton, firePos);
        }
        
        if (aimButton != null)
        {
            Vector2 aimPos = new Vector2(screenWidth - 64, screenHeight - 234);
            SetButtonPosition(aimButton, aimPos);
        }
        
        if (jumpButton != null)
        {
            Vector2 jumpPos = new Vector2(screenWidth - 88, screenHeight - 72);
            SetButtonPosition(jumpButton, jumpPos);
        }
        
        if (crouchButton != null)
        {
            Vector2 crouchPos = new Vector2(screenWidth - 255, screenHeight - 116);
            SetButtonPosition(crouchButton, crouchPos);
        }
        
        if (menuButton != null)
        {
            Vector2 menuPos = new Vector2(30f, 200f);
            SetButtonPosition(menuButton, menuPos);
        }
        
        if (chatButton != null)
        {
            Vector2 chatPos = new Vector2(30f, 260f);
            SetButtonPosition(chatButton, chatPos);
        }
        
        if (scoreboardButton != null)
        {
            Vector2 scorePos = new Vector2(30f, 320f);
            SetButtonPosition(scoreboardButton, scorePos);
        }
        
        if (statisticsButton != null)
        {
            Vector2 statsPos = new Vector2(30f, 380f);
            SetButtonPosition(statisticsButton, statsPos);
        }
    }
    
    private void SetButtonPosition(Button button, Vector2 screenPosition)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Convert screen position to Canvas position
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Vector2 canvasPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform, 
                    screenPosition, 
                    canvas.worldCamera, 
                    out canvasPosition
                );
                rectTransform.anchoredPosition = canvasPosition;
            }
        }
    }
    
    private void SetupButtonSprites()
    {
        // Apply MobileIcons texture atlas coordinates to button images
        if (MobileIcons.TextureAtlas != null)
        {
            Material atlasMaterial = MobileIcons.TextureAtlas;
            
            if (fireButtonImage != null)
                SetButtonSprite(fireButtonImage, atlasMaterial, MobileIcons.TouchFireButton);
                
            if (aimButtonImage != null)
                SetButtonSprite(aimButtonImage, atlasMaterial, MobileIcons.TouchSecondFireButton);
                
            if (jumpButtonImage != null)
                SetButtonSprite(jumpButtonImage, atlasMaterial, MobileIcons.TouchJumpButton);
                
            if (crouchButtonImage != null)
                SetButtonSprite(crouchButtonImage, atlasMaterial, MobileIcons.TouchCrouchButton);
                
            if (menuButtonImage != null)
                SetButtonSprite(menuButtonImage, atlasMaterial, MobileIcons.TouchMenuButton);
                
            if (chatButtonImage != null)
                SetButtonSprite(chatButtonImage, atlasMaterial, MobileIcons.TouchChatButton);
                
            if (scoreboardButtonImage != null)
                SetButtonSprite(scoreboardButtonImage, atlasMaterial, MobileIcons.TouchScoreboardButton);
        }
    }
    
    private void SetButtonSprite(Image image, Material atlasMaterial, Rect atlasRect)
    {
        if (atlasMaterial.mainTexture != null)
        {
            Texture2D atlasTexture = atlasMaterial.mainTexture as Texture2D;
            if (atlasTexture != null)
            {
                // Create sprite from atlas texture using the rect coordinates
                Sprite buttonSprite = Sprite.Create(
                    atlasTexture,
                    new Rect(
                        atlasRect.x * atlasTexture.width,
                        atlasRect.y * atlasTexture.height,
                        atlasRect.width * atlasTexture.width,
                        atlasRect.height * atlasTexture.height
                    ),
                    new Vector2(0.5f, 0.5f)
                );
                image.sprite = buttonSprite;
            }
        }
    }
    
    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerMotor = GetComponentInParent<PlayerMotor>();
        
        SetupButtonPositions();
        SetupButtonSprites();
        //SetupButtonEvents();
    }
    
    // private void SetupButtonEvents()
    // {
    //     // Connect buttons to PlayerInput methods
    //     if (statisticsButton != null && playerInput != null)
    //     {
    //         statisticsButton.onClick.AddListener(() => playerInput.OnStatisticsButtonPressed());
    //     }
        
    //     // Add other button event handlers as needed
    //     // These would connect to the appropriate input system events
    // }
}
