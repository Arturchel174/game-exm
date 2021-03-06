﻿using Battleship.ViewportAdapters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    /// <summary>
    /// Represents an item that can be dragged and/or dropped.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DragonDrop<T> : DrawableGameComponent where T : IDragAndDropItem
    {
        #region Fields
        MouseState oldMouse, currentMouse;

        public readonly ViewportAdapter viewport;

        public T selectedItem;
        public List<T> dragItems;
        public List<T> mouseItems;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor. Uses MonoGame.Extended ViewportAdapter
        /// </summary>
        /// <param name="game"></param>
        /// <param name="sb"></param>
        /// <param name="vp"></param>
        public DragonDrop(Game game, ViewportAdapter vp) : base(game)
        {
            viewport = vp;
            selectedItem = default(T);
            dragItems = new List<T>();
            mouseItems = new List<T>();
        }
        #endregion

        #region Methods and Properties
        public void Add(T item)
        {
            dragItems.Add(item);
        }
        public void Remove(T item, GameTime gameTime) { dragItems.Remove(item); item.Update(gameTime); }

        public void Clear()
        {
            selectedItem = default(T);
            dragItems.Clear();
        }

        private bool click => currentMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;
        private bool unClick => currentMouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed;
        private bool drag => currentMouse.LeftButton == ButtonState.Pressed;

        private Vector2 CurrentMouse
        {
            get
            {

                var point = viewport.PointToScreen(currentMouse.X, currentMouse.Y);

                return new Vector2(point.X, point.Y);

            }
        }

        public Vector2 OldMouse
        {
            get
            {

                var point = viewport.PointToScreen(oldMouse.X, oldMouse.Y);

                return new Vector2(point.X, point.Y);

            }
        }

        public Vector2 Movement => CurrentMouse - OldMouse;

        private T GetCollusionItem()
        {

            var items = dragItems.OrderByDescending(z => z.ZIndex).ToList();
            foreach (var item in items)
            {

                if (item.Contains(CurrentMouse) && !Equals(selectedItem, item)) return item;

            }

            // if it doesn't contain the current mouse, run again to see if it intersects
            foreach (var item in items)
            {

                if (item.Border.Intersects(selectedItem.Border) && !Equals(selectedItem, item)) return item;

            }
            return default(T);

        }

        private T GetMouseHoverItem()
        {

            var items = dragItems.OrderByDescending(z => z.ZIndex).ToList();

            foreach (var item in items)
            {

                if (item.Contains(CurrentMouse)) return item;

            }

            return default(T);

        }

        public override void Update(GameTime gameTime)
        {

            currentMouse = Mouse.GetState();


            if (selectedItem != null)
            {

                if (selectedItem.IsSelected)
                {

                    if (drag)
                    {
                        selectedItem.Position += Movement;
                        selectedItem.Update(gameTime);
                    }
                    else if (unClick)
                    {

                        var collusionItem = GetCollusionItem();

                        if (collusionItem != null)
                        {
                            selectedItem.OnCollusion(collusionItem);
                            collusionItem.Update(gameTime);
                        }

                        selectedItem.OnDeselected();
                        selectedItem.Update(gameTime);

                    }
                }

            }


            foreach (var item in dragItems)
            {
                item.IsMouseOver = false;
                item.Update(gameTime);
            }

            var hoverItem = GetMouseHoverItem();

            if (hoverItem != null)
            {

                hoverItem.IsMouseOver = true;

                if (hoverItem.IsDraggable && click)
                {
                    selectedItem = hoverItem;
                    selectedItem.OnSelected();
                }

                hoverItem.Update(gameTime);

            }


            oldMouse = currentMouse;

        }
        #endregion
    }
}
