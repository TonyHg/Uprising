﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uprising.Items;

public class GrapnelController : MonoBehaviour
{
    public GameObject hook;
    public GameObject flyingHook;
    public GameObject player;
    public Grapnel grapnel;

    public void Shoot(Grapnel grapnel)
    {
        this.grapnel = grapnel;
        flyingHook = Instantiate(hook, transform.position, transform.rotation);
        flyingHook.GetComponent<HookController>().Init(gameObject);
    }

    public void Detach()
    {
        Destroy(flyingHook);
        flyingHook = null;
    }
}
