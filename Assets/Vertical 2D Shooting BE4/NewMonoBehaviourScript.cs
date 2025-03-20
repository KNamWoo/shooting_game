using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float speed;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    Animator anim;

    public void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
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
