using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud : MonoBehaviour
{
    // Start is called before the first frame update
    private float clock;
    private float windMagn;
    private float screenWidth;
    private float screenHeight;

    private Vector3 pos;

    public Camera camera;

    public float windRange; 

    void Start()
    {
        ResetWind();

        screenHeight = camera.orthographicSize;
        screenWidth = screenHeight * camera.aspect;

    }

    public float getWind()
    {
        return windMagn;
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;

        if(clock > 2f)
        {
            ResetWind();
        }

        pos = new Vector3(transform.position.x + windMagn * Time.deltaTime, transform.position.y, 1);

        if(pos.x >= -screenWidth && pos.x + 2.25f <= screenWidth) //if the cloud is within the bounds of the screen
                                                                  //i dont get why it doesnt work with pos.x- 2.25f?????????????????????????????
        {
            transform.position = pos;
        }
        else
        {
            windMagn *= -1; //otherwise the wind should change direction to keep the cloud within
        }
       
    }

    void ResetWind()
    {
        clock = 0f;

        windMagn = Random.value * windRange - windRange / 2;

    }
}
