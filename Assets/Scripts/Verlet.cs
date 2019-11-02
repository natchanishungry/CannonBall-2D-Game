using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Verlet : MonoBehaviour
{

    public List<VerletPoint> points;
    public Vector2 randomPosition = new Vector2(-5, 0);

    private LineRenderer lr;

    private float firstStep = 0.0002f;

    private static float[,] coordinates = { { 0, 0 }, { -0.5f, 2 }, { -1, 0.5f }, { -1.5f, 2 }, { -2, 1 }, { -2.5f, 2 }, { -2.5f, 2.5f }, { -2.5f, 3 }, { -1.65f, 3.5f }, { -0.8f, 3.5f },
                                    {0, 3 }, {0, 2.5f }, {0, 2 } };


    private static float[,] constraints = new float[15, 15];
  

    private PerlinNoise pn;
    private Rectangle boundBox;

    private cannon cannon;

    public GameObject dotPrefab;
    public GameObject[] dots;
    

    public Verlet(float x, float y) //x and y 's next position will not intersect with wall
    {
        randomPosition = new Vector2(x, y);
    }

    bool destroy = false;
    //public makeGhosts mg;
   
    // Start is called before the first frame update
    void Start()
    {
        points = new List<VerletPoint>();
        lr = GetComponent<LineRenderer>();

        establishConstraints();

        pn = GameObject.Find("floor").GetComponent<PerlinNoise>();
        boundBox = new Rectangle(new Vector2(pn.xMin, pn.yMin), new Vector2(pn.xMin, pn.yMax), new Vector2(pn.xMax, pn.yMax), new Vector2(pn.xMax, pn.yMin));

        cannon = GameObject.Find("cannonRectangle").GetComponent<cannon>();
        
        lr.positionCount = 13;
        dots = new GameObject[13];
        for (int i = 0; i<13; i++)
        {
            points.Add(new VerletPoint(this, i, randomPosition.x + coordinates[i,0], randomPosition.y + coordinates[i, 1], randomPosition.x + firstStep + coordinates[i, 0], randomPosition.y + firstStep + coordinates[i, 1]));
            
           lr.SetPosition(i, new Vector3(randomPosition.x + firstStep + coordinates[i, 0], randomPosition.y + firstStep + coordinates[i, 1], 0));
            dots[i] = Instantiate(dotPrefab, new Vector3(randomPosition.x + firstStep + coordinates[i, 0], randomPosition.y + firstStep + coordinates[i, 1], 0), Quaternion.identity);
        }

      


        for (int i = 0; i<13; i++)
        {
            
            for (int j = 0; j < 13; j++)
            {
                points[i].distances[j] = Vector2.Distance(new Vector2(points[i].xprev, points[i].yprev), new Vector2(points[j].xprev, points[j].yprev));
            }
        }

    }


    void establishConstraints()
    {
        for(int i = 0; i< 13; i++)
        {
            for(int j = 0; j<13; j++)
            {
                constraints[i, j] = 0;
            }
        }
        
        //tail
        constraints[0, 12] = 2f;
        constraints[0, 1] = 2.06f;
        constraints[1, 12] = 0.5f;
        constraints[2, 1] = 1.58f;
        constraints[3, 2] = 1.58f;
        constraints[3, 1] = 1;
        constraints[4, 3] = 1.12f;
        constraints[5, 4] = 1.12f;
        constraints[5, 3] = 1;

        //head
        constraints[6, 5] = 0.5f;
        constraints[7, 6] = 0.5f;
        constraints[8, 7] = 0.986f;
        constraints[9, 8] = 0.85f;
        constraints[10, 9] = 0.943f;
        constraints[11, 10] = 0.5f;
        constraints[12, 11] = 0.5f;

        //across body
        constraints[8, 12] = 2.23f;
        constraints[9, 5] = 2.27f;
        constraints[6, 11] = 2.5f;
        constraints[7, 10] = 2.5f;


    }
    private int j;
    Vector2 point = new Vector2(0, 0);
    Vector2 myPoint = new Vector2(0, 0);
    Vector2 vec = new Vector2(0, 0);
    void Update()
    {
        

        foreach (VerletPoint p in points)
        {
           p.PseudoUpdate();
        }
        

        foreach(VerletPoint p in points)
        {
            p.CheckConstraints(points);
        }

        foreach (VerletPoint p in points)
        {

            p.Fixed = false;
            // p.CheckConstraints(points);
            collisionWithCannonballs(p);
        }


        for (int i = 0; i < points.Count; i++)
        {
            ghostCollisionResolution(points[i % points.Count], points[(i + 1) % points.Count]);
        }


        j = 0;
        foreach (VerletPoint p in points)
        {
            //p.PseudoUpdate();
            //p.CheckConstraints();

            lr.SetPosition(p.myIndex, new Vector3(p.xcurr, p.ycurr, 0));
            dots[j].transform.position = new Vector3(p.xcurr, p.ycurr, 0);
            j++;
        }
        

        if (destroy)
        {
            // mg.destroy = destroy;
            //create a ghost right before i destroy gameObject
            GameObject v = Instantiate(this.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            Verlet vv = v.GetComponent<Verlet>();
            vv.randomPosition.x = -Random.value * 13 + -5;
            vv.randomPosition.y = Random.value * 3 + 0.5f;
                
            Destroy(this.gameObject);
        }
    }
    private bool isQuitting = false;
    void OnBecameInvisible()
    {
        
        if (!isQuitting)
        {
            GameObject v = Instantiate(this.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
            Verlet vv = v.GetComponent<Verlet>();
            vv.randomPosition.x = -Random.value * 13 + -5;
            vv.randomPosition.y = Random.value * 3 + 0.5f;
            foreach(GameObject dot in dots)
            {
                Destroy(dot);
            }
            Destroy(this.gameObject);
        }
        
 
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }


    void collisionWithCannonballs(VerletPoint p)
    {
        GameObject cbtoRemove = new GameObject();

        foreach (GameObject cb in cannon.cannonballs)
        {
            if ( Vector2.Distance(new Vector2(cb.transform.position.x, cb.transform.position.y), new Vector2(p.xnext, p.ynext)) <= 0.5f){
                //collision detected
                p.xnext = p.xcurr + Time.deltaTime  * cb.GetComponent<CannonBall>().vix;
                p.ynext = p.ycurr + Time.deltaTime  * cb.GetComponent<CannonBall>().viy;

                cbtoRemove = cb;
                break;
                
            }
        }

        if(cbtoRemove != null)
        {
            cannon.cannonballs.Remove(cbtoRemove);
            Destroy(cbtoRemove);


        }

        
    }

    private Vector2 normal;

    public static float accX = 0.1f;
    public static float accY = 0.1f;

   

    void ghostCollisionResolution(VerletPoint a, VerletPoint b)
    {
        
       

        if (LineSegmentsIntersect(new Vector2(a.xnext, a.ynext), new Vector2(b.xnext, b.ynext), boundBox.a, boundBox.b)) //intersect on left side
        {

            normal = new Vector2(-(boundBox.b.y - boundBox.a.y), boundBox.b.x - boundBox.a.x);
            normal.Normalize();

            if (a.xnext < b.xnext) //then point2.x must be changed
            {
               b.xnext = pn.xMin - 0.01f;

            }
            else
            {
                a.xnext = pn.xMin - 0.01f;

            }


        }if(LineSegmentsIntersect(new Vector2(a.xnext, a.ynext), new Vector2(b.xnext, b.ynext), boundBox.b, boundBox.c)) //intersect on top
        {
        

            if(a.ynext < pn.yMax && b.ynext < pn.yMax)
            {
                a.ynext = pn.yMax + 0.01f;
                b.ynext = pn.yMax + 0.01f;
            }

            else if (a.ynext < b.ynext)
            {
                a.ynext = pn.yMax + 0.01f;
            }
            else 
            {
                b.ynext = pn.yMax + 0.01f;
            }
        }
        if(LineSegmentsIntersect(new Vector2(a.xnext, a.ynext), new Vector2(b.xnext, b.ynext), boundBox.c, boundBox.d)){ //intersect on right
         

            if (a.xnext < b.xnext) 
            {
                a.xnext = pn.xMax + 0.01f;
            }
            else
            {
                b.xnext = pn.xMax + 0.01f;
            }
        }

    }


    public class VerletPoint
    {

        private Verlet ghost;
        public float xprev, xcurr, xnext;
        public float yprev, ycurr, ynext;

        

        public float[] distances = new float[15];
        public int myIndex; 
        public bool Fixed = false;

        private PerlinNoise pn = GameObject.Find("floor").GetComponent<PerlinNoise>();

        //these following vectors are used in checkConstraints() function
        private Vector2 point = new Vector2(0, 0);
        private Vector2 vec = new Vector2(0, 0);
        private Vector2 myPoint;

        //private Rectangle boundBox;

        public VerletPoint(Verlet ghost, int index, float xprev, float yprev, float xcurr, float ycurr)
        {
            this.ghost = ghost;
            this.myIndex = index;
            this.xprev = xprev;
            this.yprev = yprev;
            this.xcurr = xcurr;
            this.ycurr = ycurr;
            setNext();
          
        }

        


        // Update is called once per frame
        public void PseudoUpdate()
        {
            xprev = xcurr;
            yprev = ycurr;
            xcurr = xnext;
            ycurr = ynext;
            setNext();

            myPoint.x = xnext;
            myPoint.y = ynext;


        }

        void setNext()
        {
            xnext = 2 * xcurr - xprev + accX * Time.deltaTime * Time.deltaTime;
            ynext = 2 * ycurr - yprev + accY * Time.deltaTime * Time.deltaTime;
        }


        float magnitude = 0;
        public void CheckConstraints(List<VerletPoint> points)
        {

            

            for (int i = 0; i < 13; i++)
            {
                if (i == myIndex)
                {
                    continue;
                }

                if (constraints[myIndex, i] == 0) continue;

                point.x = ghost.points[i].xnext;
                point.y = ghost.points[i].ynext;

                vec.x = xnext - point.x;
                vec.y = ynext - point.y;

                magnitude = vec.magnitude;

                if (vec.magnitude - constraints[myIndex, i] >= 0.1f || vec.magnitude - constraints[myIndex, i] <= -0.1f)
                {
                    vec.Normalize();
                    //   xnext = vec.x * constraints[myIndex, i] / magnitude + point.x;
                    // ynext = vec.y * constraints[myIndex, i] / magnitude + point.y;

                    xnext = (xnext + vec.x * distances[i] / magnitude + point.x) / 2;
                    ynext = (ynext + vec.y * distances[i] / magnitude + point.y) / 2;

                    // xnext = distances[i] / vec.magnitude * vec.x + point.x;
                    // ynext = distances[i] / vec.magnitude * vec.y + point.y;
                }
            
            }
            if(ynext <= 0.5f)
            {
                ynext = 0.6f;
            }
   
        }

    }

    public class Rectangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 d;

        public Rectangle() { }

        public Rectangle(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            this.a = new Vector2(a.x, a.y);
            this.b = new Vector2(b.x, b.y);
            this.c = new Vector2(c.x, c.y);
            this.d = new Vector2(d.x, d.y);
        }

        public void setCorners(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            this.a = new Vector2(a.x, a.y);
            this.b = new Vector2(b.x, b.y);
            this.c = new Vector2(c.x, c.y);
            this.d = new Vector2(d.x, d.y);
        }
    }

    public bool RectangleLineIntersect(Rectangle r, Vector2 u, Vector2 v)
    {
        

        return (LineSegmentsIntersect(u, v, r.a, r.b) ||
            LineSegmentsIntersect(u, v, r.b, r.c) ||
            LineSegmentsIntersect(u, v, r.c, r.d) ||
            LineSegmentsIntersect(u, v, r.d, r.a)
            );
    }

    public bool LineSegmentsIntersect(Vector2 lineOneA, Vector2 lineOneB, Vector2 lineTwoA, Vector2 lineTwoB)
    {
        return (((lineTwoB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x) > (lineTwoA.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)) !=
            ((lineTwoB.y - lineOneB.y) * (lineTwoA.x - lineOneB.x) > (lineTwoA.y - lineOneB.y) * (lineTwoB.x - lineOneB.x)) &&
            ((lineTwoA.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x)) !=
            ((lineTwoB.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)));
    }

    

}
