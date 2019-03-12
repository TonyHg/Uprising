﻿using System;
using UnityEngine;
using Uprising.Players;

namespace Uprising.Items
{
    public abstract class Item 
    {
        public ItemType type;
        public int durability;
        public bool isCurrentlyUsed = false;
        public GameObject player; // TODO : Set player as a PlayerControl instead of GameObject
        public GameObject gameObject;

        public int GetDurability()
        {
            return durability;
        }

        // Called by Player's behavior to use the item.
        public abstract void Use();
        protected abstract void StopUsing();
        // Display item, and apply passif effect
        public virtual void Select()
        {
            player.GetComponent<PlayerControl>().hand.transform.Find("h_"+type.ToString()).gameObject.SetActive(true);
        }
        public virtual void Unselect()
        {
            player.GetComponent<PlayerControl>().hand.transform.Find("h_" + type.ToString()).gameObject.SetActive(false);
        }
    }

    public abstract class Effect : Item
    {
        // Called every frame
        public void Update()
        {
            // Remove from durability the time passed since the last frame.
            this.durability -= (int)(Time.deltaTime * 1000);
            if (this.durability <= 0)
            {
                this.StopUsing();
            }
        }

        public override void Use()
        {
            if (!isCurrentlyUsed)
            {
                this.isCurrentlyUsed = true;
                this.player.SendMessage("ApplyEffect", this);
                player.SendMessage("ClearItem", this as Item);
            }
        }

        protected override void StopUsing()
        {
            if (isCurrentlyUsed)
            {
                this.isCurrentlyUsed = false;
                this.player.SendMessage("UnApplyEffect", this);
            }
        }
    }

    //public abstract class Weapon : Item
    //{
    //    //
    //    public Weapon()
    //    {

    //    }

    //    // Called every frame
    //    public void Update()
    //    {
    //        if (this.durability <= 0)
    //        {
    //            this.StopUsing();
    //        }
    //    }

    //    public override void Use() //shoot
    //    {
    //        if (!isCurrentlyUsed)
    //        {
    //            this.durability--;
    //            // ici mettre le truc pour créer bullet
    //            this.isCurrentlyUsed = true;
    //        }
    //    }

    //    protected override void StopUsing()
    //    {
    //        this.isCurrentlyUsed = false;
    //    }

    //    public override void Select()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public override void Unselect()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public abstract void Aim();
    //}
}