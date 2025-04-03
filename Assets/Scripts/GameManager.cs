using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float spawnCool;
    public float curTime;
    public float Timefloat;
    public int TimeScore;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    Player playerLogic;

    void Start()
    {
        playerLogic = player.GetComponent<Player>();
        playerLogic.isExtant = true;
    }

    void Awake() {
        enemyObjs = new string[]{ "EnemyS", "EnemyM", "EnemyL" };
    }

    void Update()
    {
        curTime += Time.deltaTime;
        Timefloat += Time.deltaTime;
        TimeScore = Mathf.RoundToInt(Timefloat) * 10;

        if(curTime > spawnCool){
            SpawnEnemy();
            spawnCool = Random.Range(0.5f, 3f);
            curTime = 0;
        }

        scoreText.text = "S : " + string.Format("{0:n0}", playerLogic.score + TimeScore);
    }

    void SpawnEnemy(){
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = objectManager.MakeObj(enemyObjs[ranEnemy]);
        enemy.transform.position=spawnPoints[ranPoint].position;

        Rigidbody2D rbody = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.objectManager = objectManager;

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

    public void UpdateLifeIcon(int life){
        for(int index = 0; index < 3; index++){
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }

        for(int index = 0; index < life; index++){
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom){
        for(int index = 0; index < 3; index++){
            boomImage[index].color = new Color(1, 1, 1, 0);
        }

        for(int index = 0; index < boom; index++){
            boomImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void RespawnPlayer(){
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe(){
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        playerLogic.power = 1;
        playerLogic.isHit = false;
        playerLogic.isExtant = true;
    }

    public void GameOver(){
        Time.timeScale = 0f;
        gameOverSet.SetActive(true);
    }

    public void GameRetry(){
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
