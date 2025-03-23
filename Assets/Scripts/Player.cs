using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float power;
    public float maxBulCool;
    public float curBulCool;

    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public GameObject BulletObjA;
    public GameObject BulletObjB;

    public GameManager manager;

    Animator anim;

    public void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        Move();
        Fire();
        Reload();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
        {
            h = 0;
        }
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
        {
            v = 0;
        }
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal")||Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire(){
        if (Input.GetKey(KeyCode.Space)){
            if (curBulCool < maxBulCool){
                return;
            }

            switch (power)
            {
                case 1:{
                    GameObject bullet = Instantiate(BulletObjA, transform.position, transform.rotation);
                    Rigidbody2D rbody = bullet.GetComponent<Rigidbody2D>();
                    rbody.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    break;
                }
                case 2:{
                    GameObject bulletR = Instantiate(BulletObjA, transform.position + Vector3.right*0.1f, transform.rotation);
                    GameObject bulletL = Instantiate(BulletObjA, transform.position + Vector3.left*0.1f, transform.rotation);
                    Rigidbody2D rbodyR = bulletR.GetComponent<Rigidbody2D>();
                    Rigidbody2D rbodyL = bulletL.GetComponent<Rigidbody2D>();
                    rbodyR.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    rbodyL.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    break;
                }
                case 3:{
                    GameObject bulletRR = Instantiate(BulletObjA, transform.position + Vector3.right*0.25f, transform.rotation);
                    GameObject bulletSS = Instantiate(BulletObjB, transform.position, transform.rotation);
                    GameObject bulletLL = Instantiate(BulletObjA, transform.position + Vector3.left*0.25f, transform.rotation);
                    Rigidbody2D rbodyRR = bulletRR.GetComponent<Rigidbody2D>();
                    Rigidbody2D rbodySS = bulletSS.GetComponent<Rigidbody2D>();
                    Rigidbody2D rbodyLL = bulletLL.GetComponent<Rigidbody2D>();
                    rbodyRR.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    rbodySS.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    rbodyLL.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    break;
                }
            }

            curBulCool = 0;
        }
    }

    void Reload(){
        curBulCool += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border"){
            switch (collision.gameObject.name)
            {
                case "Top_Border":
                    isTouchTop = true;
                    break;
                case "Bottom_Border":
                    isTouchBottom = true;
                    break;
                case "Left_Border":
                    isTouchLeft = true;
                    break;
                case "Right_Border":
                    isTouchRight = true;
                    break;
            }
        }else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet"){
            manager.RespawnPlayer();
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border"){
            switch (collision.gameObject.name)
            {
                case "Top_Border":
                    isTouchTop = false;
                    break;
                case "Bottom_Border":
                    isTouchBottom = false;
                    break;
                case "Left_Border":
                    isTouchLeft = false;
                    break;
                case "Right_Border":
                    isTouchRight = false;
                    break;
            }
        }
    }
}
