using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Valentina Genoese-Zerbi
// Contains all the scripting necessary for a base gameobject
// Notes for github merging:
//      Special Tower must use base's StartRespawn() when special tower is destroyed
//      Tower must remove StartRespawn
public class Base : Entity
{
    public Vector3 position = Vector3.zero;
    public int maxHp;
    public float m_Red;

    public bool gameOver = false;

    public GameObject avatarPrefab;
    public GameObject currentAvatar;
    public GameObject mapManager;

    public GameObject healthBarPrefab;
    [HideInInspector]
    public GameObject healthBar;
    [HideInInspector]
    public GameObject fullHealthBar;
    private int healthStartingScale = 8;

    // Start is called before the first frame update
    void Start()
    {
        currentAvatar = Instantiate(avatarPrefab, transform.position, Quaternion.identity);
        currentAvatar.GetComponent<Avatar>().playerBase = gameObject;
        currentAvatar.GetComponent<Avatar>().mapManager = mapManager;
        currentAvatar.transform.position += new Vector3(0, 0, -3);
        maxHp = base.hp;
        Vector3 hpPosition = transform.position;
        hpPosition.y -= 0.5f;
        fullHealthBar = Instantiate(healthBarPrefab, hpPosition, Quaternion.identity);
        healthBar = Instantiate(healthBarPrefab, hpPosition, Quaternion.identity);
        fullHealthBar.GetComponent<SpriteRenderer>().color = Color.black;
        healthBar.GetComponent<SpriteRenderer>().color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoseHealth(int amount)
    {
        healthBar.transform.localScale = new Vector3(healthStartingScale * base.hp / maxHp, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        //healthBar.transform.position = new Vector3(/*something sprite width etc*/healthBar.transform.position.x - healthStartingScale * base.hp / maxHp / 2, healthBar.transform.position.y, healthBar.transform.position.z);
        base.LoseHealth(amount);
    }

    /// <summary>
    /// Ends game (destroys everything and displays game over message)
    /// </summary>
    protected override void Die()
    {
        gameOver = true;
    }
}
