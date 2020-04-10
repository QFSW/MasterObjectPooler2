using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoController : MonoBehaviour
{
    public int currentAmmo;
    public int availableAmmo;
    [SerializeField] private int defaultReloadAmount;
    [SerializeField] private int maxAmmo;

    [SerializeField] private Text availableAmmoUI;
    [SerializeField] private Text currentAmmoUI;

    void Start()
    {
        currentAmmoUI.text = currentAmmo.ToString("0");
        UpdateAmmo(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            if (availableAmmo >= defaultReloadAmount)
            {
                currentAmmo = defaultReloadAmount;
                availableAmmo -= defaultReloadAmount;
                UpdateAmmo(0);
                currentAmmoUI.text = currentAmmo.ToString("0");
            }

        }
    }
    public void UpdateCurrentAmmo()
    {
        if (currentAmmo >= 0)
        {
            currentAmmo--;
            currentAmmoUI.text = currentAmmo.ToString("0");
        }
    }

    public void UpdateAmmo(int ammo)
    {
        if (availableAmmo <= maxAmmo)
        {
            availableAmmo = availableAmmo + ammo;
            availableAmmoUI.text = availableAmmo.ToString("0");
        }
    }
}
