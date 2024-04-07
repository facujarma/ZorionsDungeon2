using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class handSwordLogic : MonoBehaviour
{
    [SerializeField] GameObject eje;
    [SerializeField] float rotationSpeed;
    bool isInVuelta = false;
    bool espadaOculta = false;

    private void Start()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isInVuelta)
        {
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            direction = direction.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            eje.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            blandir_OcultarEspada();
        }

    }

    public void Vuelta()
    {
        if (espadaOculta) { blandir_OcultarEspada(); }
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        isInVuelta = true;
        StartCoroutine(Rotate360Degrees());

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInVuelta)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                Destroy(collision.gameObject);
            }
        }
    }
    IEnumerator Rotate360Degrees()
    {
        float totalRotation = 360f; 
        float currentRotation = 0f; 

        while (currentRotation < totalRotation)
        {
            float rotationIncrement = 360 * rotationSpeed * Time.deltaTime;
            currentRotation += rotationIncrement;
            eje.transform.Rotate(Vector3.forward, rotationIncrement);

            yield return null;
        }
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        isInVuelta = false;
    }

    public void blandir_OcultarEspada()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = espadaOculta;
        espadaOculta = !espadaOculta;
    }
}