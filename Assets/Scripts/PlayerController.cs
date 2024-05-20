using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamagable
{
    enum State 
    {
        IDLE, 
        MOVING, 
        PUSHING, 
        STUNNED, 
        DEATH
    }

    State currentState;
    Rigidbody rb;
    Coroutine coroutine;
    LayerMask obstacleMask;
    float currentHealth;
    float timeElapsed = 0f;
    bool canBeStunned = true;

    [SerializeField] float maxHealth = 5f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float pushDuration = 1.2f;
    [SerializeField] float stunDuration = 2.5f;
    [SerializeField] float interationRange = 1f;
    [SerializeField] Slider healthBar;
    [SerializeField] GameObject controlHint;

    public Vector3 MoveDir { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        // assign variables
        rb = GetComponent<Rigidbody>();
        obstacleMask = LayerMask.GetMask("Obstacles");
        canBeStunned = true;

        // set health
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
            healthBar.gameObject.SetActive(false);
        }

        // hide UI game objects
        if (controlHint != null) controlHint.SetActive(false);
        // set default state
        currentState = State.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        // check any state transitions
        HandleInteractionWithObject();

        // handle state behaviours
        switch (currentState)
        {
            case State.IDLE:
                idle();
                break;
            case State.MOVING:
                moving();
                break;
            case State.PUSHING:
                pushing();
                break;
            case State.STUNNED:
                if (coroutine != null) return;
                coroutine = StartCoroutine(stunned());
                break;
            case State.DEATH:
                death();
                break;
            default:
                Debug.LogError("State not found.");
                break;
        }
    }

    // gizmos
    void OnDrawGizmosSelected() 
    {
        Gizmos.DrawWireSphere(transform.position, interationRange);
    }

    // interface methods
    public void Damage(float damage)
    {
        // prevent damage when pushing (aka when cannot be stunned)
        if (!canBeStunned) return;
        // change health
        currentHealth -= damage;
        // update health bar
        if (healthBar != null)
        { 
            if (!healthBar.gameObject.activeInHierarchy) healthBar.gameObject.SetActive(true);
            healthBar.value = currentHealth;
        }
        // check if player has died
        if (currentHealth > 0f) return;
        // switch to death state
        currentState = State.DEATH;
    }

    // handle states
    void idle()
    {
        // reset move direction
        MoveDir = Vector3.zero;
        // allow stun
        canBeStunned = true;
        // check transition to moving
        if (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) == Vector2.zero) return;
        currentState = State.MOVING;
    }

    void moving()
    {
        // allow stun
        canBeStunned = true;

        // get move direction
        MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        
        // check for state transition to idle
        if (MoveDir == Vector3.zero)
        {
            currentState = State.IDLE;
            return;
        }

        // check for running (shift pressed), if running, multiple speed by 1.5f.
        rb.AddForce(MoveDir * moveSpeed * (Input.GetKey(KeyCode.LeftShift)? 1.5f : 1f));
    }

    void pushing()
    {
        // do not allow self stun
        canBeStunned = false;
        // reset move direction
        MoveDir = Vector3.zero;

        if (timeElapsed >= pushDuration)
        {
            currentState = State.IDLE;
            return;
        }

        timeElapsed += Time.deltaTime;
    }

    void death()
    {
        // reset move direction
        MoveDir = Vector3.zero;
        // allow stun
        canBeStunned = true;
        // do death stuff
        Debug.Log("Player Died: Mission Failed!");
        Destroy(gameObject);
    }

    IEnumerator stunned()
    {
        // reset move direction
        MoveDir = Vector3.zero;
        // double stunning not allowed
        canBeStunned = false;
        // wait for stun duration
        yield return new WaitForSeconds(stunDuration);
        coroutine = null;
        currentState = State.IDLE;
    }

    // any state transition handlers
    void HandleInteractionWithObject()
    {
        // ensure state not in pushing
        if (currentState == State.PUSHING) return;
        // check if there are obstacles nearby
        Collider[] hit = Physics.OverlapSphere(transform.position, interationRange, obstacleMask);
        // exit function if no nearby obstacles are found, and hide UI
        if (hit.Length <= 0)
        {
            if (controlHint != null) controlHint.SetActive(false);
            return;
        }
        // show UI if near object
        if (controlHint != null) controlHint.SetActive(true);
        // check inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // drop object when space is pressed
            PushableObject obj = hit[0].transform.parent.GetComponent<PushableObject>();
            // check if object drop is successful
            if (obj == null || !obj.DropObject(out Vector3 pushSpot)) return;
            // handle successful response
            // set position to pushing spot
            transform.position = new Vector3(pushSpot.x, transform.position.y, pushSpot.z);
            // hide message
            if (controlHint != null) controlHint.SetActive(false);
            // reset time elasped
            timeElapsed = 0f;
            // change state if pushing is successful
            currentState = State.PUSHING;
        }
    }

    public void Stun()
    {
        if (!canBeStunned) return;
        currentState = State.STUNNED;
    }
}
