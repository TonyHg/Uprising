﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using Uprising.Items;

namespace Uprising.Players
{

    [RequireComponent(typeof(PlayerControl))]
    public class InventoryManager : MonoBehaviour
    {
        private Item[] items; // 0: Primary Weapon, 1: Secondary Weapon, 2: Bonus 1, 3: Bonus 2
        private int selectedItem;
        private List<Item> appliedEffects;
        private PlayerControl playerControl;

        // Start is called before the first frame update
        void Start()
        {
            appliedEffects = new List<Item>();
            items = new Item[4];
            playerControl = GetComponent<PlayerControl>();
        }

        void FixedUpdate()
        {
            //if (true || PlayerControl.photonView.IsMine)
            //{
            //    // GetInput();
            //}

            // Update all bonuses timer
            foreach (Item effect in appliedEffects.ToList())
            {
                (effect as Effect).Update();
            }
        }

        public int GetSelectedItem()
        {
            return selectedItem;
        }

        // Inventory Management
        public void GiveItem(Item item)
        {
            Debug.Log("Give item : " + item.type);
            // Add item to inventory
            if (item is Weapon)
            {
                items[1] = item;
                // playerControl.hud.transform.Find("Slot2 Weapon").Find(item.type.ToString()).gameObject.SetActive(true);
            }
            else
            {
                if (items[2] == null)
                {
                    items[2] = item;
                    //playerControl.hud.transform.Find("Slot3 Item").Find(item.type.ToString()).gameObject.SetActive(true);
                    Debug.Log(playerControl.hud.transform.Find("Slot3 Item"));
                }
                else if (items[3] == null)
                {
                    items[3] = item;
                    // playerControl.hud.transform.Find("Slot4 Item").Find(item.type.ToString()).gameObject.SetActive(true);
                }
                else items[(selectedItem < 2) ? 2 : selectedItem] = item;
            }
            item.player = playerControl.gameObject;

        }

        public void SelectItem(int index)
        {
            // Limit selecting range
            if (index < 0) index = 3;
            if (index > 3) index = 0;

            Debug.Log("Select item : " + items[index]);

            // Unselect previous item, then select new one
            if (items[selectedItem] != null)
            {
                items[selectedItem].Unselect();
            }
            selectedItem = index;
            if(items[index] != null) items[index].Select();
            Debug.Log(items[index] != null);
        }

        public void UseSelectedItem()
        {
            if(items[selectedItem] != null)
                items[selectedItem].Use();
        }

        public void ApplyEffect(Item effectToApply)
        {
            Debug.Log("Apply Effect : " + effectToApply.type);
            Item applied = appliedEffects.Find(x => x.type == effectToApply.type);
            if (applied == null) // If the effect is not already applied
            {
                appliedEffects.Add(effectToApply);
                switch (effectToApply.type)
                {
                    case ItemType.SpeedBoost:
                        playerControl.ModifySpeed(5);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                applied.durability += effectToApply.durability;
                effectToApply = null;
            }
        }

        public void UnApplyEffect(Item effectToDisable)
        {
            Item applied = appliedEffects.Find(x => x.type == effectToDisable.type);
            if (applied != null)
            {
                appliedEffects.Remove(applied);
                switch (effectToDisable.type)
                {
                    case ItemType.SpeedBoost:
                        playerControl.ModifySpeed(-5);
                        break;
                    default:
                        break;
                }
            }
        }

        public void ClearItem(Item item)
        {
            for (var i = 0; i < items.Length; i++)
            {
                if (items[i] == item)
                {
                    if (i == selectedItem) items[i].Unselect();
                    items[i] = null;
                    return;
                }
            }
        }
    }
}