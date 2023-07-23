using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class Gun : MonoBehaviour
{
    InputAction shoot;
    public Transform fpsCamera;
    public float range = 20;
    public float impactForce = 150;
    public int fireRate = 10;
    private float nextTimeToFire = 0;
    public ParticleSystem muzzleFlush;
    public GameObject impactEffect;
    public AudioSource clip;

    private int currentAmmo;
    public int maxAmmo = 10;
    public int magazineSize = 30;
    public TextMeshProUGUI ammoInfoText;
    public TextMeshProUGUI scoreText;

    public GameObject restartTextObject;
    private int score;

    public float reloadTime = 2f;
    private bool isReloading = false; 

    // public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        shoot = new InputAction("Shoot", binding: "<mouse>/leftButton");
        shoot.Enable();
        score = 0;
        restartTextObject.SetActive(false);
        StartCoroutine(sceneLoader());

        currentAmmo = maxAmmo;
    }
    
    private void OnEnable() {
        isReloading = false;
        // animator.SetBool("isReloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentAmmo == 0 && magazineSize == 0) {
            restartTextObject.SetActive(true);
        }
     
        ammoInfoText.text = $"{currentAmmo} / {magazineSize}";

        if(isReloading) {
            return;
        }
        bool isShooting = shoot.ReadValue<float>() == 1;

        if(isShooting && Time.time >= nextTimeToFire && currentAmmo > 0) {
            nextTimeToFire = Time.time + 1f / fireRate;
            Fire();
        }

        if(currentAmmo == 0 && magazineSize > 0 && !isReloading) {
            StartCoroutine(Reload());
        }
    }

    private void Fire() {
        RaycastHit hit;
        muzzleFlush.Play();
        clip.Play();
        currentAmmo--;
        if(Physics.Raycast(fpsCamera.position, fpsCamera.forward, out hit, range)) {
            if(hit.rigidbody != null) {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            if(hit.transform.CompareTag("Target")) {
                score += 10;
                SetCountText();
                hit.transform.gameObject.SetActive(false);
            }
        

            Quaternion impactRotation = Quaternion.LookRotation(hit.normal);
            GameObject impact = Instantiate(impactEffect, hit.point, impactRotation);
            Destroy(impact, 5);
        }
    }

    IEnumerator Reload() {
        isReloading = true;
        // animator.SetBool("isReloading", true);
        yield return new WaitForSeconds(reloadTime);
        // animator.SetBool("isReloading", false);

        if(magazineSize >= maxAmmo){
            currentAmmo = maxAmmo;
            magazineSize -= maxAmmo;
        } else {
            currentAmmo = magazineSize;
            magazineSize = 0;
        }
        isReloading = false;
    }

    IEnumerator sceneLoader()
    {
        Debug.Log("Waiting for Player ammo to be 0 ");
        yield return new WaitUntil(() => currentAmmo <= 0 && magazineSize <= 0);
        Debug.Log("No ammo left, restarting game.");
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }

     void SetCountText()
	{
		scoreText.text = "Score: " + score.ToString();

		// if (count >= 12) 
		// {
        //     winTextObject.SetActive(true);
		// }
	}
}
