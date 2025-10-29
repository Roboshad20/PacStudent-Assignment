using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Animator animator;
    public LayerMask wallLayer;
    public float checkRadius = 0.1f;
    public AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip eatPelletSound;
    public ParticleSystem dustParticles;

    private Vector2Int currentGridPos;
    private Vector2Int targetGridPos;
    private Vector2Int currentInput = Vector2Int.zero;
    private Vector2Int lastInput = Vector2Int.zero;
    private bool isMoving = false;
    private float moveTimer = 0f;

    private enum AnimState
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
    }

    private AnimState currentState = AnimState.RIGHT;
    void Start()
    {
        Vector3 pos = transform.position;
        currentGridPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        targetGridPos = currentGridPos;
        transform.position = new Vector3(currentGridPos.x, currentGridPos.y, 0);

        if (animator == null)
            animator = GetComponent<Animator>();

        animator.speed = 0f;
    }

    void Update()
    {
        HandleInput();
        if (!isMoving)
        {
            TryMove();
        }
        else
        {
            ContinueMove();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector2Int.right;
    }

    void TryMove()
    {
        if (CanMove(lastInput))
        {
            if (!isMoving)
            {
                currentInput = lastInput;
                StartMove(currentInput);
            }
        }
        else if (CanMove(currentInput))
        {
            if (!isMoving)
            {
                StartMove(currentInput);
            }
        }
        else
        {
            animator.speed = 0f;
        }
    }

    bool CanMove(Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return false;

        Vector2 nextPos = new Vector2(currentGridPos.x + dir.x, currentGridPos.y + dir.y);

        Collider2D[] hits = Physics2D.OverlapCircleAll(nextPos, checkRadius, wallLayer);

        return hits.Length == 0;
    }

    void StartMove(Vector2Int dir)
    {
        isMoving = true;
        moveTimer = 0f;
        targetGridPos = currentGridPos + dir;

        AnimState newState = currentState;

        if (dir == Vector2Int.left)
            newState = AnimState.LEFT;
        else if (dir == Vector2Int.right)
            newState = AnimState.RIGHT;
        else if (dir == Vector2Int.up)
            newState = AnimState.UP;
        else if (dir == Vector2Int.down)
            newState = AnimState.DOWN;

        if (newState != currentState)
        {
            currentState = newState;
            PlayAnimationState(newState);
        }

        animator.speed = 1f;

        if (dustParticles != null)
        dustParticles.Play();

        if (audioSource != null)
        {
            audioSource.clip = moveSound;
            audioSource.Play();
        }
    }

    void ContinueMove()
    {
        moveTimer += Time.deltaTime * moveSpeed;
        float t = Mathf.Clamp01(moveTimer);

        Vector3 startPos = new Vector3(currentGridPos.x, currentGridPos.y, 0);
        Vector3 targetPos = new Vector3(targetGridPos.x, targetGridPos.y, 0);

        transform.position = Vector3.Lerp(startPos, targetPos, t);

        if (t >= 1f)
        {
            currentGridPos = targetGridPos;
            isMoving = false;

            if (dustParticles != null)
                dustParticles.Stop();

            if (audioSource != null)
                audioSource.Stop();
        }
    }

    void PlayAnimationState(AnimState state)
    {
        switch (state)
        {
            case AnimState.LEFT:
                animator.SetTrigger("Left");
                break;
            case AnimState.RIGHT:
                animator.SetTrigger("Right");
                break;
            case AnimState.UP:
                animator.SetTrigger("Up");
                break;
            case AnimState.DOWN:
                animator.SetTrigger("Down");
                break;
        }
    }
}
