using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
/// <summary>
/// Happy Glass 
/// Credit: Satyam Parkhi
/// Email: satyamparkhi@gmail.com
/// Facebook : https://www.facebook.com/satyamparkhi
/// Instagram : https://www.instagram.com/satyamparkhi/
/// Whatsapp : +91 7050225661
/// </summary>
public class GameManager : MonoBehaviour
{

    [Tooltip("The color of the drawn lines")]
    public Color lineColor;
    public Material lineMaterial;
    public Transform Pencil;
    public Sprite SurpriseGlass;
    public Sprite HappyGlass;
    public Sprite SadGlass;
    public Slider PenCapacity;
    public Text PenPercent;
    [HideInInspector]
    public GameObject[] Hint;

    public Image Star1;
    public Image Star2;
    public Image Star3;
    public GameObject LevComp;
    private GameObject[] Obs;
    private List<GameObject> listLine = new List<GameObject>();
    private List<Vector2> listPoint = new List<Vector2>();
    private GameObject currentLine;
    private GameObject currentColliderObject;
    private GameObject hintTemp;
    private GameObject[] waterTap;
    private GameObject Glass;
    private Vector3 LastMosPos;
    private BoxCollider2D currentBoxCollider2D;
    private LineRenderer currentLineRenderer;
    private bool stopHolding;
    private bool allowDrawing = true;
    [HideInInspector]
    public bool completed;
    int clickCont;
    private List<Rigidbody2D> listObstacleNonKinematic = new List<Rigidbody2D>();
    private GameObject[] obstacles;

