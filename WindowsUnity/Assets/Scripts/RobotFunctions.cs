using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RobotFunctions : MonoBehaviour
{
    public int player;
    public GameObject enemy;
    public RectTransform healthBar;

    public Image meleeImage;
    public Image rangedImage;
    public Image leftImage;
    public Image leftArrowImage;
    public Image rightImage;
    public Image rightArrowImage;

    public int maxHealth;
    public int armor;

    public int meleeCooldown;
    public int meleeUses;
    public int meleeDamage;
    public float meleeSpeed;

    public int rangedCooldown;
    public int rangedUses;
    public int rangedDamage;
    public float rangedSpeed;

    public int specialUses;

    public int wheelSpeed;
    public int wheelPower;

    public GameObject cannonBallPrefab;
    public GameObject arrowPrefab;
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;

    public GameObject barrelPrefab;
    public GameObject shieldPrefab;
    public GameObject oilPrefab;

    private int oldHealth;
    public int currentHealth;

    private float rotationSpeed = 500f;
    private float hitSpeed = 10f;
    private float stabSpeed = 20f;
    private float amplitude = 0.75f;

    [HideInInspector]
    public bool meleeIsCooling = false;
    public bool rangedIsCooling = false;
    public bool specialHasUsed = false;

    private int meleeUsedTimes=0;
    private int rangedUsedTimes=0;
    private int specialUsedTimes=0;

    [HideInInspector]
    public GameObject playerBody;
    public GameObject playerMelee;
    public GameObject playerRanged;
    public GameObject playerSpecial;
    public GameObject playerWheelFront;
    public GameObject playerWheelBack;

    private Rigidbody2D rb;

    private List<GameObject> oilSpills = new List<GameObject>();
    private List<GameObject> burningOilSpills = new List<GameObject>();
    private bool isBurning = false;

    private bool isPoisoned = false;

    private int iceAmount = 0;

    private GameObject ammoHolder;

    public int playerPosition;

    private bool checkingMeleeImage = false;
    private bool checkingRangedImage = false;
    private bool checkingLeftImage = false;
    private bool checkingRightImage = false;
    private float lastUsedMelee;
    private float lastUsedRanged;
    private float lastUsedLeft;
    private float lastUsedRight;

    private float timeDelay = 0.2f;

    private bool dead = false;

    private GameObject shield;

    AudioSource[] bodyAudioSources;
    AudioSource flamethrowerAudioSource;
    AudioSource specialAudioSource;

    private void Start()
    {
        playerBody = gameObject;
        playerMelee = playerBody.transform.GetChild(0).transform.GetChild(0).gameObject;
        playerRanged = playerBody.transform.GetChild(1).transform.GetChild(0).gameObject;
        playerSpecial = playerBody.transform.GetChild(2).transform.GetChild(0).gameObject;
        playerWheelFront = playerBody.transform.GetChild(3).transform.GetChild(0).gameObject;
        playerWheelBack = playerBody.transform.GetChild(4).transform.GetChild(0).gameObject;

        bodyAudioSources = playerBody.GetComponents<AudioSource>();
        foreach (AudioSource audioSource in bodyAudioSources)
        {
            float maxVolume = audioSource.volume;
            audioSource.volume = PlayerPrefs.GetFloat("Battle", 1)*maxVolume;
        }
        specialAudioSource = playerSpecial.GetComponent<AudioSource>();

        float maxSpecialVolume = specialAudioSource.volume;
        specialAudioSource.volume = PlayerPrefs.GetFloat("Battle", 1)*maxSpecialVolume;


        StartCoroutine(GetImages());

        StartCoroutine(GetColor());

        rb = playerBody.GetComponent<Rigidbody2D>();
        rb.mass = wheelPower;

        ammoHolder=new GameObject("Ammo Holder");

        if (playerRanged.name =="FlameThrower(Clone)")
        {
            flamethrowerAudioSource = playerRanged.GetComponent<AudioSource>();

            float maxVolume = flamethrowerAudioSource.volume;
            flamethrowerAudioSource.volume = PlayerPrefs.GetFloat("Battle", 1)*maxVolume;

            StartCoroutine(GetEnemyFlameThrower());

            StartCoroutine(GetPlayer());

            if(playerSpecial.name=="Oil(Clone)")
            {
                StartCoroutine(CheckForOilSpills());
            }
        }

        StartCoroutine(GetEnemyMelee());

        oldHealth = maxHealth;
        currentHealth = maxHealth;
    }

    private IEnumerator GetImages()
    {
        while (meleeImage == null || rangedImage == null || leftImage == null || rightImage == null || leftArrowImage == null || rightArrowImage == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        meleeImage.sprite = playerMelee.GetComponent<SpriteRenderer>().sprite;
        rangedImage.sprite = playerRanged.GetComponent<SpriteRenderer>().sprite;
        leftImage.sprite = playerWheelFront.GetComponent<SpriteRenderer>().sprite;
        rightImage.sprite = playerWheelBack.GetComponent<SpriteRenderer>().sprite;
    }

    private IEnumerator GetPlayer()
    {
        while (player == 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        ParticleSystem burn = playerRanged.GetComponentInChildren<ParticleSystem>();

        var collider = burn.collision;

        string layerName = "";
        string shieldLayer = "";
        if (player == 1)
        {
            layerName = "Player2Body";
            shieldLayer = "Shield2";
        }
        else
        {
            layerName = "Player1Body";
            shieldLayer = "Shield1";
        }

        collider.collidesWith = LayerMask.GetMask(layerName,"Floor",shieldLayer);
    }

    private IEnumerator GetColor()
    {
        if (player == 0)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(GetColor());
        }
        else
        {
            if (player == 1)
            {
                playerWheelFront.GetComponent<SpriteRenderer>().color = Color.red;
                playerWheelBack.GetComponent<SpriteRenderer>().color = Color.red;

                playerPosition = 1;
            }
            else
            {
                playerWheelFront.GetComponent<SpriteRenderer>().color = Color.blue;
                playerWheelBack.GetComponent<SpriteRenderer>().color = Color.blue;

                playerPosition = 2;
            }
        }
    }

    private IEnumerator GetEnemyFlameThrower()
    {
        if (enemy == null)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(GetEnemyFlameThrower());
        }
        else
        {
            FlameThrower flameThrower = playerRanged.GetComponentInChildren<FlameThrower>();
            flameThrower.enemy = enemy;
            flameThrower.damage = rangedDamage;
        }
    }

    private IEnumerator GetEnemyMelee()
    {
        if (enemy == null)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(GetEnemyMelee());
        }
        else
        {
            Melees meleeScript = playerMelee.GetComponent<Melees>();
            meleeScript.enemy = enemy;
            meleeScript.damage = meleeDamage;
            meleeScript.speed = meleeSpeed;
        }
    }

    public void TakeDamage(int damage)
    {
        int onePrecent=damage/100;
        int damageReduction = armor * onePrecent;
        
        int realDamage = damage - damageReduction;

        currentHealth -= realDamage;

        bodyAudioSources[1].PlayOneShot(bodyAudioSources[1].clip);
    }

    public void Heal(int heal)
    {
        currentHealth += heal;

        if(currentHealth>maxHealth)
        {
            currentHealth = maxHealth;
        }

        bodyAudioSources[2].Play();
    }

    public void ResetRobot(Vector2 position)
    {
        dead = false;

        StopAudioSources();

        playerBody.transform.position = position;

        ParticleSystem[] particleSystems = playerBody.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Stop();
            particleSystem.Clear();
        }

        playerBody.GetComponentInChildren<Rigidbody2D>().simulated = true;

        playerBody.GetComponent<SpriteRenderer>().enabled = true;

        for (int i = 0; i <= 4; i++)
        {
            playerBody.transform.GetChild(i).gameObject.SetActive(true);
        }

        playerSpecial.SetActive(true);

        currentHealth = maxHealth;

        meleeIsCooling = false;
        rangedIsCooling = false;
        specialHasUsed = false;

        meleeUsedTimes = 0;
        rangedUsedTimes = 0;
        specialUsedTimes = 0;

        isBurning = false;
        isPoisoned = false;

        iceAmount = 0;

        for (int i = 0; i < oilSpills.Count; i++)
        {
            Destroy(oilSpills[i]);
        }

        for (int i = 0; i < burningOilSpills.Count; i++)
        {
            Destroy(burningOilSpills[i]);
        }

        oilSpills.Clear();
        burningOilSpills.Clear();

        for (int i = 0; i < ammoHolder.transform.childCount; i++)
        {
            Destroy(ammoHolder.transform.GetChild(i).gameObject);
        }

        meleeImage.color = new Color(1, 1, 1, 0.2f);
        rangedImage.color = new Color(1, 1, 1, 0.2f);
        leftImage.color = new Color(1, 1, 1, 0.2f);
        rightImage.color = new Color(1, 1, 1, 0.2f);
        leftArrowImage.color = new Color(1, 1, 1, 0.2f);
        rightArrowImage.color = new Color(1, 1, 1, 0.2f);

        Destroy(shield);
        shield = null;
    }

    private void StopAudioSources()
    {
        specialAudioSource.Stop();
        AudioSource meleeAudioSource= playerMelee.GetComponent<AudioSource>();
        meleeAudioSource.Stop();
        if(flamethrowerAudioSource!=null)
        {
            flamethrowerAudioSource.Stop();
        }
    }

    private void Update()
    {
        if(currentHealth!=oldHealth)
        {
            float widthOfHealthBar = (650*currentHealth)/maxHealth;

            healthBar.sizeDelta = new Vector2(widthOfHealthBar, healthBar.sizeDelta.y);
            oldHealth = currentHealth;
        }
    }

    int speedMultiplier = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Oil")
        {
            speedMultiplier = 2;
        }
        else if(collision.tag=="Burning Oil")
        {
            speedMultiplier = 2;
            isBurning = true;
            StartCoroutine(Burning());
        }
        else if(collision.tag=="Poison")
        {
            isPoisoned = true;
            StartCoroutine(Poison());
        }
        else if(collision.tag=="Finish")
        {
            Demolish();
        }
        else if(collision.tag=="Ice")
        {
            speedMultiplier=2;
            iceAmount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Oil")
        {
            speedMultiplier = 1;
        }
        else if (collision.tag == "Burning Oil")
        {
            speedMultiplier = 1;
            isBurning = false;
        }
        else if (collision.tag == "Poison")
        {
            isPoisoned = false;
        }
        else if (collision.tag == "Ice")
        {
            iceAmount--;
            if (iceAmount == 0)
            {
                speedMultiplier = 1;
            }
        }
    }

    public IEnumerator Poison()
    {
        currentHealth -= 5;

        bodyAudioSources[1].PlayOneShot(bodyAudioSources[1].clip);

        yield return new WaitForSeconds(0.5f);

        if(isPoisoned)
        {
            StartCoroutine(Poison());
        }
    }

    /////////////////////////BODY FUNCTIONS/////////////////////////

    private int demolishDamage = 1000;
    public void Demolish()
    {
        dead = true;

        StopAudioSources();

        ParticleSystem[] particleSystems = playerBody.GetComponentsInChildren<ParticleSystem>();

        bodyAudioSources[0].Play();

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            if(particleSystem.transform.parent==gameObject.transform)
            {
                particleSystem.Play();
                break;
            }
        }

        playerBody.GetComponentInChildren<Rigidbody2D>().simulated = false;

        playerBody.GetComponent<SpriteRenderer>().enabled = false;

        float distance = Vector2.Distance(playerBody.transform.position, enemy.transform.position);
        if (distance < 1.5f)
        {
            int explosionDamage = (int)((distance * demolishDamage) / 1.5f);
            enemy.GetComponent<RobotFunctions>().currentHealth -= explosionDamage;
        }

        for (int i = 0;i<=4;i++)
        {
            playerBody.transform.GetChild(i).gameObject.SetActive(false);
        }

        currentHealth = 0;
    }

    /////////////////////////TIRE FUNCTIONS/////////////////////////

    //starfish, diamond, fat and robo tires
    public void MoveForward()
    { 
        Vector2 moveDirection = Vector2.right;
        Vector2 newVelocity = rb.linearVelocity;
        newVelocity.y = rb.linearVelocity.y;

        int direction = 1;
        if(player == 2)
        {
            direction = -1;
        }
        newVelocity.x = moveDirection.x * wheelSpeed*speedMultiplier;
        rb.linearVelocity = newVelocity;

        if(playerWheelFront.name=="Tire5(Clone)")
        {
            playerWheelFront.GetComponent<Animator>().SetTrigger("Move");
            playerWheelBack.GetComponent<Animator>().SetTrigger("Move");
        }

        if(player==playerPosition && playerWheelFront.name!="Tire5(Clone)")
        {
            playerWheelFront.transform.Rotate(direction*rotationSpeed * Time.deltaTime * Vector3.forward);
            playerWheelBack.transform.Rotate(direction*rotationSpeed * Time.deltaTime * Vector3.forward);
        }
        else if(playerWheelFront.name!="Tire5(Clone)")
        {
            playerWheelFront.transform.Rotate(-direction*rotationSpeed * Time.deltaTime * Vector3.forward);
            playerWheelBack.transform.Rotate(-direction*rotationSpeed * Time.deltaTime * Vector3.forward);
        }

        if(playerSpecial.name=="Turbo(Clone)")
        {
            RotateTurbo("f");
        }

        lastUsedRight = Time.time;
        if(!checkingRightImage)
        {
            StartCoroutine(CheckRightImage());
        }
    }

    private IEnumerator CheckRightImage()
    {
        checkingRightImage = true;
        rightImage.color = new Color(1, 1, 1, 1);
        rightArrowImage.color = new Color(1, 1, 1, 1);

        while(Time.time-lastUsedRight<timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
        }

        rightImage.color = new Color(1, 1, 1, 0.2f);
        rightArrowImage.color= new Color(1, 1, 1, 0.2f);

        checkingRightImage = false;
    }

    //starfish, diamond, fat and robo tires
    public void MoveBackward()
    {
        Vector2 moveDirection = Vector2.left;
        Vector2 newVelocity = rb.linearVelocity;
        newVelocity.y = rb.linearVelocity.y;

        int direction = 1;
        if(player == 2)
        {
            direction = -1;
        }
        newVelocity.x = moveDirection.x * wheelSpeed*speedMultiplier;
        rb.linearVelocity = newVelocity;

        if (playerWheelFront.name == "Tire5(Clone)")
        {
            playerWheelFront.GetComponent<Animator>().SetTrigger("Move");
            playerWheelBack.GetComponent<Animator>().SetTrigger("Move");
        }

        if (player == playerPosition && playerWheelFront.name!="Tire5(Clone)")
        {
            playerWheelFront.transform.Rotate(-direction * rotationSpeed * Time.deltaTime * Vector3.forward);
            playerWheelBack.transform.Rotate(-direction * rotationSpeed * Time.deltaTime * Vector3.forward);
        }
        else if(playerWheelFront.name!="Tire5(Clone)")
        {
            playerWheelFront.transform.Rotate(direction * rotationSpeed * Time.deltaTime * Vector3.forward);
            playerWheelBack.transform.Rotate(direction * rotationSpeed * Time.deltaTime * Vector3.forward);
        }

        if (playerSpecial.name == "Turbo(Clone)")
        {
            RotateTurbo("b");
        }

        lastUsedLeft = Time.time;
        if (!checkingLeftImage)
        {
            StartCoroutine(CheckLeftImage());
        }
    }

    private IEnumerator CheckLeftImage()
    {
        checkingLeftImage = true;
        leftImage.color = new Color(1, 1, 1, 1);
        leftArrowImage.color = new Color(1, 1, 1, 1);

        while (Time.time - lastUsedLeft < timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
        }

        leftImage.color = new Color(1, 1, 1, 0.2f);
        leftArrowImage.color = new Color(1, 1, 1, 0.2f);

        checkingLeftImage = false;
    }

    /////////////////////////MELEE FUNCTIONS/////////////////////////
    
    public void MeleeAttack()
    {
        if (!meleeIsCooling && !dead)
        {
            if (playerMelee.name == "Axe(Clone)")
            {
                Spin();
            }
            else if (playerMelee.name == "Saw(Clone)")
            {
                Spin();
            }
            else if (playerMelee.name == "Hammer(Clone)")
            {
                Hit();
            }
            else if (playerMelee.name == "Sword(Clone)")
            {
                Stab();
            }
            else if (playerMelee.name == "Chainsaw(Clone)")
            {
                Cut();
            }

        }
        lastUsedMelee = Time.time;
        if(!checkingMeleeImage)
        {
            StartCoroutine(CheckMeleeImage());
        }
    }

    private IEnumerator CheckMeleeImage()
    {
        checkingMeleeImage = true;
        meleeImage.color = new Color(meleeImage.color.r, meleeImage.color.g, meleeImage.color.b, 1);

        while (Time.time - lastUsedMelee < timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
        }

        meleeImage.color = new Color(meleeImage.color.r, meleeImage.color.g, meleeImage.color.b, 0.2f);

        checkingMeleeImage = false;
    }

    public void MeleeCooldown()
    {
        if (!meleeIsCooling)
        {
            StartCoroutine(MeleeCooldownCoroutine());
        }
    }

    public IEnumerator MeleeCooldownCoroutine()
    {
        meleeIsCooling = true;
        if(player==1)
        {
            meleeImage.color = new Color(1, 0, 0, meleeImage.color.a);
        }
        else
        {
            meleeImage.color = new Color(0, 0, 1, meleeImage.color.a);
        }
        yield return new WaitForSeconds(meleeCooldown);
        meleeIsCooling = false;
        meleeImage.color = new Color(1, 1, 1, meleeImage.color.a);
    }

    //axe and saw
    public void Spin()
    {
        float oldRotation = playerMelee.transform.rotation.eulerAngles.z;

        playerMelee.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);

        float newRotation = playerMelee.transform.rotation.eulerAngles.z;

        if(oldRotation > newRotation)
        {
            meleeUsedTimes++;

            if (meleeUsedTimes >= meleeUses)
            {
                meleeUsedTimes = 0;
                StartCoroutine(MeleeCooldownCoroutine());
            }
        }
    }

    int maxAngle = 90;
    bool countedHammer = false;
    float lastAngle = 0;

    //hammer
    public void Hit()
    {
        float angle = Mathf.Sin(Time.time * hitSpeed) * maxAngle;
        playerMelee.transform.rotation = Quaternion.Euler(0, 0, angle);

        float currentAngle = playerMelee.transform.rotation.eulerAngles.z;

        float normalizedCurrent = NormalizeAngle(currentAngle);
        float normalizedLast = NormalizeAngle(lastAngle);

        if (normalizedLast > 0 && normalizedCurrent <= 0 && !countedHammer)
        {
            countedHammer = true;
            meleeUsedTimes++;

            if (meleeUsedTimes >= meleeUses)
            {
                meleeUsedTimes = 0;
                StartCoroutine(MeleeCooldownCoroutine());
            }
        }
        else if (normalizedCurrent > 0)
        {
            countedHammer = false;
        }

        lastAngle = currentAngle;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180)
            angle -= 360;
        return angle;
    }

    bool countedSword = false;
    //Blade
    public void Stab()
    {
        float newX = Mathf.Sin(Time.time * stabSpeed) * amplitude;
        playerMelee.transform.localPosition = new Vector3(newX, 0, 0);

        if (newX > 0 && !countedSword)
        {
            countedSword = true;
            meleeUsedTimes++;

            if (meleeUsedTimes >= meleeUses)
            {
                meleeUsedTimes = 0;
                StartCoroutine(MeleeCooldownCoroutine());
            }
        }
        else if (newX <= 0)
        {
            countedSword = false;
        }
    }

    bool isCountingCut = false;
    //Chainsaw
    public void Cut()
    {
        playerMelee.GetComponent<Animator>().SetTrigger("Cut");

        if (!isCountingCut)
        {
            isCountingCut = true;
            StartCoroutine(CountCut());
        }
    }

    public IEnumerator CountCut()
    {
        yield return new WaitForSeconds(meleeSpeed);

        meleeUsedTimes++;

        if (meleeUsedTimes >= meleeUses)
        {
            meleeUsedTimes = 0;
            StartCoroutine(MeleeCooldownCoroutine());
        }

        isCountingCut = false;
    }

    /////////////////////////RANGED FUNCTIONS/////////////////////////
    
    public void RangedAttack()
    {
        if (!rangedIsCooling && !dead)
        {
            if (playerRanged.name == "Cannon(Clone)")
            {
                StartCoroutine(Fire());
            }
            else if (playerRanged.name == "Crossbow(Clone)")
            {
                StartCoroutine(Release());
            }
            else if (playerRanged.name == "GrenadeThrower(Clone)")
            {
                StartCoroutine(Throw());
            }
            else if (playerRanged.name == "Gun(Clone)")
            {
                StartCoroutine(Shred());
            }
            else if (playerRanged.name == "FlameThrower(Clone)")
            {
                StartCoroutine(Burn());
            }
        }
        lastUsedRanged = Time.time;
        if (!checkingRangedImage)
        {
            StartCoroutine(CheckRangedImage());
        }
    }

    private IEnumerator CheckRangedImage()
    {
        checkingRangedImage = true;
        rangedImage.color = new Color(rangedImage.color.r, rangedImage.color.g, rangedImage.color.b, 1);

        while (Time.time - lastUsedRanged < timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
        }

        if(flamethrowerAudioSource!=null)
        {
            flamethrowerAudioSource.Stop();
        }

        rangedImage.color = new Color(rangedImage.color.r, rangedImage.color.g, rangedImage.color.b, 0.2f);

        checkingRangedImage = false;
    }

    public void RangedCooldown()
    {
        if (!rangedIsCooling)
        {
            StartCoroutine(RangedCooldownCoroutine());
        }
    }

    public IEnumerator RangedCooldownCoroutine()
    {
        rangedIsCooling = true;
        if (player == 1)
        {
            rangedImage.color = new Color(1, 0, 0, rangedImage.color.a);
        }
        else
        {
            rangedImage.color = new Color(0, 0, 1, rangedImage.color.a);
        }
        yield return new WaitForSeconds(rangedCooldown);
        rangedIsCooling = false;
        rangedImage.color = new Color(1, 1, 1, rangedImage.color.a);
    }

    int launchForce = 10;
    bool isFiring = false;
    //Cannon
    public IEnumerator Fire()
    {
        if(isFiring)
        {
            yield break;
        }

        isFiring = true;
        playerRanged.GetComponent<Animator>().SetTrigger("Fire");


        GameObject cannonball = Instantiate(cannonBallPrefab, playerRanged.transform.position, Quaternion.identity, ammoHolder.transform);
        if(player==1)
        {
            cannonball.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0.8f, 1);
            cannonball.GetComponentInChildren<TrailRenderer>().startColor = Color.red;
        }
        else
        {
            cannonball.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 1, 1);
            cannonball.GetComponentInChildren<TrailRenderer>().startColor = Color.blue;
        }
        Ammo ammo = cannonball.GetComponent<Ammo>();
        ammo.damage = rangedDamage;
        ammo.enemy = enemy;

        if (player == 1)
        {
            cannonball.layer = 7;
        }
        else
        {
            cannonball.layer = 9;
        }

        Rigidbody2D rb = cannonball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 launchDirection = Vector2.right;

            if (playerPosition == 1)
            {
                launchDirection = Quaternion.Euler(0, 0, 35) * Vector2.right;
                rb.AddForce(launchDirection * launchForce * 2.5f, ForceMode2D.Impulse);
            }
            else
            {
                launchDirection = Quaternion.Euler(0, 0, -35) * Vector2.right;
                rb.AddForce(-launchDirection * launchForce * 2.5f, ForceMode2D.Impulse);
            }
        }

        rangedUsedTimes++;

        if (rangedUsedTimes >= rangedUses)
        {
            rangedUsedTimes = 0;
            StartCoroutine(RangedCooldownCoroutine());
        }

        yield return new WaitForSeconds(rangedSpeed);
        isFiring = false;
    }

    //Crossbow
    public IEnumerator Release()
    {
        if (isFiring)
        {
            yield break;
        }

        isFiring = true;
        playerRanged.GetComponent<Animator>().SetTrigger("Release");


        GameObject arrow = Instantiate(arrowPrefab, playerRanged.transform.position, Quaternion.identity, ammoHolder.transform);
        if (player == 1)
        {
            arrow.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0.8f, 1);
            arrow.GetComponentInChildren<TrailRenderer>().startColor = Color.red;
        }
        else
        {
            arrow.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 1, 1);
            arrow.GetComponentInChildren<TrailRenderer>().startColor = Color.blue;
        }
        Ammo ammo = arrow.GetComponent<Ammo>();
        ammo.damage = rangedDamage;
        ammo.enemy = enemy;

        if (player == 1)
        {
            arrow.layer = 7;
        }
        else
        {
            arrow.layer = 9;
        }

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (playerPosition == 1)
            {
                arrow.transform.Rotate(0, 180, 0);
                rb.linearVelocity = new Vector2(launchForce * 2, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(-launchForce * 2, rb.linearVelocity.y);
            }
        }

        rangedUsedTimes++;

        if (rangedUsedTimes >= rangedUses)
        {
            rangedUsedTimes = 0;
            StartCoroutine(RangedCooldownCoroutine());
        }

        yield return new WaitForSeconds(rangedSpeed);
        isFiring = false;
    }

    //Gun
    public IEnumerator Shred()
    {
        if (isFiring)
        {
            yield break;
        }

        isFiring = true;
        playerRanged.GetComponent<Animator>().SetTrigger("Shred");

        GameObject bullet = Instantiate(bulletPrefab, playerRanged.transform.position, Quaternion.identity, ammoHolder.transform);
        if (player == 1)
        {
            bullet.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0.8f, 1);
            bullet.GetComponentInChildren<TrailRenderer>().startColor = Color.red;
        }
        else
        {
            bullet.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 1, 1);
            bullet.GetComponentInChildren<TrailRenderer>().startColor = Color.blue;
        }
        Ammo ammo = bullet.GetComponent<Ammo>();
        ammo.damage = rangedDamage;
        ammo.enemy = enemy;

        if (player == 1)
        {
            bullet.layer = 7;
        }
        else
        {
            bullet.layer = 9;
        }

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (playerPosition == 1)
            {
                bullet.transform.Rotate(0, 180, 0);
                rb.linearVelocity = new Vector2(launchForce * 3, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(-launchForce * 3, rb.linearVelocity.y);
            }
        }

        rangedUsedTimes++;

        if (rangedUsedTimes >= rangedUses)
        {
            rangedUsedTimes = 0;
            StartCoroutine(RangedCooldownCoroutine());
        }

        yield return new WaitForSeconds(rangedSpeed);
        isFiring = false;
    }

    //Flamethrower
    public IEnumerator Burn()
    {
        if(!flamethrowerAudioSource.isPlaying)
        {
            flamethrowerAudioSource.Play();
        }

        if (isFiring)
        {
            yield break;
        }

        isFiring = true;
        playerRanged.GetComponent<Animator>().SetBool("Burn",true);

        ParticleSystem fire = playerRanged.GetComponentInChildren<ParticleSystem>();
        fire.Play();

        rangedUsedTimes++;

        if (rangedUsedTimes >= rangedUses)
        {
            rangedUsedTimes = 0;
            StartCoroutine(RangedCooldownCoroutine());
        }

        yield return new WaitForSeconds(rangedSpeed);

        playerRanged.GetComponent<Animator>().SetBool("Burn", false);
        fire.Stop();
        isFiring = false;
    }

    //Grenade
    public IEnumerator Throw()
    {
        if (isFiring)
        {
            yield break;
        }

        isFiring = true;
        playerRanged.GetComponent<Animator>().SetTrigger("Throw");

        GameObject grenade = Instantiate(grenadePrefab, playerRanged.transform.position, Quaternion.identity, ammoHolder.transform);
        if (player == 1)
        {
            grenade.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0.8f, 1);
            grenade.GetComponentInChildren<TrailRenderer>().startColor = Color.red;
        }
        else
        {
            grenade.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 1, 1);
            grenade.GetComponentInChildren<TrailRenderer>().startColor = Color.blue;
        }
        Ammo ammo = grenade.GetComponent<Ammo>();
        ammo.damage = rangedDamage;
        ammo.enemy = enemy;

        if (player == 1)
        {
            grenade.layer = 7;
        }
        else
        {
            grenade.layer = 9;
        }

        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 launchDirection = Vector2.right;
            if (playerPosition == 1)
            {
                launchDirection = Quaternion.Euler(0, 0, 50) * Vector2.right;
                rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
            }
            else
            {
                launchDirection = Quaternion.Euler(0, 0, -50) * Vector2.right;
                rb.AddForce(-launchDirection * launchForce, ForceMode2D.Impulse);
            }
        }

        rangedUsedTimes++;

        if (rangedUsedTimes >= rangedUses)
        {
            rangedUsedTimes = 0;
            StartCoroutine(RangedCooldownCoroutine());
        }

        yield return new WaitForSeconds(rangedSpeed);
        isFiring = false;
    }

    /////////////////////////SPECIAL FUNCTIONS/////////////////////////
    
    bool isSpecial = false;
    public void Special()
    {
        if (!specialHasUsed && !isSpecial)
        {
            if (playerSpecial.name == "Barrel(Clone)")
            {
                StartCoroutine(Barrel());
            }
            else if (playerSpecial.name == "Magnet(Clone)")
            {
                StartCoroutine(Magnet());
            }
            else if (playerSpecial.name == "ShieldBall(Clone)")
            {
                StartCoroutine(Shield());
            }
            else if (playerSpecial.name == "Oil(Clone)")
            {
                StartCoroutine(Oil());
            }
            else if (playerSpecial.name == "Turbo(Clone)")
            {
                StartCoroutine(Turbo());
            }
        }
    }

    public void SpecialUsedAll()
    {
        specialHasUsed = true;
        playerSpecial.SetActive(false);
    }

    //Barrel
    public IEnumerator Barrel()
    {
        if (isSpecial)
        {
            yield break;
        }

        isSpecial = true;

        GameObject barrel = Instantiate(barrelPrefab, playerSpecial.transform.position, Quaternion.identity, ammoHolder.transform);

        specialAudioSource.Play();

        barrel.GetComponent<Barrel>().enemy = enemy;
        barrel.GetComponent<Barrel>().player = gameObject;

        barrel.transform.localScale = new Vector2(0.75f, 0.75f);

        Rigidbody2D rb = barrel.GetComponent<Rigidbody2D>();
        rb.simulated = true;

        Vector2 launchDirection = Vector2.right;
        if (playerPosition == 1)
        {
            launchDirection = Quaternion.Euler(0, 0, 50) * Vector2.right;
            rb.AddForce(launchDirection * launchForce*1.5f, ForceMode2D.Impulse);
        }
        else
        {
            launchDirection = Quaternion.Euler(0, 0, -50) * Vector2.right;
            rb.AddForce(-launchDirection * launchForce*1.5f, ForceMode2D.Impulse);
        }

        specialUsedTimes++;

        if (specialUsedTimes >= specialUses)
        {
            SpecialUsedAll();
        }

        yield return new WaitForSeconds(1f);

        isSpecial = false;
        barrel.layer = 12;
    }

    //Magnet
    public IEnumerator Magnet()
    {
        if (isSpecial)
        {
            yield break;
        }

        isSpecial = true;

        playerSpecial.GetComponent<Animator>().SetBool("Pull", true);

        StartCoroutine(Pull());

        specialUsedTimes++;

        specialAudioSource.Play();

        yield return new WaitForSeconds(5f);

        specialAudioSource.Stop();

        if (specialUsedTimes >= specialUses)
        {
            SpecialUsedAll();
        }

        isSpecial = false;
        
        playerSpecial.GetComponent<Animator>().SetBool("Pull", false);
    }

    int pullForce = 500;
    public IEnumerator Pull()
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        Vector2 direction = playerBody.transform.position - enemy.transform.position;

        while (isSpecial && !dead)
        {
            enemyRb.AddForce(direction.normalized * pullForce, ForceMode2D.Force);

            yield return new WaitForSeconds(0.1f);
        }
    }

    //Shield
    public IEnumerator Shield()
    {
        if (isSpecial)
        {
            yield break;
        }

        isSpecial = true;

        shield = Instantiate(shieldPrefab, playerBody.transform);
        shield.transform.localPosition = new Vector3(0, 0, 0);
        shield.transform.SetSiblingIndex(5);

        specialAudioSource.Play();

        if(player==1)
        {
            shield.layer = 13;
        }
        else
        {
            shield.layer = 14;
        }
        
        shield.GetComponent<Animator>().enabled = true;
        shield.GetComponent<Animator>().SetTrigger("Shield");

        specialUsedTimes++;


        StartCoroutine(CheckDeathForShield(shield));

        yield return new WaitForSeconds(8f);

        if (specialUsedTimes >= specialUses)
        {
            SpecialUsedAll();
        }

        specialAudioSource.Stop();

        if (shield != null)
        {
            Destroy(shield);
            shield = null;
        }

        isSpecial = false;
    }

    public IEnumerator CheckDeathForShield(GameObject shield)
    {
        while (true)
        {
            if (dead)
            {
                Destroy(shield);
                shield = null;
                yield break;
            }

            if(shield==null)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    //Oil
    public IEnumerator Oil()
    {
        if (isSpecial)
        {
            yield break;
        }

        isSpecial = true;

        GameObject oil = Instantiate(oilPrefab, playerSpecial.transform.position, Quaternion.identity);

        specialAudioSource.Play();

        oilSpills.Add(oil);

        specialUsedTimes++;

        if (specialUsedTimes >= specialUses)
        {
            SpecialUsedAll();
        }

        yield return new WaitForSeconds(5f);

        isSpecial = false;
    }

    public IEnumerator CheckForOilSpills()
    {
        while (true)
        {
            for (int i = 0; i < oilSpills.Count; i++)
            {
                if(isFiring)
                {
                    if (Vector2.Distance(playerBody.transform.position, oilSpills[i].transform.position)<6 && Mathf.Abs(playerBody.transform.position.y - oilSpills[i].transform.position.y) < 2)
                    {
                        if ((playerPosition == 1 && oilSpills[i].transform.position.x > playerBody.transform.position.x) || (playerPosition == 2 && oilSpills[i].transform.position.x < playerBody.transform.position.x))
                        {
                            oilSpills[i].GetComponentInChildren<ParticleSystem>().Play();

                            oilSpills[i].tag = "Burning Oil";

                            float maxVolume = oilSpills[i].GetComponent<AudioSource>().volume;
                            oilSpills[i].GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Battle", 1)*maxVolume;
                            oilSpills[i].GetComponent<AudioSource>().Play();

                            burningOilSpills.Add(oilSpills[i]);
                            oilSpills.RemoveAt(i);
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator Burning()
    {
        currentHealth -= 25;

        bodyAudioSources[1].PlayOneShot(bodyAudioSources[1].clip);

        yield return new WaitForSeconds(0.1f);

        if(isBurning)
        {
            StartCoroutine(Burning());
        }
    }

    //Turbo
    public IEnumerator Turbo()
    {
        if (isSpecial)
        {
            yield break;
        }

        isSpecial = true;

        wheelSpeed*=2;
        playerSpecial.GetComponentInChildren<ParticleSystem>().Play();

        specialAudioSource.Play();

        specialUsedTimes++;

        yield return new WaitForSeconds(5f);

        specialAudioSource.Stop();

        if (specialUsedTimes >= specialUses)
        {
            SpecialUsedAll();
        }

        wheelSpeed/=2;
        playerSpecial.GetComponentInChildren<ParticleSystem>().Stop();

        isSpecial = false;
    }

    private void RotateTurbo(string direction)
    {
        if(player==1)
        {
            if(direction=="f")
            {
                if(playerSpecial.transform.localRotation.y==1)
                {
                    playerSpecial.transform.Rotate(0, -180, 0);
                }
            }
            else if(direction=="b")
            {
                if (playerSpecial.transform.localRotation.y == 0)
                {
                    playerSpecial.transform.Rotate(0, 180, 0);
                }
            }
        }
        else
        {
            if (direction == "f")
            {
                if (playerSpecial.transform.localRotation.y == 0)
                {
                    playerSpecial.transform.Rotate(0, 180, 0);
                }
            }
            else if (direction == "b")
            {
                if (playerSpecial.transform.localRotation.y == 1)
                {
                    playerSpecial.transform.Rotate(0, -180, 0);
                }
            }
        }
    }
}
