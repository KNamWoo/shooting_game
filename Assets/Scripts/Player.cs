using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int life;
    public int score;

    public float speed;
    public int maxPower;
    public int power;
    public int maxBoom;
    public int boom;
    public float maxBulCool;
    public float curBulCool;

    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;
    public bool isHit;
    public bool isExtant;//플레이어 존재여부
    public bool isBoomTime;

    public GameObject BulletObjA;
    public GameObject BulletObjB;
    public GameObject boomEffect;

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
        Boom();
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

    void Boom(){
        if(!Input.GetKeyDown(KeyCode.Q)){
            return;
        }
        if(isBoomTime){
            return;
        }
        if(boom == 0){
            return;
        }
        
        boom--;
        isBoomTime = true;
        manager.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for(int index = 0; index < enemies.Length; index++){
            Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
            enemyLogic.OnHit(1000);
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for(int index = 0; index < bullets.Length; index++){
            Destroy(bullets[index]);
        }
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
            if(isHit){
                return;
            }

            isHit = true;
            isExtant = false;
            life--;
            manager.UpdateLifeIcon(life);

            if(life == 0){
                manager.GameOver();
            }else{
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                for(int index = 0; index < enemies.Length; index++){
                    Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
                    enemyLogic.curBulCool = 0f;
                }
                manager.RespawnPlayer();
            }
            gameObject.SetActive(false);
            Destroy(collision.gameObject);
        }else if(collision.gameObject.tag == "Item"){
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":{
                    score += 100;
                    break;
                }
                case "Power":{
                    if(power == maxPower){
                        score += 40;
                    }else{
                        power++;
                    }
                    break;
                }
                case "Boom":{
                    if(boom == maxBoom){
                        score += 40;
                    }else{
                        boom++;
                        manager.UpdateBoomIcon(boom);
                    }
                    break;
                }
            }
            Destroy(collision.gameObject);
        }
    }

    void OffBoomEffect(){
        boomEffect.SetActive(false);
        isBoomTime = false;
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
