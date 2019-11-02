using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;

public class PerlinNoise : MonoBehaviour
{
    // Start is called before the first frame update
    private int nbPoints;
    private int[] randomGradient;
    private LineRenderer lr;
    private int width = 2;
    private int length = 5;
    public float amplitude;
    public GameObject prefab;

    [HideInInspector] public float xMin = 200;
    [HideInInspector] public float xMax = -200;
    [HideInInspector] public float yMin = 200;
    [HideInInspector] public float yMax = -200; 

    private float xOffset = 0;
   // private float xMaxOffset;
    private float yOffset = 0.1f;

    [HideInInspector] public Vector3[] positions;

    void Awake()
    {

        GameObject line1 = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        line1.name = "left";
        xOffset = -3f;
        yOffset = 1f;

        DrawRectangle(line1);
        line1.transform.SetParent(transform);
 

        GameObject line2 = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        line2.name = "right";
        xOffset = 1f;
        DrawRectangle(line2);
        line2.transform.SetParent(transform);



        GameObject line3 = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        line3.name = "top";
        xOffset = -3f;
        yOffset = 6f;
        length = 2;
        width = 7;
        DrawRectangle(line3);
        line3.transform.SetParent(transform);

        LineRenderer lr1 = line1.GetComponent<LineRenderer>();
        LineRenderer lr2 = line2.GetComponent<LineRenderer>();
        LineRenderer lr3 = line3.GetComponent<LineRenderer>();
        positions = new Vector3[lr1.positionCount + lr2.positionCount + lr3.positionCount];

        int j = 0;
        for(int i = 0; i < lr1.positionCount; i++)
        {
            positions[i + j] = lr1.GetPosition(i);
        }
        j += lr1.positionCount;
        for (int i = 0; i < lr2.positionCount; i++)
        {
            positions[i + j] = lr2.GetPosition(i);
        }
        j += lr2.positionCount;
        for (int i = 0; i < lr3.positionCount; i++)
        {
            positions[i + j] = lr3.GetPosition(i);
        }




    }


