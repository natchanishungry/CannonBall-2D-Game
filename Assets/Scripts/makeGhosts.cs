using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeGhosts : MonoBehaviour
{

    public GameObject prefab;
   

    // Start is called before the first frame update
    void Start()
    {
        //instantiate four ghosts

        prefab = GameObject.Find("ghosty");
        GameObject v = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        Verlet vv = v.GetComponent<Verlet>();
        vv.randomPosition.x = -Random.value * 13 + -5;
        vv.randomPosition.y = Random.value * 3 + 0.5f;

        v = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        vv = v.GetComponent<Verlet>();
        vv.randomPosition.x = -Random.value * 13 + -5;
        vv.randomPosition.y = Random.value * 3 + 0.5f;

        v = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        vv = v.GetComponent<Verlet>();
        vv.randomPosition.x = -Random.value * 13 + -5;
       vv.randomPosition.y = Random.value * 3 + 0.5f;

     

    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