    void Start()
    {
        Pencil.gameObject.SetActive(false);
        waterTap = GameObject.FindGameObjectsWithTag("Interactive");
        Glass = GameObject.FindGameObjectWithTag("GlassParent");
        Glass.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Hint = GameObject.FindGameObjectsWithTag("Hint");
        for (int i = 0; i < Hint.Length; i++)
        {
            Hint[i].SetActive(false);
        }
        lineMaterial.SetColor("_Color", lineColor);
        Obs = GameObject.FindGameObjectsWithTag("Obstacle");
        for (int i = 0; i < Obs.Length; i++)
        {
            Obs[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

    void Update()
    {
        if (PenCapacity.value <= 0.01f || !Input.GetMouseButton(0))
        {
            Pencil.gameObject.SetActive(false);
        }
       
        if (Input.GetMouseButtonDown(0))
        {
            //Get the button on click
            GameObject thisButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (thisButton != null)//Is click on button
            {
                allowDrawing = false;
            }
            else //Not click on button
            {
                allowDrawing = true;

                stopHolding = false;
                listPoint.Clear();
                CreateLine(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButton(0) && !stopHolding && allowDrawing && PenCapacity.value > 0)
        {
            if (LastMosPos != Camera.main.ScreenToWorldPoint(Input.mousePosition))
            {
                Pencil.position = new Vector3(LastMosPos.x, LastMosPos.y, 0);
                Pencil.gameObject.SetActive(true);
                Pencil.GetComponent<TrignometricRotation>().enabled = true;
                PenCapacity.value = PenCapacity.value - .003f;
                PenPercent.text = Mathf.FloorToInt(PenCapacity.value * 100).ToString()+ " %";
                if (Mathf.FloorToInt(PenCapacity.value * 100) < 75)
                {
                    Star3.gameObject.SetActive(false);
                }
                if (Mathf.FloorToInt(PenCapacity.value * 100) < 50)
                {
                    Star2.gameObject.SetActive(false);
                }
                if (Mathf.FloorToInt(PenCapacity.value * 100) < 25)
                {
                    Star1.gameObject.SetActive(false);
                }
            }
            else
            {
                Pencil.GetComponent<TrignometricRotation>().enabled = false;
            }
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!listPoint.Contains(mousePos))
            {
                //Add mouse pos, set vertex and position for line renderer
                listPoint.Add(mousePos);
                currentLineRenderer.positionCount = listPoint.Count;
                currentLineRenderer.SetPosition(listPoint.Count - 1, listPoint[listPoint.Count - 1]);

                //Create collider
                if (listPoint.Count >= 2)
                {
                    Vector2 point_1 = listPoint[listPoint.Count - 2];
                    Vector2 point_2 = listPoint[listPoint.Count - 1];

                    currentColliderObject = new GameObject("Collider");
                    currentColliderObject.transform.position = (point_1 + point_2) / 2;
                    currentColliderObject.transform.right = (point_2 - point_1).normalized;
                    currentColliderObject.transform.SetParent(currentLine.transform);

                    currentBoxCollider2D = currentColliderObject.AddComponent<BoxCollider2D>();
                    currentBoxCollider2D.size = new Vector3((point_2 - point_1).magnitude, 0.1f, 0.1f);
                    currentBoxCollider2D.enabled = false;

                    Vector2 rayDirection = currentColliderObject.transform.TransformDirection(Vector2.right);

                    Vector2 pointDir = currentColliderObject.transform.TransformDirection(Vector2.up);

                    Vector2 rayPoint_1 = (Vector2)currentColliderObject.transform.position + (-rayDirection) * (currentBoxCollider2D.size.x);

                    Vector2 rayPoint_2 = ((Vector2)currentColliderObject.transform.position + pointDir * (currentBoxCollider2D.size.y / 2f))
                                         + ((-rayDirection) * (currentBoxCollider2D.size.x));

                    Vector2 rayPoint_3 = ((Vector2)currentColliderObject.transform.position + (-pointDir) * (currentBoxCollider2D.size.y / 2f))
                                         + ((-rayDirection) * (currentBoxCollider2D.size.x));

                    float rayLength = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - rayPoint_1).magnitude;

                    RaycastHit2D hit_1 = Physics2D.Raycast(rayPoint_1, rayDirection, rayLength);
                    RaycastHit2D hit_2 = Physics2D.Raycast(rayPoint_2, rayDirection, rayLength);
                    RaycastHit2D hit_3 = Physics2D.Raycast(rayPoint_3, rayDirection, rayLength);

                    if (hit_1.collider != null || hit_2.collider != null || hit_3.collider != null)
                    {
                        GameObject hit = (hit_1.collider != null) ? (hit_1.collider.gameObject) :
                                        ((hit_2.collider != null) ? (hit_2.collider.gameObject) : (hit_3.collider.gameObject));
                        if (currentColliderObject.transform.parent != hit.transform.parent)
                        {
                            Destroy(currentBoxCollider2D.gameObject);
                            currentLineRenderer.positionCount = (listPoint.Count - 1);
                            listPoint.Remove(listPoint[listPoint.Count - 1]);
                            listLine.Add(currentLine);
                            currentLine.AddComponent<Rigidbody2D>().useAutoMass = true;
                            float m = currentLine.GetComponent<Rigidbody2D>().mass * 500;
                            currentLine.GetComponent<Rigidbody2D>().useAutoMass = false;
                            currentLine.GetComponent<Rigidbody2D>().mass = m;
                            foreach (Rigidbody2D rigid in listObstacleNonKinematic)
                            {
                                rigid.isKinematic = false;
                            }
                            if (clickCont == 0)
                            {
                                Glass.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                                for (int i = 0; i < Obs.Length; i++)
                                {
                                    Obs[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                                }
                                for (int i = 0; i < waterTap.Length; i++)
                                {
                                    waterTap[i].GetComponent<ParticleGenerator>().enabled = true;
                                }

                                clickCont++;
                            }
                            Pencil.gameObject.SetActive(false);
                            for (int i = 0; i < currentLine.transform.childCount; i++)
                            {
                                currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().size = new Vector2(currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().size.x, 0.05f);
                                currentLine.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                                currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                            }
                            stopHolding = true;
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && !stopHolding && allowDrawing)
        {
            
            if (currentLine.transform.childCount > 0)
            {
                for (int i = 0; i < currentLine.transform.childCount; i++)
                {
                    currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                }
                listLine.Add(currentLine);
                currentLine.AddComponent<Rigidbody2D>().useAutoMass = true;
                float m = currentLine.GetComponent<Rigidbody2D>().mass * 500;
                currentLine.GetComponent<Rigidbody2D>().useAutoMass = false;
                currentLine.GetComponent<Rigidbody2D>().mass = m;
            }
            else
            {
                Destroy(currentLine);
            }
            foreach (Rigidbody2D rigid in listObstacleNonKinematic)
            {
                rigid.isKinematic = false;
            }
            if (clickCont == 0)
            {
                Glass.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                for (int i = 0; i < Obs.Length; i++)
                {
                    Obs[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                }
                for (int i = 0; i < waterTap.Length; i++)
                {
                    waterTap[i].GetComponent<ParticleGenerator>().enabled = true;
                }

                clickCont++;
            }
            Pencil.gameObject.SetActive(false);
            for (int i = 0; i < currentLine.transform.childCount; i++)
            {
                currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().size = new Vector2(currentLine.transform.GetChild(i).GetComponent<BoxCollider2D>().size.x, 0.05f);
                currentLine.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
        }
        LastMosPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void CreateLine(Vector2 mousePosition)
    {
        currentLine = new GameObject("Line");
        currentLineRenderer = currentLine.AddComponent<LineRenderer>();
        currentLineRenderer.sharedMaterial = lineMaterial;
        currentLineRenderer.positionCount = 0;
        currentLineRenderer.startWidth = 0.05f;
        currentLineRenderer.endWidth = 0.05f;
        currentLineRenderer.startColor = lineColor;
        currentLineRenderer.endColor = lineColor;
        currentLineRenderer.useWorldSpace = false;
    }

    public void StopAllPhysics()
    {
        for (int i = 0; i < listLine.Count; i++)
        {
            Rigidbody2D rigid = listLine[i].GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;
            rigid.simulated = false;
        }       
    }


}

