using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SimpleMove : MonoBehaviour {
    public bool lockCursor = false;
    CursorLockMode currentMode;

    public float sensitivity = 30;
    public int smoothing = 10;

    float ymove;
    float xmove;

    int iteration = 0;

    float xaggregate = 0;
    float yaggregate = 0;

    //int Ylimit = 0;
    public int Xlimit = 20;

    Image Cross;
    [SerializeField] Sprite CursorImg, CursorActiveImg;

    void Start()
    {
        Cross = GameObject.Find("Cross").GetComponent<Image>();
        currentMode = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lockCursor = !lockCursor;
            Cursor.visible = !Cursor.visible;
            if (lockCursor)
            {
                currentMode = CursorLockMode.None;
                Cross.enabled = false;
            }
            else
            {
                currentMode = CursorLockMode.Locked;
                Cross.enabled = true;
            }
        }

        if (Input.GetMouseButton(0) && !lockCursor)
            Cross.sprite = CursorActiveImg;
        else
            Cross.sprite = CursorImg;

        Cursor.lockState = currentMode;
    }

    void FixedUpdate()
    {
        if (lockCursor)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontal, 0, vertical));
        
        float[] x = new float[smoothing];
        float[] y = new float[smoothing];

        xaggregate = 0;
        yaggregate = 0;

        ymove = Input.GetAxis("Mouse Y");
        xmove = Input.GetAxis("Mouse X");

        y[iteration % smoothing] = ymove;
        x[iteration % smoothing] = xmove;

        iteration++;

        foreach (float xmov in x)
        {
            xaggregate += xmov;
        }

        xaggregate = xaggregate / smoothing * sensitivity;

        foreach (float ymov in y)
        {
            yaggregate += ymov;
        }

        yaggregate = yaggregate / smoothing * sensitivity;

        Vector3 newOrientation = transform.eulerAngles + new Vector3(-yaggregate, xaggregate, 0);

        transform.eulerAngles = newOrientation;

    }
}
