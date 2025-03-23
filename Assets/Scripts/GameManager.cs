using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyObjs;
    public Transform[] spawnPoints;

    public float spawnCool;
    public float curTime;

    public GameObject player;

    void Update()
    {
        curTime += Time.deltaTime;

        if(curTime > spawnCool){
            SpawnEnemy();
            spawnCool = Random.Range(0.5f, 3f);
            curTime = 0;
        }
    }
    void SpawnEnemy(){
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = Instantiate(enemyObjs[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        Rigidbody2D rbody = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;

        if(ranPoint == 5 || ranPoint == 6){//Right
            enemy.transform.Rotate(Vector3.back*90);
            rbody.linearVelocity = new Vector2(enemyLogic.speed*(-1), -1);
        }else if(ranPoint == 7 || ranPoint == 8){//Left
            enemy.transform.Rotate(Vector3.forward*90);
            rbody.linearVelocity = new Vector2(enemyLogic.speed, -1);
        }else{ //Front
            rbody.linearVelocity = new Vector2(0, enemyLogic.speed * (-1));
        }
    }

    public void RespawnPlayer(){
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe(){
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);
    }
}
