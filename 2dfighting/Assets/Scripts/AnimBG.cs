using UnityEngine;
using UnityEngine.UI;

public class PanelBackgroundAnimator : MonoBehaviour
{
    public Sprite[] backgrounds;
    public float frameRate = 0.5f;

    private Image panelImage;
    private int currentFrame;
    private float timer;

    void Start()
    {
        panelImage = GetComponent<Image>();
        currentFrame = 0;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame++;
            if (currentFrame >= backgrounds.Length)
                currentFrame = 0;
            panelImage.sprite = backgrounds[currentFrame];
        }
    }
}