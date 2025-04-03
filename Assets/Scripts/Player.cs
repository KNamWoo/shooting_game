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

    public GameManager gameManager;
    public ObjectManager objectManager;

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
                    GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                    bullet.transform.position = transform.position;
                    
                    Rigidbody2D rbody = bullet.GetComponent<Rigidbody2D>();
                    rbody.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    break;
                }
                case 2:{
                    GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                    bulletR.transform.position=transform.position + Vector3.right*0.1f;

                    GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                    bulletL.transform.position=transform.position+Vector3.left*0.1f;

                    Rigidbody2D rbodyR = bulletR.GetComponent<Rigidbody2D>();
                    Rigidbody2D rbodyL = bulletL.GetComponent<Rigidbody2D>();
                    rbodyR.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    rbodyL.AddForce(Vector2.up*10, ForceMode2D.Impulse);
                    break;
                }
                case 3:{
                    GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                    bulletRR.transform.position = transform.position+Vector3.right*0.25f;

                    GameObject bulletSS = objectManager.MakeObj("BulletPlayerB");
                    bulletSS.transform.position = transform.position;

                    GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                    bulletLL.transform.position=transform.position+Vector3.left*0.25f;

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
        gameManager.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");
        for(int index = 0; index < enemiesL.Length; index++){
            if(enemiesL[index].activeSelf) {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for(int index = 0;index<enemiesM.Length;index++) {
            if(enemiesM[index].activeSelf) {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for(int index = 0;index<enemiesS.Length;index++) {
            if(enemiesS[index].activeSelf) {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        for(int index = 0;index<bulletsA.Length;index++) {
            if(bulletsA[index].activeSelf) {
                bulletsA[index].SetActive(false);
            }
        }
        for(int index = 0;index<bulletsB.Length;index++) {
            if(bulletsB[index].activeSelf) {
                bulletsB[index].SetActive(false);
            }
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
            gameManager.UpdateLifeIcon(life);

            if(life == 0){
                gameManager.GameOver();
            }else{
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                for(int index = 0; index < enemies.Length; index++){
                    Enemy enemyLogic = enemies[index].GetComponent<Enemy>();
                    enemyLogic.curBulCool = 0f;
                }
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
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
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
                }
            }
            collision.gameObject.SetActive(false);
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
