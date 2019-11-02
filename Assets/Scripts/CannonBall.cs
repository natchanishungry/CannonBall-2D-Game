using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float gravity;
    public float launchSpeed;
    
    public float damp; //dampening factor
    [HideInInspector] public float vix; //initial
    [HideInInspector] public float viy;

    private float x;
    private float y;

    private float theta;

    private float wind;

    private cloud cloud;

    public Camera camera;
    private float screenWidth;
    private float screenHeight;

    private float timeIdle = 0;

    public cannon cannon;


    void Start()
    {

        screenHeight = camera.orthographicSize;
        screenWidth = screenHeight * camera.aspect;

        GameObject tip = GameObject.Find("tipOfCannon");
        transform.position = tip.transform.position;
        
        theta = tip.transform.parent.transform.eulerAngles.z;
        vix = -launchSpeed * Mathf.Sin(theta * (Mathf.PI / 180)); //sin and cos reversed for Initial x and Initial y velocity because theta = 0 along y axis
                                                                  //add negative sign because vix is in left direction (-x direction)
        viy = launchSpeed * Mathf.Cos(theta * (Mathf.PI / 180));

        x = transform.position.x;
        y = transform.position.y;

        cloud = GameObject.Find("cloud").GetComponent<cloud>();

        cannon = GameObject.Find("cannonRectangle").GetComponent<cannon>();

    }

    // Update is called once per frame
    void Update()
    {

        MoveCannonball();

        if (vix <= 0.0001f && viy <= 0.0001f)
        {
            timeIdle += Time.deltaTime;
        }
        else timeIdle = 0;

        if (timeIdle > 3.0f)
        {
            cannon.cannonballs.Remove(this.gameObject);
            Destroy(this.gameObject); //destroy game object if still for longer than 3 seconds
        }

    }

    public void MoveCannonball()
    {
        y = y * damp + viy * Time.deltaTime;

        if (y > 8.3f) wind = cloud.getWind(); //when the cannonball is above the stonehedge
        else wind = 0;

        viy -= gravity * Time.deltaTime;
        x = x * damp + vix * Time.deltaTime + wind * Time.deltaTime;

        transform.position = new Vector3(x, y, 1);

        
    }

    void OnBecameInvisible() //if cannonball goes out of bounds
    {
        cannon.cannonballs.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
