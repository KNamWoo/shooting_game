using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public int enemyScore;
    public float maxBulCool;
    public float curBulCool;

    public bool OnFire;
    public Sprite[] sprites;

    public GameObject BulletObjA;
    public GameObject BulletObjB;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public GameObject player;
    Player playerLogic;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        OnFire = true;
        playerLogic = player.GetComponent<Player>();
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(playerLogic.isExtant == true && OnFire == true){
            Fire();
            Reload();
        }
    }

    public void OnHit(int dmg){
        if(health <= 0){
            return;
        }
        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f);//시간차로 실행행

        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            int ran = Random.Range(0, 10);
            if(ran < 6){//no 50%
                Debug.Log("Not Item");
            }else if(ran < 8){//coin 20%
                Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);
            }else if(ran < 9){//power 10%
                Instantiate(itemPower, transform.position, itemPower.transform.rotation);
            }else if(ran < 10){//boom 10%
                Instantiate(itemBoom, transform.position, itemBoom.transform.rotation);
            }
            Destroy(gameObject);
        }
    }

    void ReturnSprite(){
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet"){
            Destroy(gameObject);
        }else if (collision.gameObject.tag == "PlayerBullet"){
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            Destroy(collision.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BackGround"){
            OnFire = false;
        }
    }

    void Fire(){
        if (curBulCool < maxBulCool){
            return;
        }

        if(enemyName == "S"){
            GameObject bullet = Instantiate(BulletObjA, transform.position, transform.rotation);
            Rigidbody2D rbody = bullet.GetComponent<Rigidbody2D>();

            Vector3 dirVec = player.transform.position - transform.position;
            rbody.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);
        }
        if(enemyName == "L"){
            GameObject bulletL = Instantiate(BulletObjB, transform.position + Vector3.left*0.3f, transform.rotation);
            GameObject bulletR = Instantiate(BulletObjB, transform.position + Vector3.right*0.3f, transform.rotation);
            Rigidbody2D rbodyL = bulletL.GetComponent<Rigidbody2D>();
            Rigidbody2D rbodyR = bulletR.GetComponent<Rigidbody2D>();

            Vector3 dirVecL = player.transform.position - transform.position;
            Vector3 dirVecR = player.transform.position - transform.position;
            rbodyL.AddForce(dirVecL.normalized * 6, ForceMode2D.Impulse);
            rbodyR.AddForce(dirVecR.normalized * 6, ForceMode2D.Impulse);
        }

        curBulCool = 0;
    }

    void Reload(){
        curBulCool += Time.deltaTime;
    }
}
