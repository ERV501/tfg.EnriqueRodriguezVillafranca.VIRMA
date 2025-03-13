using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{

    private float rot_X = 0f, rot_Y = 0f;

    [SerializeField]
    public float speedRotation = 80f, speedMovement = 40f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Lock cursor
    }

    // Update is called once per frame
    void Update()
    {
        //Adjust movement speed
        if (Input.GetKeyDown(KeyCode.LeftShift)) //Faster while holding
        {
            speedMovement = speedMovement * 4f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) //Normal when released
        {
            speedMovement = speedMovement / 4f;
        }

        //Mouse rotation = camera rotation
        if (Input.GetMouseButton(1)) //Activate rotation on "right click" hold
        {
            rot_X += Input.GetAxis("Mouse X") * speedRotation * Time.deltaTime; //guiño (yaw)
            rot_Y -= Input.GetAxis("Mouse Y") * speedRotation * Time.deltaTime; //cabeceo (pitch)

            rot_Y = Mathf.Clamp(rot_Y, -90f, 90f); //Limit rotation

            transform.eulerAngles = new Vector3(rot_Y, rot_X, 0); //Set rotation
        }

        //Keyboard movement (directional)
        if (Input.GetKey(KeyCode.W)) //Forwards
        {
            transform.position += transform.forward * Time.deltaTime * speedMovement;
        }
        if (Input.GetKey(KeyCode.S)) //Backwards
        {
            transform.position -= transform.forward * Time.deltaTime * speedMovement;
        }
        if (Input.GetKey(KeyCode.D)) //Right
        {
            transform.position += transform.right * Time.deltaTime * speedMovement;
        }
        if (Input.GetKey(KeyCode.A)) //Left
        {
            transform.position -= transform.right * Time.deltaTime * speedMovement;
        }

        //Keyboard movement (elevation)
        if (Input.GetKey(KeyCode.Space)) //Up
        {
            transform.position += transform.up * Time.deltaTime * speedMovement;
        }
        else if (Input.GetKey(KeyCode.LeftAlt)) //Down
        {
            transform.position -= transform.up * Time.deltaTime * speedMovement;
        }
    }
}
