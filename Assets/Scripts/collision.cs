using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    private PerlinNoise pn;
    private CannonBall cb;
    private cloud cloud;

  
    public float radius;

    private Rectangle boundBox;
    private Rectangle cBallHitBox;
    private Circle cBall;
    private Circle cBallNext;

   
    private Vector2 p1 = new Vector2(0, 0);
    private Vector2 p2 = new Vector2(0, 0);

    private bool insideBoundBox = false;

    class Circle
    {
      
        public Vector2 center;

        public Circle() { }

        public Circle(Vector2 center)
        {
            this.center = new Vector2(center.x, center.y);
        }

        public void setCenter(Vector2 center)
        {
            this.center = new Vector2(center.x, center.y);
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
            this.a = new Vector2(a.x , a.y);
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

    // Start is called before the first frame update
    void Start()
    {
        radius = 0.5f;
        cloud = GameObject.Find("cloud").GetComponent<cloud>();
        pn = GameObject.Find("floor").GetComponent<PerlinNoise>();
       
        cb = this.GetComponent<CannonBall>();

        xMin = pn.xMin - 1f;
        xMax = pn.xMax + 1f;
        yMin = pn.yMin - 1f;
        yMax = pn.yMax + 1f;

        //bound box is a big axis aligned bounding box around the stonehedge
        boundBox = new Rectangle(new Vector2(xMin, yMin), new Vector2(xMin, yMax), new Vector2(xMax, yMax), new Vector2(xMax, yMin));

        //cBall is an object representing the cannonball
        cBall = new Circle(new Vector2(transform.position.x, transform.position.y));
        cBallNext = new Circle();
        cBallHitBox = new Rectangle();

        

    }

    
    // Update is called once per frame
    void Update()
    {
        cBall.center.x = transform.position.x;
        cBall.center.y = transform.position.y;

        UpdateCbNextAndHitbox();

        if (insideBoundBox)
        {
            Debug.Log("inside!");
            for(int i = 0; i<pn.positions.Length; i++) //pn.positions holds all the points of the stonehedge
            {

                p1.x = pn.positions[i % pn.positions.Length].x;
                p1.y = pn.positions[i % pn.positions.Length].y;

                p2.x = pn.positions[(i + 1) % pn.positions.Length].x;
                p2.y = pn.positions[(i + 1) % pn.positions.Length].y;

                if (CircleLineIntersect(cBall, p1, p2) || //could maybe just do circleLineIntersect
                   // CircleLineIntersect(cBallNext, p1, p2) ||
                    RectangleLineIntersect(cBallHitBox, p1, p2))
                {
                    HandleCollision(pn.positions[i % pn.positions.Length], pn.positions[(i + 1) % pn.positions.Length]);
                   break;
                }
                    
            }

        }
        //check if ball is now inside the bound box
        
        insideBoundBox = CircleRectangleIntersect(cBall, boundBox) ||
                        CircleRectangleIntersect(cBallNext, boundBox) ||
            RectangleRectangleIntersect(cBallHitBox, boundBox);


    }

    private Vector3 reflected;
    private Vector3 velocity = new Vector3 (0, 0, 0);
    private Vector3 normal = new Vector3(0, 0, 0);

    void HandleCollision(Vector3 a, Vector3 b)
    {
        Debug.Log("COLLISION DETECTED");


        velocity.x = cb.vix;
        velocity.y = cb.viy;

        normal.x = b.y-a.y;
        normal.y = -(b.x - a.x);

        reflected = Vector3.Reflect(velocity, normal);


        cb.vix = -reflected.x*3/4;
        cb.viy = -reflected.y*3/4;

        cb.MoveCannonball();

    }


    

    private float xNext, yNext, wind;
    private float viyNext, vixNext;

    /*REMINDER: (declared above)
     * 
     * private Rectangle cBallHitBox;
    private Circle cBall;
    private Circle cBallNext;*/

    void UpdateCbNextAndHitbox()
    {
        yNext = cBall.center.y * cb.damp + cb.viy * Time.deltaTime;
        if (cBall.center.y > 8.3f) wind = cloud.getWind(); //when the cannonball is above the stonehedge
        else wind = 0;
       // viyNext = cb.viy - cb.gravity * Time.deltaTime; //i dont think i need this
        xNext = cBall.center.x * cb.damp + cb.vix * Time.deltaTime + wind * Time.deltaTime;

        //update cBallNext
        cBallNext.setCenter(new Vector2(xNext, yNext));

        Vector2 centerToNext = new Vector2(cBallNext.center.x - cBall.center.x, cBallNext.center.y - cBall.center.y);
        Vector2 norm = new Vector2(centerToNext.y, -centerToNext.x); //centerToNext and norm are perpendicular, aka centerToNext dot norm = 0

        norm.Normalize();

        //four corners of rectangle hit box
        Vector2 a = new Vector2(cBall.center.x - radius * norm.x, cBall.center.y - radius * norm.y);
        Vector2 b = new Vector2(cBall.center.x + radius * norm.x, cBall.center.y + radius * norm.y);   
        Vector2 c = new Vector2(cBallNext.center.x + radius * norm.x, cBallNext.center.y + radius * norm.y);
        Vector2 d = new Vector2(cBallNext.center.x - radius * norm.x, cBallNext.center.y - radius * norm.y);

        //update hitbox
        cBallHitBox.setCorners(a, b, c, d);

    }


    bool CenterInRectangle(Vector2 center, Rectangle r)
    {
        Vector2 ap = new Vector2(center.x - r.a.x, center.y - r.a.y);
        Vector2 ab = new Vector2(r.b.x - r.a.x, r.b.y - r.a.y);
        Vector2 ad = new Vector2(r.d.x - r.a.x, r.d.y - r.a.y);

        if ( (0 <= dot(ap, ab) && dot(ap, ab) <= dot(ab, ab)) && (0 <= dot(ap, ad) && dot(ap, ad) <= dot(ad, ad)) ) return true;
        else return false;
    }

    bool CircleLineIntersect(Circle c, Vector2 a, Vector2 b) //b - a is the line we're checking intersection against
    {
        Vector2 u = new Vector2(b.x - a.x, b.y - a.y);
        Vector2 v = new Vector2(c.center.x - a.x, c.center.y - a.y);
        //w is the projection of v onto u
        float d = dot(u, v);
        Vector2 w = new Vector2(-v.x + d * u.x, -v.y + d * u.y);
        if (w.magnitude <= radius) return true;
        else return false;
    }

    bool CircleRectangleIntersect(Circle c, Rectangle r)
    {
        return (CenterInRectangle(c.center, r) ||
               CircleLineIntersect(c, r.a, r.b) ||
               CircleLineIntersect(c, r.b, r.c) ||
               CircleLineIntersect(c, r.c, r.d) ||
               CircleLineIntersect(c, r.d, r.a));
    }


    bool LineSegmentsIntersect(Vector2 lineOneA, Vector2 lineOneB, Vector2 lineTwoA, Vector2 lineTwoB)
    {
        return (((lineTwoB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x) > (lineTwoA.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)) !=
            ((lineTwoB.y - lineOneB.y) * (lineTwoA.x - lineOneB.x) > (lineTwoA.y - lineOneB.y) * (lineTwoB.x - lineOneB.x)) &&
            ((lineTwoA.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x)) !=
            ((lineTwoB.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)));
    }

    bool RectangleLineIntersect(Rectangle r, Vector2 u, Vector2 v)
    {
        return (LineSegmentsIntersect(u, v, r.a, r.b) ||
            LineSegmentsIntersect(u, v, r.b, r.c) ||
            LineSegmentsIntersect(u, v, r.c, r.d) ||
            LineSegmentsIntersect(u, v, r.d, r.a)
            );
    }

    bool RectangleRectangleIntersect(Rectangle r1, Rectangle r2)
    {
        return (LineSegmentsIntersect(r1.a, r1.b, r2.a, r2.b) ||
            LineSegmentsIntersect(r1.a, r1.b, r2.b, r2.c) ||
            LineSegmentsIntersect(r1.a, r1.b, r2.c, r2.d) ||
            LineSegmentsIntersect(r1.a, r1.b, r2.d, r2.a) ||

            LineSegmentsIntersect(r1.b, r1.c, r2.a, r2.b) ||
            LineSegmentsIntersect(r1.b, r1.c, r2.b, r2.c) ||
            LineSegmentsIntersect(r1.b, r1.c, r2.c, r2.d) ||
            LineSegmentsIntersect(r1.b, r1.c, r2.d, r2.a) ||

            LineSegmentsIntersect(r1.c, r1.d, r2.a, r2.b) ||
            LineSegmentsIntersect(r1.c, r1.d, r2.b, r2.c) ||
            LineSegmentsIntersect(r1.c, r1.d, r2.c, r2.d) ||
            LineSegmentsIntersect(r1.c, r1.d, r2.d, r2.a) ||

            LineSegmentsIntersect(r1.d, r1.a, r2.a, r2.b) ||
            LineSegmentsIntersect(r1.d, r1.a, r2.b, r2.c) ||
            LineSegmentsIntersect(r1.d, r1.a, r2.c, r2.d) ||
            LineSegmentsIntersect(r1.d, r1.a, r2.d, r2.a) 
            );
    }

    float dot(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y + b.y;
    }
}
