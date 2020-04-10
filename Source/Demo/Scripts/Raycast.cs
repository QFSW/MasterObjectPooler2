using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    [SerializeField] private int rayLength;
    [SerializeField] private LayerMask layerMaskInteract;
    private GameObject raycastedObject;

    [SerializeField] private AmmoController myAmmoController;

    void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract))
        {
            if (hit.collider.CompareTag("AmmoBox"))
            {
                if (Input.GetKeyDown("e"))
                {
                    myAmmoController.UpdateAmmo(30);
                }
            }
        }
    }
}
