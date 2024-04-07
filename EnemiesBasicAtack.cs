using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesBasicAtack : MonoBehaviour
{
    public float cooldownTime = 1f; 
    private float cooldownTimer = 0f;

    [SerializeField] GameObject player;
    [SerializeField] int atackDistance;
    [SerializeField] GameObject fireball;
    void Start()
    {
        cooldownTimer = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector2 moveDirection = targetPosition - (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, atackDistance);

        if (hit.collider != null && hit.collider.gameObject.name == "Player" && Time.time - cooldownTimer >= cooldownTime) {
            cooldownTimer = Time.time;

            GameObject CurrentFireball =  Instantiate(fireball, new Vector3(transform.position.x,transform.position.y,transform.position.z), Quaternion.identity);
            CurrentFireball.GetComponent<FireballLogic>().goTo(moveDirection, "Enemy");
        }
    }
}
