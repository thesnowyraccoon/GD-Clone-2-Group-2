using UnityEngine;
using Ilumisoft.HealthSystem;

/// <summary>
/// Thin wrapper around the Ilumisoft Health component. Keeps a stable
/// TakeDamage/Heal API for the rest of the codebase (EnemyController, etc.)
/// while delegating actual health tracking to the asset, which also drives
/// its own Health Bar UI directly off the Health component.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public Health Health { get; private set; }

    void Awake()
    {
        Health = GetComponent<Health>();

        if (Health == null)
        {
            Debug.LogWarning($"{name}: No Health component found. Add the Ilumisoft Health component manually and wire it to the Health Bar prefab.");
            return;
        }

        Health.OnHealthEmpty += HandleHealthEmpty;
    }

    void OnDestroy()
    {
        if (Health != null)
        {
            Health.OnHealthEmpty -= HandleHealthEmpty;
        }
    }

    /// <summary>Call this from enemies, traps, etc.</summary>
    public void TakeDamage(float amount)
    {
        if (Health == null) return;

        Health.ApplyDamage(amount);

        Debug.Log($"Player took {amount} damage, {Health.CurrentHealth}/{Health.MaxHealth} HP remaining");

        if (FeedbackManager.Instance != null)
        {
            FeedbackManager.Instance.ShowFloatingText(transform.position, $"-{amount:F0}", FeedbackManager.FeedbackType.Negative);
        }
    }

    /// <summary>Call this from healing items, rest points, etc.</summary>
    public void Heal(float amount)
    {
        if (Health == null) return;

        Health.AddHealth(amount);

        if (FeedbackManager.Instance != null)
        {
            FeedbackManager.Instance.ShowFloatingText(transform.position, $"+{amount:F0}", FeedbackManager.FeedbackType.Positive);
        }
    }

    void HandleHealthEmpty()
    {
        Debug.Log("Player died");
        // Hook up respawn / game over logic here once it exists
    }
}