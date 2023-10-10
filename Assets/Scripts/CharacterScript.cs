using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public GameObject target;
    public GameObject startPoint;

    public float speed = 0.001f;
    public float maxEyesDistance = 1.5f;
    public float sideEyesDistance = 0.5f;

    public bool finished = false;
    public bool hitOnTarget = false;

    public bool hitFront, hitLeft, hitRight;


    public bool rotageLeft;

    void Start()
    {
        target = GameObject.Find("Main/target");
        startPoint = new GameObject("startPoint_" + transform.name);
        startPoint.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        startPoint.transform.SetParent(GameObject.Find("Main/CharacterSet").transform);
        print("fuck" + new System.Random().Next(0, 2));
        rotageLeft = new System.Random().Next(0,2)==1;
    }

    void Update()
    {
        if (!finished)
        {
            Walk();

            Looking();
            LookingLeft();
            LookingRight();

            FindingPath();
        }
    }

    private void FindingPath()
    {
         if (hitFront)
        {
            print("hit at front");
            float rotageTo = rotageLeft ? 3f : -3f;
            this.gameObject.transform.Rotate(new Vector3(0, rotageTo, 0));
        }
        else if (hitLeft)
        {
            this.gameObject.transform.Rotate(new Vector3(0, 0.5f, 0));
        }
        else if (hitRight)
        {
            this.gameObject.transform.Rotate(new Vector3(0, -0.5f, 0));

        }
        else if (!hitLeft && !hitRight && !hitFront)
        {
            print("No hit");
            this.gameObject.transform.LookAt(target.transform);
        }
    }

    private void Walk()
    {
        this.gameObject.transform.Translate(new Vector3(0, 0, speed)); 
    }

    private void Looking()
    {

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        RaycastHit hit;

        Debug.DrawRay(origin, direction * maxEyesDistance, Color.green);

        if (Physics.SphereCast(origin, 0.1f, direction, out hit, maxEyesDistance))
        {
            hitFront = true;
            print($"look at front [{hit.transform.name}]->" + hit.distance);
            if(hit.distance < 0.5f && hit.transform.name.Equals("target")) {
                this.BacktoStartPoint();
            }
        }
        else 
        {
            hitFront = false;
        }
    }

    private void LookingLeft()
    {
        Ray rayLeft = new Ray(transform.position, -transform.right);
        RaycastHit hitLeft;
        this.hitLeft = (Physics.Raycast(rayLeft, out hitLeft, sideEyesDistance));

        Debug.DrawRay(transform.position, -transform.right * sideEyesDistance, Color.blue);
    }
    private void LookingRight()
    {
        Ray rayRight = new Ray(transform.position, transform.right);
        RaycastHit hitRight;
        this.hitRight = (Physics.Raycast(rayRight, out hitRight, maxEyesDistance));

        Debug.DrawRay(transform.position, transform.right * sideEyesDistance, Color.red);

    }

    private void BacktoStartPoint()
    {
        this.target = this.startPoint;
        hitOnTarget = true;

        BoxCollider startPointCollider = startPoint.AddComponent<BoxCollider>();
        startPointCollider.size = new Vector3(0.01f, 0.01f, 0.01f);
        startPointCollider.enabled = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        print("HIT AT =>");
        if (collision.gameObject.transform.name.Equals("target"))
        {
            print("Hit the target");
            this.BacktoStartPoint();
        }
        else if (collision.gameObject.transform.name.Equals("startPoint_" + this.transform.name))
        {
            print("back to start point");
            finished = true;
            this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        }

    }

}
