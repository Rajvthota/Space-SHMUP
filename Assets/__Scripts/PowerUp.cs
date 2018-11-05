﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    [Header(" Set in Inspector")]
    // This is an unusual but handy use of Vector2s. x holds a min value 
    // and y a max value for a Random.Range() that will be called later 
    public Vector2  rotMinMax = new Vector2(15,90);
    public Vector2  driftMinMax = new Vector2(.25f, 2);
    public float    lifeTime = 6f; // Seconds the PowerUp exists 
    public float    fadeTime = 4f; // Seconds it will then fade

    [Header(" Set Dynamically")]
    public WeaponType    type; // The type of the PowerUp
    public GameObject    cube; // Reference to the Cube child
    public TextMesh      letter; // Reference to the TextMesh 
    public Vector3       rotPerSecond; // Euler rotation speed 
    public float         birthTime;

    private Rigidbody    rigid;
    private BoundsCheck  bndCheck;
    private Renderer     cubeRend;

    void Awake()
    {
        // Find the Cube reference 
        cube = transform.Find("Cube").gameObject;
        // Find the TextMesh and other components 
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        // Set a random velocity 
        Vector3 vel = Random.onUnitSphere;
        // Get Random XYZ velocity 
        // Random.onUnitSphere gives you a vector point that is somewhere on 
        // the surface of the sphere with a radius of 1m around the origin 
        vel.z = 0; // Flatten the vel to the XY plane 
        vel.Normalize(); // Normalizing a Vector3 makes it length 1m 
        vel *= Random.Range( driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // Set the rotation of this GameObject to R:[ 0, 0, 0 ] 
        transform.rotation = Quaternion.identity;
        // Quaternion.identity is equal to no rotation. 

        // Set up the rotPerSecond for the Cube child using rotMinMax x & y 
        rotPerSecond = new Vector3( Random.Range( rotMinMax.x, rotMinMax.y), 
            Random.Range( rotMinMax.x, rotMinMax.y), 
            Random.Range( rotMinMax.x, rotMinMax.y) );

        birthTime = Time.time; }


}
