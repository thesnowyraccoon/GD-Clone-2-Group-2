using UnityEngine;
using Ilumisoft.HealthSystem;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float contactDamage = 10f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 6f;
    public float stopDistance = 1.2f; // stays this far from player once in range
    float gravity = -9.8f;
    float verticalVelocity;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    float attackTimer;

    [Header("References")]
    public Transform player; // assign in Inspector, or auto-found by tag below

    CharacterController cc;
    Health health; // Ilumisoft Health component - also drives this asset's own Health Bar UI

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        health = GetComponent<Health>();

        if (health == null)
        {
            Debug.LogWarning($"{name}: No Health component found. Add the Ilumisoft Health component manually and wire it to the Health Bar prefab.");
            return;
        }

        health.OnHealthEmpty += Die;
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthEmpty -= Die;
        }
    }

    void Start()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }
    }

    void Update()
    {
        if (health == null || health.IsAlive == false || player == null) return;

        attackTimer -= Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        // --- Chase ---
        Vector3 move = Vector3.zero;
        if (distance <= detectionRange && distance > stopDistance)
        {
            Vector3 direction = (player.position - transform.position);
            direction.y = 0f;
            direction.Normalize();

            move = direction * moveSpeed;

            // face the player
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction), 10f * Time.deltaTime);
            }
        }

        // --- Gravity (same pattern as MovementController) ---
        if (cc.isGrounded)
        {
            verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        move.y = verticalVelocity;

        cc.Move(move * Time.deltaTime);

        // --- Attack ---
        if (distance <= attackRange && attackTimer <= 0f)
        {
            Attack();
        }
    }

    void Attack()
    {
        attackTimer = attackCooldown;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage);
        }
    }

    /// <summary>
    /// Convenience wrapper for non-hitbox damage sources (explosions, DOT, etc).
    /// For weapon hits, prefer adding an Ilumisoft Hitbox + Collider directly to
    /// this enemy instead - it calls Health.ApplyDamage() automatically.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (health == null || health.IsAlive == false) return;

        health.ApplyDamage(amount);

        if (FeedbackManager.Instance != null)
        {
            FeedbackManager.Instance.ShowFloatingText(transform.position, $"-{amount:F0}", FeedbackManager.FeedbackType.Negative);
        }
    }

    void Die()
    {
        Destroy(gameObject, 0.1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}