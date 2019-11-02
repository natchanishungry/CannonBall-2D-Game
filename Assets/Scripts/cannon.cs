using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cannon : MonoBehaviour
{

    public float speed; //speed of rotation
    public float tiltAngle;
    public GameObject cannoball;

    private float y;
    private float rotateBy;

    public List<GameObject> cannonballs;
   
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow)) //rotate cannon
        {

            rotateBy = tiltAngle * -Input.GetAxis("Vertical");
            Quaternion target = Quaternion.Euler(0, 0, rotateBy);
            Quaternion newAngle = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
          

            if (newAngle.eulerAngles.z >= 0f && newAngle.eulerAngles.z <= 90f)
            {
                transform.rotation = newAngle;
            }

        }

        else if (Input.GetButtonDown("Fire1")) //when space bar is pressed
        {
            GameObject ball = Instantiate(cannoball, new Vector3(0, 0, 0), Quaternion.identity);
            cannonballs.Add(ball);

        }


    }


}
