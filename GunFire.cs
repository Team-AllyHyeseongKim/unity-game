using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : MonoBehaviour
{
    float timeSpan;
    float checkTime;

    public void shoot()
    {
        //포워드 반대쪽이 뾰족한 부분이라서 -곱했어요.
        //뾰족한 부분으로 날라가요.
        GetComponent<Rigidbody>().AddForce(transform.forward * -900);
    }

    // Start is called before the first frame update
    void Start()
    {
        timeSpan = 0.0f;
        checkTime = 5.0f;       
        shoot();
    }

    void OnCollisionEnter(Collision other)
    {
        //Debug.Log(other.gameObject.tag);
       
        if (other.gameObject.tag == "projectile")
        {

        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
            Destroy(gameObject);
            if (other.gameObject.tag == "wall")
            {
                Debug.Log("벽");               
            }

            if (other.gameObject.tag == "Player")
            {
                Debug.Log("사람");
                Health health = other.gameObject.GetComponent<Health>();
                health.TakeDamage(gameObject);


            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        timeSpan += Time.deltaTime;
        if(timeSpan > checkTime)
        {
            Destroy(gameObject);
        }
    }



}
