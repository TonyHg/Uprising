﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uprising.Players;

namespace Uprising.Items
{
    public class Minigun : Weapon
    {

        public Minigun(int durability, float accuracy, float firerate, float knockback, GameObject player)
        {
            this.type = ItemType.Minigun;
            this.durability = durability;
            this.accuracy = accuracy;
            this.firerate = firerate;
            this.knockback = knockback;
            this.player = player;

            fireratetime = firerate;
        }

        public override void Aim()
        {
            Debug.Log("Aimed.");

        }

        public override void Use()
        {
            if (fireratetime >= firerate)
            {
                player.GetComponent<PlayerControl>().hand.transform.Find("h_Minigun").GetComponent<belettegen>().shoot(durability, this.player.transform.forward, this);
                fireratetime = 0;
            }
            if (durability < 0)
            {
                StopUsing();
            }
        }
    }
}