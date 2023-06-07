using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rigid;    
    Material Mat;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();        
        Mat = GetComponent<MeshRenderer>().material;
    }

    public void SetRigidMass(float val)
    {
        if (rigid !=null)
        {
            rigid.mass = val;
        }
    }

    public void SetVelocity(Vector3 Vec3)
    {
        rigid.velocity = Vec3;
    }
}
