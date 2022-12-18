﻿using DevilsWarehouse.Common;
using DevilsWarehouse.Common.Abstract;
using DevilsWarehouse.Content.Items.Consumables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace DevilsWarehouse.Content.UI
{
    class VampireUI : UIState
    {
        private const float Precent = 0f;
        private UIElement area;
		private UIImage barFrame;
		public override void OnInitialize()
        {
			area = new UIElement();
			area.Left.Set(460, Precent);
			area.Top.Set(25, Precent);
			area.Width.Set(50, Precent);
			area.Height.Set(42, Precent);

			barFrame = new UIImage(ModContent.Request<Texture2D>("DevilsWarehouse/Assets/Textures/BerserkerUIEmpty"));
			barFrame.Left.Set(0, Precent);
			barFrame.Top.Set(0, Precent);
			barFrame.Width.Set(50, Precent);
			barFrame.Height.Set(42, Precent);

			area.Append(barFrame);
			Append(area);
		}
		public override void Draw(SpriteBatch spriteBatch)
        {
            var player = Main.LocalPlayer.GetModPlayer<Vampire>();
            if (!player.vampire)
                return;

            base.Draw(spriteBatch);
        }
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var player = Main.LocalPlayer.GetModPlayer<Vampire>();

			float quotient = (float)player.blood / player.bloodMax;
			quotient = Utils.Clamp(quotient, 0f, 1f);

			Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
			hitbox.X += 12;
			hitbox.Width -= 24;
			hitbox.Y += 30;
			hitbox.Height -= 40;

			int left = hitbox.Left;
			int right = hitbox.Right;
			int steps = (int)((right - left) * quotient);
			for (int i = 0; i < steps; i += 1)
			{
				float percent = (float)i / (hitbox.Top - hitbox.Bottom);
				spriteBatch.Draw((Texture2D)TextureAssets.MagicPixel, new Rectangle(hitbox.X, hitbox.Bottom - i, 25, hitbox.Height), Color.Lerp(Color.Red, Color.Orange, percent));
				base.DrawSelf(spriteBatch);
			}
		}
		public override void Update(GameTime gameTime)
		{
			var player = Main.LocalPlayer.GetModPlayer<Vampire>();

			if (!player.vampire)
				return;

            if (Main.playerInventory == false)
            {
				area.Left.Set(460, Precent);
				area.Top.Set(25, Precent);
			}
            else
            {
				area.Left.Set(560, Precent);
				area.Top.Set(40, Precent);
			}

			if(player.blood < player.bloodMax)
			{
				if (area.IsMouseHovering)
					Main.instance.MouseText(player.blood + "/" + player.bloodMax, 0, 0);

				barFrame.SetImage(ModContent.Request<Texture2D>("DevilsWarehouse/Assets/Textures/BerserkerUIEmpty"));
			}
            else{

				if (area.IsMouseHovering)
					Main.instance.MouseText("Max");

				barFrame.SetImage(ModContent.Request<Texture2D>("DevilsWarehouse/Assets/Textures/BerserkerUIMax"));
			}

			base.Update(gameTime);
		}
    }
}