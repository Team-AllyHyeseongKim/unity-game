using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerMover : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public string GUN_IP;

    Health health;
    Text leftHealth;
    Vector3 origin;


    // Start is called before the first frame update
    void Start()
    {
        health = gameObject.GetComponent<Health>();
        health.onDie += this.kill;
        health.onDamaged += this.damage;

        if(gameObject.name == "player1")
            leftHealth = GameObject.Find("player1Health").GetComponent<Text>();
        else if (gameObject.name == "player2")
            leftHealth = GameObject.Find("player2Health").GetComponent<Text>();

        leftHealth.text = gameObject.name + "  " + health.currentHealth + "Left";

        origin = transform.position;

    }

    void damage(float amount, GameObject src)
    {
         leftHealth.text = gameObject.name + "  " + health.currentHealth + "Left";

    }
   public void fire()
    {
        Vector3 forward = gameObject.transform.forward;
        Vector3 position = gameObject.transform.position + forward; // 1칸 앞에서 총알이 날라가게 포워드 더해준거임(단위벡터)

        Instantiate(ProjectilePrefab, position, Quaternion.LookRotation(-forward));
    }

    public void move(float[] rt)
    {
        Matrix4x4 mat = new Matrix4x4();
        for(int i=0; i<4; i++)//row
        {
            for(int j=0; j<4; j++)//col
            {
                mat[i, j] = rt[i*4 + j];
            }
        }



        Debug.Log(mat.ToString());
        //mat is Tcw

        /*
         * 
         cv::Mat Rwc = Tcw.rowRange(0,3).colRange(0,3).t(); // Rotation information
         cv::Mat twc = -Rwc*Tcw.rowRange(0,3).col(3); 
         */
        
        
        //요소들은 float
        Vector3 R = mat.transpose.rotation.eulerAngles;
        R.y = -R.y;
        transform.rotation = Quaternion.Euler(R);

        transform.position = new Vector3(rt[3], rt[7], rt[11]) * 20 + origin;



    }


    void kill()
    {
        Debug.Log("사라진다");
        Destroy(gameObject);
    }




    // Update is called once per frame
    void Update()
    {
        
        Vector3 forward = gameObject.transform.forward;
        Vector3 position = gameObject.transform.position + forward; // 1칸 앞에서 총알이 날라가게 포워드 더해준거임(단위벡터)


        if (Input.GetMouseButtonDown(0) && gameObject.name == "player1")
        {
            Instantiate(ProjectilePrefab, position, Quaternion.LookRotation(-forward));
        }

        else if (Input.GetMouseButtonDown(1) && gameObject.name == "player2")
        {
            Instantiate(ProjectilePrefab, position, Quaternion.LookRotation(-forward));
        }






        if (gameObject.name == "player1")
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.Translate(1, 0, 0, Space.Self);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)){
                transform.Translate(-1, 0, 0, Space.Self);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.Translate(0, 0, 1, Space.Self);   
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                transform.Translate(0, 0, -1, Space.Self);
            }

        }
            /*
        else if(gameObject.name == "player2")
        {
            if (Input.GetKeyDown(KeyCode.D))
            {

            }
            else if (Input.GetKeyDown(KeyCode.A))
            {

            }
            else if (Input.GetKeyDown(KeyCode.W))
            {

            }
            else if (Input.GetKeyDown(KeyCode.S))
            {

            }
        }
            */
    
    }
}
