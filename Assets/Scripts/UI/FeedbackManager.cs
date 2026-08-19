using UnityEngine;
using TMPro;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("Prefab & Canvas")]
    [Tooltip("A simple prefab: a TextMeshProUGUI on a small Canvas/RectTransform")]
    public GameObject floatingTextPrefab;
    public Canvas worldSpaceCanvas; // assign a World Space canvas, or leave null to auto-create one

    [Header("Animation")]
    public float floatDistance = 1.2f;
    public float duration = 0.9f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Color Language")]
    [Tooltip("Damage taken, negative events")]
    public Color negativeColor = new Color(0.85f, 0.25f, 0.25f);
    [Tooltip("Healing, rewards, positive events")]
    public Color positiveColor = new Color(0.35f, 0.75f, 0.35f);
    [Tooltip("Neutral info, e.g. resource counts")]
    public Color neutralColor = new Color(0.9f, 0.9f, 0.9f);

    [Header("Test Mode (remove once real systems hook in)")]
    [Tooltip("Press keys to preview feedback without a real combat system")]
    public bool enableTestKeys = true;
    public Transform testSpawnPoint; // e.g. the player or a dummy cube in the scene

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (!enableTestKeys || testSpawnPoint == null) return;

        // Simulate taking damage
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ShowFloatingText(testSpawnPoint.position, "-12", FeedbackType.Negative);

        // Simulate a reward / heal
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ShowFloatingText(testSpawnPoint.position, "+8", FeedbackType.Positive);

        // Simulate a neutral pickup (e.g. resource count)
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ShowFloatingText(testSpawnPoint.position, "+1 Wood", FeedbackType.Neutral);
    }

    public enum FeedbackType { Negative, Positive, Neutral }

    public void ShowFloatingText(Vector3 worldPosition, string text, FeedbackType type)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning("FeedbackManager: floatingTextPrefab not assigned.");
            return;
        }

        GameObject instance = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity,
                                           worldSpaceCanvas != null ? worldSpaceCanvas.transform : null);

        TextMeshProUGUI tmp = instance.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text;
            tmp.color = GetColor(type);
        }

        StartCoroutine(AnimateAndDestroy(instance));
    }

    Color GetColor(FeedbackType type)
    {
        switch (type)
        {
            case FeedbackType.Negative: return negativeColor;
            case FeedbackType.Positive: return positiveColor;
            default: return neutralColor;
        }
    }

    System.Collections.IEnumerator AnimateAndDestroy(GameObject instance)
    {
        Vector3 startPos = instance.transform.position;
        Vector3 endPos = startPos + Vector3.up * floatDistance;
        TextMeshProUGUI tmp = instance.GetComponentInChildren<TextMeshProUGUI>();
        Color baseColor = tmp != null ? tmp.color : Color.white;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            instance.transform.position = Vector3.Lerp(startPos, endPos, normalized);

            if (tmp != null)
            {
                float alpha = fadeCurve.Evaluate(normalized);
                tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            }

            yield return null;
        }

        Destroy(instance);
    }
}