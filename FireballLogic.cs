using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireballLogic : MonoBehaviour
{
    Vector2 dir = Vector2.zero;
    [SerializeField] int speed;
    Rigidbody2D rb;

    string origin;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        rb.velocity = dir;
    }

    public void goTo( Vector2 target, string a)
    {
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        dir = target.normalized * speed;
        origin = a;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != origin && other.gameObject.name != "PlayerSword")
        {
            if(other.gameObject.tag == "Enemy" )
            {
                Destroy(other.gameObject);
            }
            Destroy(gameObject);
        }

    }
}
