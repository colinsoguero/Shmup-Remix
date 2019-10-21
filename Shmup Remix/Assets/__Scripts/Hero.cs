using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hero : MonoBehaviour
{
    static public Hero S; // Singleton
    [Header("Set in Inspector")]
    //These fields control the movement of the ship
    public float speed = 37;
    public float health = 100;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 1f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public float showDamageDuration = 0.1f;
    private bool charging = false;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;
    // This variable holds a reference to the last triggering GameObject
    private GameObject lastTriggerGo = null;
    public Color[] originalColors;
    public Material[] materials; // All the Materials of this and its children
    public bool showingDamage = false;
    public float damageDoneTime; // Time to stop showing damage
    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // Create a WeaponFireDelegate field named fireDelegate.
    public WeaponFireDelegate fireDelegate;

    public Image panel;
    public Image EnemyPanel;

    public AudioSource collide;


    private void Awake()
    {
        if (S == null)
        {
            S = this; // Set the Singleton
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
        panel = GameObject.Find("Panel").GetComponent<Image>();
        if(SceneManager.GetActiveScene().name == "_Scene_4")
        EnemyPanel = GameObject.Find("EnemyHealth").GetComponent<Image>();


    }
    private void Update()
    {
        // Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        if (!(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)))
        {
            // Change transform.position based on the axes
            Vector3 pos = transform.position;
            pos.x += xAxis * speed * Time.deltaTime;
            pos.y += yAxis * speed * Time.deltaTime;
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.UpArrow))
        {
            speed += 5;
            charging = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 37;
            charging = false;
        }
        if (Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.V))
        {
            Weapon.instance.SetType(WeaponType.blaster);
        }
        else if (Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.B))
        {
            Weapon.instance.SetType(WeaponType.blueblaster);
        }
        else
        {
            Weapon.instance.SetType(WeaponType.none);
        }

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if (Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            shieldLevel = 2;
        }
        else
        {
            shieldLevel = 0;
        }



        // Rotate the ship to make it feel more dynamic
            transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);


        // Use the fireDelegate to fire Weapons
        // First, make sure the button is pressed: Axis("Jump")
        // Then ensure that fireDelegate isn't null to avoid an error


        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }

        if (health <= 0)
        {
            Destroy(this.gameObject);
            // Tell Main.S to restart the game after a delay
            Main.S.DelayedRestart(gameRestartDelay);
        }

        panel.fillAmount = health/100;

    }


    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        // Make sure it's not the same triggering go as last time
        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "Enemy") // If the shield was triggered by an enemy
        {          
            if (go.name == "Enemy_4(Clone)")
            {
                if (charging)
                {
                    go.GetComponent<Enemy_4>().health -= 33.4f;
                }
                else
                {
                    health -= 25;
                    ShowDamage();
                }               
            }
            else
            {
                Destroy(go); // ... and Destroy the enemy
                health -= 25;
                ShowDamage();
            }
            
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
        EnemyPanel.fillAmount = go.GetComponent<Enemy_4>().health / 100;
        if (go.GetComponent<Enemy_4>().health <= 0)
        {
            Destroy(go);
            SceneManager.LoadScene("_Scene_5");
        }
        collide.Play();
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
            if (value < 0)
            {
                Destroy(this.gameObject);
                // Tell Main.S to restart the game after a delay
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

    private void OnTriggerExit(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        if (go.tag == "Enemy")
        {
            lastTriggerGo = null;
        }
    }

}
