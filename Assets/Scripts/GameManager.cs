using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject polyParent;
    public GameObject background;
    public Camera cam;
    public GameObject pointer;
    public float drawSpeed = 1f;
    public float moveSpeed = 10.0f;

    public float boarder = 50;
    public float polyDepth = 0f;
    public int polyCountMax = 5;

    public Canvas pauseCanvas;
    public Canvas gameCanvas;
    public Text numberOfSquaresText;
    public Text polyCountText;
    public Slider numberOfSquaresSlider;
    int polyCount;
    

    bool longPress = false;
    bool touching = false;
    bool gameRunning;

    public float longPressTime = 0.1f;
    private void Start()
    {
        pauseCanvas.enabled = false;
        gameRunning = true;
        numberOfSquaresSlider.value = polyCountMax;

        for (int i = 0; i < polyCountMax; i++)
        {
            DrawSquare();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning == true)
        {
            if(Input.touchCount == 0)
            {
                touching = false;
                longPress = false;
            }
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    StartCoroutine("LongTouch");
                    touching = true;
                    Ray();
                                
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    touching = false;
                    longPress = false;
                    UpdatePolyCount();
                }
            }

            if (Input.GetKey(KeyCode.Space))
            {
                DrawSquare();
            }

            if (Input.touchCount == 3)
            {
                Pause();
            }
        }

        if(longPress == true)
        {
            DrawSquare();
        }
        polyCountMax = Mathf.RoundToInt(numberOfSquaresSlider.value);
        numberOfSquaresText.text = polyCountMax.ToString();       
    }

    //  UI   -----------------------------------------------------
    public void Pause()
    {
        gameRunning = false;
        Time.timeScale = 0f;
        pauseCanvas.enabled = true;
        gameCanvas.enabled = false;
    }

    public void Resume()
    {
        gameRunning = true;
        Time.timeScale = 1f;
        pauseCanvas.enabled = false;
        gameCanvas.enabled = true;
    }

    IEnumerator LongTouch()
    {
        Debug.Log("corutine started"); 
        for(int i = 0; i<10; i++)
        {
            if (touching == false)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(longPressTime/10);
                if (i == 9)
                {
                    longPress = true;
                }
            }        
        }
    }

    void Ray()
    {
        Vector3 touchPoint = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0);
        Vector3 worldTouchPoint = cam.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 10));
        pointer.transform.position = worldTouchPoint;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, worldTouchPoint - cam.transform.position, out hit, Mathf.Infinity, layerMask) && hit.collider.tag == "Square")
        {
            Debug.Log("Did Hit");
            Debug.DrawRay(cam.transform.position, (worldTouchPoint - cam.transform.position) * hit.distance, Color.yellow);
            Destroy(hit.collider.gameObject);
        }
        else
        {
            Debug.DrawRay(cam.transform.position, (worldTouchPoint - cam.transform.position) * 1000, Color.white);
            Debug.Log("Did not Hit");
            DrawSquare();
        }
    }

    void DrawSquare()
    {
        if(gameRunning == true)
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.AddComponent<Move>();
            Collider col = plane.GetComponent<Collider>();
            plane.gameObject.tag = "Square";
            
            float z = 9 - Random.Range(0, polyDepth);

            Vector3 planePos = new Vector3(Random.Range(boarder, Screen.width - boarder), Random.Range(boarder, Screen.height - boarder), z);
            Vector3 fixedPlanePos = cam.ScreenToWorldPoint(planePos);

            plane.transform.localScale = new Vector3(Random.Range(0.001f, 0.2f), 1, Random.Range(0.001f, 0.2f));
            plane.transform.position = fixedPlanePos;
            plane.transform.rotation = Quaternion.Euler(0, 90, -90);
            plane.transform.parent = polyParent.transform;
            UpdatePolyCount();
        }          
    }

    void DestroySquare()
    {
        if(gameRunning == true)
        {
            Destroy(polyParent.transform.GetChild(0).gameObject);
            
        }
        UpdatePolyCount();
    }

    private void UpdatePolyCount()
    {
        polyCount = polyParent.transform.childCount;
        polyCountText.text = polyCount.ToString();
    }
}