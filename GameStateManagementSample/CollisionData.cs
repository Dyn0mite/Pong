﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongaThemes
{
    public class CollisionData
    {
        public Rectangle m_rect;
        public Color[]   m_color_data;
        public Matrix    m_transformation;

        public CollisionData(Rectangle rect)
        {
            m_transformation = Matrix.CreateTranslation(new Vector3(rect.X, rect.Y, 0.0f));

            m_rect = HelperUtils.CalculateBoundingRectangle(new Rectangle(0, 0, rect.Width, rect.Height), m_transformation);

            m_color_data = new Color[m_rect.Width * m_rect.Height];

            for (int i = 0; i < (m_rect.Width * m_rect.Height); i++)
            {
                m_color_data[i] = Color.White;
            }
        }

        public CollisionData(Rectangle rect, Color[] data, Vector2 origin, float scale, float rotation, SpriteEffects effects)
        {
            m_transformation = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                               Matrix.CreateScale(scale) *
                               ((effects == SpriteEffects.FlipVertically) ? Matrix.CreateRotationX(MathHelper.Pi) : Matrix.Identity) *
                               ((effects == SpriteEffects.FlipHorizontally) ? Matrix.CreateRotationY(MathHelper.Pi) : Matrix.Identity) * 
                               Matrix.CreateRotationZ(rotation) *
                               Matrix.CreateTranslation(new Vector3(rect.X, rect.Y, 0.0f));

            m_rect = HelperUtils.CalculateBoundingRectangle(new Rectangle(0, 0, rect.Width, rect.Height), m_transformation);

            m_color_data = data;
        }

        public CollisionData(int xpos, int ypos, Texture2D texture, Rectangle? src_rect, Vector2 origin, float scale, float rotation, SpriteEffects effects)
        {
            m_transformation = Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                               Matrix.CreateScale(scale) *
                               ((effects == SpriteEffects.FlipVertically) ? Matrix.CreateRotationX(MathHelper.Pi) : Matrix.Identity) *
                               ((effects == SpriteEffects.FlipHorizontally) ? Matrix.CreateRotationY(MathHelper.Pi) : Matrix.Identity) * 
                               Matrix.CreateRotationZ(rotation) *
                               Matrix.CreateTranslation(new Vector3(xpos, ypos, 0.0f));

            if (src_rect.HasValue)
            {
                m_rect = HelperUtils.CalculateBoundingRectangle(new Rectangle(0, 0, src_rect.Value.Width, src_rect.Value.Height), m_transformation);
                m_color_data = new Color[src_rect.Value.Width * src_rect.Value.Height];
                texture.GetData<Color>(0, src_rect, m_color_data, 0, src_rect.Value.Width * src_rect.Value.Height);
            }
            else
            {
                HelperUtils.CalculateBoundingRectangle(new Rectangle(0, 0, texture.Width, texture.Height), m_transformation);
                m_color_data = new Color[texture.Width * texture.Height];
                texture.GetData<Color>(0, src_rect, m_color_data, 0, texture.Width * texture.Height);
            }
        }
    }
}
