﻿using DevilsWarehouse.Content.NPCs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DevilsWarehouse.Common.Systems
{
    public class WeaponWithGrowingDamage : GlobalItem
    {
        public int experience;
        public static int experiencePerLevel = 100;
        private int bonusValuePerItem;
        public int level => experience / experiencePerLevel;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            //Apply to weapons
            return entity.damage > 0;
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            experience = 0;
            GainExperience(item, tag.Get<int>("experience"));//Load experience tag
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            tag["experience"] = experience;//Save experience tag
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(experience);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            experience = 0;
            GainExperience(item, reader.ReadInt32());
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            OnHitNPCGeneral(player, target, damage, knockBack, crit, item);
        }

        public void OnHitNPCGeneral(Player player, NPC target, int damage, float knockBack, bool crit, Item item = null, Projectile projectile = null)
        {
            //The weapon gains experience when hitting an npc.
            int xp = damage;
            if (projectile != null)
            {
                xp /= 2;
            }

            GainExperience(item, xp);
        }

        public void GainExperience(Item item, int xp)
        {
            experience += xp;

            UpdateValue(item);
        }

        public void UpdateValue(Item item, int stackChange = 0)
        {
            if (item == null)
            {
                return;
            }

            item.value -= bonusValuePerItem;
            int stack = item.stack + stackChange;
            if (stack == 0)
            {
                bonusValuePerItem = 0;
            }
            else
            {
                bonusValuePerItem = experience * 5 / stack;
            }

            item.value += bonusValuePerItem;
        }

        public override void UpdateInventory(Item item, Player player)
        {
            UpdateValue(item);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            //Gain 1% multiplicative damage for every level on the weapon.
            damage *= 1f + (float)level / 100f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (experience > 0)
            {
                tooltips.Add(new TooltipLine(Mod, "level", $"Level: {level}") { OverrideColor = Color.LightGreen });
                string levelString = $" ({(level + 1) * experiencePerLevel - experience} to next level)";
                tooltips.Add(new TooltipLine(Mod, "experience", $"Experience: {experience}{levelString}") { OverrideColor = Color.White });
            }
        }

        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (item.type == ItemID.Snowball)
            {
                GainExperience(item, item.stack); 
            }

            if (context is RecipeCreationContext rContext)
            {
                foreach (Item ingredient in rContext.ConsumedItems)
                {
                    if (ingredient.TryGetGlobalItem(out WeaponWithGrowingDamage ingredientGlobal))
                    {
                        //Transfer all experience from consumed items to the crafted item.
                        GainExperience(item, ingredientGlobal.experience);
                    }
                }
            }
        }
    }

    public class StarSystem : GlobalItem
    {
        public const int starMax = 7;
        public int starCurrent = 0;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.damage > 0;
        }
        public override void OnCreate(Item item, ItemCreationContext context)
        {
            starCurrent = Main.rand.Next(starMax);
            Mod.Logger.Warn(starCurrent);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += 0.1f * starCurrent;
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            if (starCurrent >= starMax)
            {
                crit += 0.1f;
            }
        }

        #region Net Update
        public override void LoadData(Item item, TagCompound tag)
        {
            starCurrent = tag.GetInt("starCurrent");
        }
        public override void SaveData(Item item, TagCompound tag)
        {
            tag["starCurrent"] = starCurrent;
        }
        #endregion

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < starCurrent; i++)
            {
                sb.Append($"[i:{ItemID.FallenStar}] ");
            }

            var line = new TooltipLine(Mod, "Face", sb.ToString());
            tooltips.Add(line);
        }

        private void StarReceive()
        {
            
        }
    }
}