using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesLogic : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float speed = 3f, distanceOfView;

    Rigidbody2D rb;
    PlayerMovement playerMovement;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        
    }
    bool[,] playerPossibleMoves;
    void Update()
    {
        playerPossibleMoves = playerMovement.playerPossibleMoves;

        Vector2 direction = player.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceOfView);

        if (hit.collider != null && hit.collider.gameObject.name == "Player")
        {
            rb.velocity = direction.normalized * speed;
        }
        else if (hit.collider != null && hit.collider.gameObject.name != "Player")
        {
            GoToPlayerPossibleMove();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void GoToPlayerPossibleMove()
    {
        Vector2Int roundedPosition = Vector2Int.RoundToInt(transform.position);
        int posX = roundedPosition.x;
        int posY = roundedPosition.y;

        bool going = false;
        for (int y = posY - 5; y <= posY + 5; y++)
        {
            for (int x = posX - 5; x <= posX + 5; x++)
            {
           
                if (y >= 0 && y < playerPossibleMoves.GetLength(0) && x >= 0 && x < playerPossibleMoves.GetLength(1) && going == false)
                {
                    
                    if (playerPossibleMoves[y, x])
                    {
                        Vector2 targetPosition = new Vector2(x, y);
                        Vector2 moveDirection = targetPosition - (Vector2)transform.position;
                                
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, distanceOfView);

                        if (hit.collider == null)
                        {
                            going = true;
                            rb.velocity = moveDirection.normalized * speed;
                        }
                    }
                }
            }
        }
    }

}
