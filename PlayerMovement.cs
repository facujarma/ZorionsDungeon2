using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] int speed;

    Rigidbody2D rb;
    [SerializeField] GameObject map_generation;
    [SerializeField] int playerPossibleMovesDistance, dashForce;
    [SerializeField] float dashCooldown;

    bool canDash = true, isDashing = false;

    public bool[,] playerPossibleMoves = new bool[125, 125];
    int[,] mp;
    mapGeneration m_generation;

    void Start()
    {
        m_generation = map_generation.GetComponent<mapGeneration>();
        mp = m_generation.map;
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(x, y);
        movement.Normalize();

        //DASH:
        if (!isDashing) rb.velocity = movement * speed;

        if(Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash(new Vector2(x, y)));
        }

        UpdatePlayerPossibleMoves();
    }

    void UpdatePlayerPossibleMoves()
    {
        for (int y = 0; y < m_generation.height; y++)
        {
            for (int x = 0; x < m_generation.width; x++)
            {
                playerPossibleMoves[y,x] = false;
            }
        }

        Vector2Int roundedPosition = Vector2Int.RoundToInt(transform.position);
        int posX = roundedPosition.x;
        int posY = roundedPosition.y;

        for (int y = posY - playerPossibleMovesDistance; y <= posY + playerPossibleMovesDistance; y++)
        {
            for (int x = posX - playerPossibleMovesDistance; x <= posX + playerPossibleMovesDistance; x++)
            {
                if (y >= 0 && y < m_generation.height && x >= 0 && x < m_generation.width)
                {
                    if (mp[y, x] != 0 && mp[y, x] != 2)
                    {
                        Vector2 t = new Vector2(x, y) - (Vector2)transform.position;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position,t.normalized, 2f);
                        if (hit.collider == null)
                        {
                            playerPossibleMoves[y, x] = true;
                        }
                            
                    }
                }
            }
        }
    }

    private IEnumerator Dash(Vector2 dir)
    {
        isDashing = true;
        canDash = false;
        rb.velocity = dir * dashForce;
        yield return new WaitForSeconds(0.2f);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Break boxes on dashing
        if (isDashing)
        {
            if (collision.gameObject.tag == "Box")
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
