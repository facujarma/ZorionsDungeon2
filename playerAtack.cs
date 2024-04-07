using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerAtack : MonoBehaviour
{
    [SerializeField] GameObject swordPrefab, swordHand;
    [SerializeField] float shootCooldownTime, handCooldownTime;
    private float shootCooldownTimer = 0f, handCooldownTimer = 0f;


    handSwordLogic swordLogic;
    bool inAVuelta;
    void Start()
    {
        swordLogic = swordHand.GetComponent<handSwordLogic>();
        shootCooldownTimer = Time.time;
        handCooldownTimer = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
            
            if(Time.time - shootCooldownTimer >= shootCooldownTime && !inAVuelta)
            {
                shootCooldownTimer = Time.time;
                GameObject CurrentSword = Instantiate(swordPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                CurrentSword.GetComponent<FireballLogic>().goTo(direction, "Player");
            }
            
        }
        if (Input.GetMouseButtonDown(1) && Time.time - handCooldownTimer >= handCooldownTime)
        {
            handCooldownTimer = Time.time;

            //Vuelta
            inAVuelta = true;
            swordLogic.Vuelta();
            inAVuelta = false;
        }
    }
}

