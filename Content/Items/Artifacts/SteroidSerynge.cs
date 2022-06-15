﻿using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using GearonArsenal.Content.Projectiles;
using Microsoft.Xna.Framework;

namespace GearonArsenal.Content.Items.Artifacts
{
    internal class SteroidSerynge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Steroid Serynge");
            Tooltip.SetDefault("each critical damage you throw a card in a enemy, if the three types of card hit a same enemy you killed instantly");
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<Artifact>();
            Item.accessory = true;
            Item.defense = 5;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<BattleMedic>().battleMedic = true;
            player.manaRegen -= 5;
        }
    }
    public class BattleMedic : ModPlayer
    {
        public bool battleMedic = false;

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.DamageType == DamageClass.Ranged && battleMedic && crit)
            {
                if (Player.statMana >= Player.statManaMax2)
                {
                    Player.statMana -= Player.statManaMax2;

                    Vector2 pos = new Vector2(Player.Center.X, Player.Center.Y - 10);

                    Item.NewItem(new EntitySource_DropAsItem(default), pos, new 
                        Vector2(10, -5), ModContent.ItemType<SupplyCrate>(), 1);

                    Item.NewItem(new EntitySource_DropAsItem(default), pos, new 
                        Vector2(-10, -5), ModContent.ItemType<SupplyCrate>(), 1);
                }
            }
        }
    }
}