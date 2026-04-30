using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class SpriteNumberDisplay : MonoBehaviour
{
    private enum NumberAlignment
    {
        Left,
        Center,
        Right
    }

    [SerializeField] private Sprite[] digitSprites = new Sprite[10];
    [SerializeField] private Image digitTemplate;
    [SerializeField] private Vector2 digitSize = new Vector2(24f, 36f);
    [SerializeField] private float digitSpacing = 4f;
    [SerializeField] private NumberAlignment alignment = NumberAlignment.Center;
    [SerializeField] private bool drawZeroOnAwake = true;
    [SerializeField] private float popScale = 0.2f;
    [SerializeField] private float popDuration = 0.18f;

    private readonly List<Image> activeDigits = new();
    private Vector3 baseScale;
    private float popTimer;
    private int currentValue = -1;
    private bool initialized;

    private void Awake()
    {
        Initialize();

        if (drawZeroOnAwake)
        {
            SetValue(0, false);
        }
    }

    private void Update()
    {
        if (popTimer <= 0f)
        {
            return;
        }

        popTimer = Mathf.Max(0f, popTimer - Time.unscaledDeltaTime);
        float progress = 1f - (popTimer / popDuration);
        float scale = 1f + Mathf.Sin(progress * Mathf.PI) * popScale;
        transform.localScale = baseScale * scale;

        if (popTimer <= 0f)
        {
            transform.localScale = baseScale;
        }
    }

    public void SetValue(int value)
    {
        SetValue(value, true);
    }

    public void SetValue(int value, bool animate)
    {
        Initialize();

        value = Mathf.Max(0, value);

        if (currentValue == value && activeDigits.Count > 0)
        {
            return;
        }

        currentValue = value;
        RebuildDigits(value);

        if (animate)
        {
            popTimer = popDuration;
        }
        else
        {
            popTimer = 0f;
            transform.localScale = baseScale;
        }
    }

    private void Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        baseScale = transform.localScale;

        if (digitTemplate == null)
        {
            digitTemplate = GetComponentInChildren<Image>(true);
        }

        if (digitTemplate != null)
        {
            digitTemplate.gameObject.SetActive(false);
        }
    }

    private void RebuildDigits(int value)
    {
        ClearDigits();

        string text = value.ToString();
        float totalWidth = text.Length * digitSize.x + Mathf.Max(0, text.Length - 1) * digitSpacing;

        for (int i = 0; i < text.Length; i++)
        {
            int digit = text[i] - '0';
            Image image = CreateDigitImage(i);
            image.sprite = GetDigitSprite(digit);
            image.preserveAspect = true;
            image.raycastTarget = false;

            RectTransform rectTransform = image.rectTransform;
            rectTransform.sizeDelta = digitSize;
            ApplyDigitPosition(rectTransform, i, totalWidth);

            activeDigits.Add(image);
        }
    }

    private Image CreateDigitImage(int index)
    {
        Image image;

        if (digitTemplate != null)
        {
            image = Instantiate(digitTemplate, transform);
            image.gameObject.SetActive(true);
        }
        else
        {
            GameObject digitObject = new GameObject($"Digit_{index}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            digitObject.transform.SetParent(transform, false);
            image = digitObject.GetComponent<Image>();
        }

        image.name = $"Digit_{index}";
        return image;
    }

    private Sprite GetDigitSprite(int digit)
    {
        if (digitSprites == null ||
            digit < 0 ||
            digit >= digitSprites.Length)
        {
            return null;
        }

        return digitSprites[digit];
    }

    private void ApplyDigitPosition(RectTransform rectTransform, int index, float totalWidth)
    {
        float step = digitSize.x + digitSpacing;
        Vector2 anchor;
        Vector2 pivot;
        float x;

        switch (alignment)
        {
            case NumberAlignment.Left:
                anchor = new Vector2(0f, 0.5f);
                pivot = new Vector2(0f, 0.5f);
                x = index * step;
                break;

            case NumberAlignment.Right:
                anchor = new Vector2(1f, 0.5f);
                pivot = new Vector2(1f, 0.5f);
                x = -totalWidth + digitSize.x + index * step;
                break;

            case NumberAlignment.Center:
            default:
                anchor = new Vector2(0.5f, 0.5f);
                pivot = new Vector2(0.5f, 0.5f);
                x = -totalWidth * 0.5f + digitSize.x * 0.5f + index * step;
                break;
        }

        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = new Vector2(x, 0f);
    }

    private void ClearDigits()
    {
        for (int i = activeDigits.Count - 1; i >= 0; i--)
        {
            if (activeDigits[i] != null)
            {
                Destroy(activeDigits[i].gameObject);
            }
        }

        activeDigits.Clear();
    }

    private void OnValidate()
    {
        digitSize.x = Mathf.Max(1f, digitSize.x);
        digitSize.y = Mathf.Max(1f, digitSize.y);
        digitSpacing = Mathf.Max(0f, digitSpacing);
        popScale = Mathf.Max(0f, popScale);
        popDuration = Mathf.Max(0.01f, popDuration);
    }
}