    void DrawRectangle(GameObject line)
    {
        lr = line.GetComponent<LineRenderer>();
        lr.positionCount = (length * 10) * 2 + (width * 10) * 2 + 1;
        randomGradient = new int[length+width+1];

        float rand;
         float frequency = 1;

        var points = new Vector3[ (length * 10)*2 + (width * 10)*2 +2];
        var j = 1;
        //  float y, n;

        points[0] = new Vector3(xOffset, yOffset, 1);

        float n, m;
        float xp, yp;
        float world;
       
        // float y;
        for (int i = 0; i <= length; i++) //set random unit vectors (either -1 or 1) for each integer x coordinate
        {
            rand = Random.value;
            randomGradient[i] = (rand < 0.5) ? -1 : 1;
        }
        for (float y = yOffset; y < length + yOffset; y = (float) System.Math.Round(y + 0.1f, 2)) //create left side
        {
            /*double n = noise(x * (1 / 300)) * 1.0 + //noise(x * frequency) * amplitude + ....
                 noise(x * (1 / 150)) * 0.5 +
                 noise(x * (1 / 75)) * 0.25 +
                 noise(x * (1 / 37.5)) * 0.125; //summing over multiple octaves*/

            /* m = Random.value * length;
             n = ((noise(m * frequency) * amplitude) + //noise(x * frequency) * amplitude + ....
                  (noise(m * frequency) * amplitude / 2) +
                  (noise(m * frequency) * amplitude / 4) +
                  (noise(m * frequency) * amplitude / 8)); //summing over multiple octaves*/
            yp = y - yOffset;
            n = ((noise(yp, yOffset) * amplitude) + //noise(x * frequency) * amplitude + ....
                 (noise(yp, yOffset) * amplitude / 2) +
                 (noise(yp, yOffset) * amplitude / 4) +
                 (noise(yp, yOffset) * amplitude / 8)); //summing over multiple octaves

            //use yoffset and xoffset to convert from local space to world space
            if (y < yMin) yMin = y; //calculating the min and max x and y coordinates to create our axis aligned bounding box
            if (y > yMax) yMax = y;
            if (n + xOffset > xMax) xMax = n + xOffset;
            if (n + xOffset < xMin) xMin = n + xOffset;
   


            points[j] = new Vector3(n+xOffset, y, 1);
            j++;

            

            /* for cool effect (calligraphy ish)
             * y = Random.value * 10f;
            float n = ((noise(y * frequency) * amplitude) + //noise(x * frequency) * amplitude + ....
                 (noise(y * frequency) * amplitude/2) +
                 (noise(y * frequency) * amplitude / 4) +
                 (noise(y * frequency) * amplitude / 8)); //summing over multiple octaves
                 points[j] = new Vector3(y, n, 1);

        Another cool one: same except points[j] = new Vector3(x, n, 1);
             * 
             * */

        }

        for (int i = 0; i <= width; i++) //set random unit vectors (either -1 or 1) for each integer x coordinate
        {
            rand = Random.value;
            randomGradient[i] = (rand < 0.5) ? -1 : 1;
        }
        for (float x = xOffset; x < width + xOffset; x = (float)System.Math.Round(x + 0.1f, 2)) //create top side
        {

            xp = x - xOffset;

            n = ((noise(xp, xOffset) * amplitude) + //noise(x * frequency) * amplitude + ....
                 (noise(xp, xOffset) * amplitude / 2) +
                 (noise(xp, xOffset) * amplitude / 4) +
                 (noise(xp, xOffset) * amplitude / 8)); //summing over multiple octaves

            if (length + n + yOffset < yMin) yMin = length + n + yOffset; //calculating the min and max x and y coordinates to create our axis aligned bounding box
            if (length + n + yOffset > yMax) yMax = length + n + yOffset;
            if (x > xMax) xMax = x;
            if (x < xMin) xMin = x;



            points[j] = new Vector3(x, length+n+yOffset, 1);
            j++;

        }

        for (int i = 0; i <= length; i++) //set random unit vectors (either -1 or 1) for each integer x coordinate
        {
            rand = Random.value;
            randomGradient[i] = (rand < 0.5) ? -1 : 1;
        }
        for (float y = length + yOffset; y > yOffset; y = (float)System.Math.Round(y - 0.1f, 2)) //create right side
        {


            yp = y - yOffset;

            n = ((noise(yp, yOffset) * amplitude) + //noise(x * frequency) * amplitude + ....
                 (noise(yp, yOffset) * amplitude / 2) +
                 (noise(yp, yOffset) * amplitude / 4) +
                 (noise(yp, yOffset) * amplitude / 8)); //summing over multiple octaves

            if (y < yMin) yMin = y; //calculating the min and max x and y coordinates to create our axis aligned bounding box
            if (y > yMax) yMax = y;
            if (width + n + xOffset > xMax) xMax = width + n + xOffset;
            if (width + n + xOffset < xMin) xMin = width + n + xOffset;


            points[j] = new Vector3(width+xOffset+n, y, 1);
            j++;

        }
        for (int i = 0; i <= width; i++) //set random unit vectors (either -1 or 1) for each integer x coordinate
        {
            rand = Random.value;
            randomGradient[i] = (rand < 0.5) ? -1 : 1;
        }
        for (float x = width + xOffset; x > xOffset; x = (float)System.Math.Round(x - 0.1f, 2)) //create bottom side
        {


            xp = x - xOffset;

            n = ((noise(xp, xOffset) * amplitude) + //noise(x * frequency) * amplitude + ....
                 (noise(xp, xOffset) * amplitude / 2) +
                 (noise(xp, xOffset) * amplitude / 4) +
                 (noise(xp, xOffset) * amplitude / 8)); //summing over multiple octaves

            if (n + yOffset < yMin) yMin = n + yOffset; //calculating the min and max x and y coordinates to create our axis aligned bounding box
            if (n + yOffset > yMax) yMax = n + yOffset;
            if (x > xMax) xMax = x;
            if (x < xMin) xMin = x;

            points[j] = new Vector3(x, n+yOffset, 1);
            j++;

        }
        points[j] = new Vector3(xOffset, yOffset, 1);
        lr.SetPositions(points);
    }

    float fade(float t) //FADE FUNCTION
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
   

    float noise(float x, float offset) //CREATE NOISE
    {
        float x0 = Mathf.Floor(x);
        float x1 = x0 + 1;

        int g0 = randomGradient[(int)x0]; 
        int g1 = randomGradient[(int)x1];

        float dot0 = (x-x0) * g0;
        float dot1 = (x-x1) * g1;

        float sx = fade(x - x0);

        float noise = (1 - sx) * dot0 + sx * dot1;

        return noise;

    }
}
