using UnityEngine;

using System.Collections;



//@script RequireComponent ("Camera-Control/Mouse Orbit")



public class CameraControll : MonoBehaviour
{



    //This code should attached to an empty object

    //I would name it "Camera Controller"

    public Transform target;

    public Transform cam; //In case you have more than one cam

    public float distance = 10.0F;

    public int cameraSpeed = 5;



    float xSpeed = 175.0F;

    float ySpeed = 75.0F;



    int yMinLimit = 20; //Lowest vertical angle in respect with the target.

    int yMaxLimit = 80;



    public int minDistance = 5; //Min distance of the cam from the target

    public int maxDistance = 20;



    private float x = 0.0F;

    private float y = 0.0F;







    void Start()
    {

        var angles = transform.eulerAngles;

        x = angles.y;

        y = angles.x;



        // Make the rigid body not change rotation

        if (GetComponent<Rigidbody>())

            GetComponent<Rigidbody>().freezeRotation = true;

    }



    void Update()
    {

        if (target && cam)
        {



            //Zooming with mouse

            distance -= Input.GetAxis("Mouse ScrollWheel") * distance;

            distance = Mathf.Clamp(distance, minDistance, maxDistance);



            //Detect mouse drag;

            if (Input.GetMouseButton(0))
            {



                x += Input.GetAxis("Mouse X") * xSpeed * 0.02F;

                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02F;

            }

            y = ClampAngle(y, yMinLimit, yMaxLimit);



            Quaternion rotation = Quaternion.Euler(y, x, 0);

            Vector3 position = rotation * (new Vector3(0.0f, 0.0f, -distance) + target.position);



            transform.position = Vector3.Lerp(transform.position, position, cameraSpeed * Time.deltaTime);

            transform.rotation = rotation;

        }

    }



    static float ClampAngle(float angle, float min, float max)
    {

        if (angle < -360)

            angle += 360;

        if (angle > 360)

            angle -= 360;



        return Mathf.Clamp(angle, min, max);

    }

}