using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public float maxBulCool;
    public float curBulCool;
    public Sprite[] sprites;

    public GameObject BulletObjA;
    public GameObject BulletObjB;
    public GameObject player;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Fire();
        Reload();
    }

    void OnHit(int dmg){
        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke("ReturnSprite", 0.1f);//시간차로 실행행

        if (health <= 0)
        {
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

    void Fire(){
        if (curBulCool < maxBulCool){
            return;
        }

        if(enemyName == "S"){
            GameObject bullet = Instantiate(BulletObjA, transform.position, transform.rotation);
            Rigidbody2D rbody = bullet.GetComponent<Rigidbody2D>();

            Vector3 dirVec = player.transform.position - transform.position;
            rbody.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        if(enemyName == "L"){
            GameObject bulletL = Instantiate(BulletObjB, transform.position + Vector3.left*0.3f, transform.rotation);
            GameObject bulletR = Instantiate(BulletObjB, transform.position + Vector3.right*0.3f, transform.rotation);
            Rigidbody2D rbodyL = bulletL.GetComponent<Rigidbody2D>();
            Rigidbody2D rbodyR = bulletR.GetComponent<Rigidbody2D>();

            Vector3 dirVecL = player.transform.position - transform.position;
            Vector3 dirVecR = player.transform.position - transform.position;
            rbodyL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
            rbodyR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
        }

        curBulCool = 0;
    }

    void Reload(){
        curBulCool += Time.deltaTime;
    }
}
