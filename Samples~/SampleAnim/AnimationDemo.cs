using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using QDuck.Animation;
using UnityEngine;

public class AnimationDemo : MonoBehaviour
{
    public AnimPro Anim;
    public float Speed = 1.0f;
    private bool isSpeedAdd =true;

    private void Awake()
    {
        Anim = GetComponent<AnimPro>();
        Anim.Play("Pose1");
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Anim.Play("Pose1");
        }else if (Input.GetKey(KeyCode.S))
        {
            Anim.Play("Pose2");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Anim.Play("Walk");
        }

        if (isSpeedAdd)
        {
            Speed+=Time.deltaTime * 0.1f;
            if (Speed >= 1)
            {
                Speed = 1;
                isSpeedAdd = false;
            }
        }
        else
        {
            Speed -= Time.deltaTime * 0.1f;
            if (Speed <= 0)
            {
                Speed = 0;
                isSpeedAdd = true;
            }
                
        }
        
        Anim.SetFloat("Speed", Speed);
        
    }
}
