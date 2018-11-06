using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    static public Hero S;

    [Header("Set in Inspector")]
    // These fields control the movement of the ship 
    public float        speed = 30;
    public float        rollMult = -45;
    public float        pitchMult = 30;
    public float        gameRestartDelay = 2f;
    public GameObject   projectilePrefab;
    public float        projectileSpeed = 40;
    public Weapon[]     weapons;



    [Header("Set Dynamically")]
    //public float        shieldLevel = 1;
    private GameObject  lastTriggerGo = null;

    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // Create a WeaponFireDelegate field named fireDelegate. 
    public WeaponFireDelegate fireDelegate;


    [SerializeField]
    private float       _shieldLevel = 1;



    void Start()
    {
        if (S == null)
        {
            S = this;
        }
           
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        //  fireDelegate += TempFire;
        // Reset the weapons to start _Hero with 1 blaster 
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);

    }

    void Update()
    {
        // Pull in information from the Input class 
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // Change transform.position based on the axes 
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos; 

        // Rotate the ship to make it feel more dynamic 
        transform.rotation = Quaternion.Euler(yAxis* pitchMult, xAxis* rollMult, 0);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //     TempFire();
        //}
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }

    }

    void TempFire()
    {
        GameObject projGO = Instantiate <GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        //  rigidB.velocity = Vector3. up * projectileSpeed;

        ProjectileHero proj = projGO.GetComponent<ProjectileHero>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidB.velocity = Vector3. up * tSpeed;
    }


    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);

        if (go == lastTriggerGo)
        {
            return;
        }

        lastTriggerGo = go;

        if(go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }

        else if (go.tag == "PowerUp")
        {
            // If the shield was triggered by a PowerUp 
            AbsorbPowerUp(go);
        }

        else
        {
            print("Triggered by non-Enemy: "+ go.name);
        }

    }

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }

        set
        {
            _shieldLevel = Mathf.Min(value, 4);
           
            // If the shield is going to be set to less than zero 
            if(value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return(null);
    }

    void ClearWeapons()
    {
        foreach(Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }


    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if(pu.type == weapons[0].type)
                { // If it is the same type 
                    Weapon w = GetEmptyWeaponSlot();
                    if(w!=null)
                    {
                        // Set it to pu.type 
                        w.SetType(pu.type);
                    }
                }
                else
                { // If this is a different weapon type 
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }

        pu.AbsorbedBy(this.gameObject);
    }


}