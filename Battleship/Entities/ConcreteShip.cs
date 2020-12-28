using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Battleship
{
    /// <summary>
    /// Represents a concrete ship to be used in the game.
    /// </summary>
    public class ConcreteShip : Ship, IDragAndDropItem
    {
        #region Fields
        SpriteBatch spriteBatch;
        private Texture2D shipTexture;
        private Rectangle m_rect;
        private int z_index = 0;
        private Vector2 position = Vector2.Zero;
        private ContentManager contentManager;
        #endregion

        #region Initialization
        /// <summary>
        /// Creates a ship.
        /// </summary>
        /// <param name="len">the ship length: from 2 to 5</param>
        /// <param name="sDir">the ship direction</param>
        /// <param name="sprite">the sprite batch used to draw</param>
        /// <param name="theContentManager">the game content manager</param>
        public ConcreteShip(int len, ShipDirection sDir, SpriteBatch sprite, ContentManager theContentManager)
        {
            contentManager = theContentManager;
            State = ShipState.Unhit;
            numberOfHit = 0;
            spriteBatch = sprite;
            switch (len)
            {
                case 2:
                    Name = "Destroyer";
                    Type = ShipType.Destroyer;
                    Direction = sDir;
                    length = len;
                    if (Direction == ShipDirection.Horizontal)
                        shipTexture = theContentManager.Load<Texture2D>("ship2h");
                    else
                        shipTexture = theContentManager.Load<Texture2D>("ship2v");
                    // Set default pos(0) and size
                    m_rect = new Rectangle(0, 0, shipTexture.Width, shipTexture.Height);
                    break;
                case 3:
                    Name = "Submarine";
                    Type = ShipType.Submarine;
                    Direction = sDir;
                    length = len;
                    if (Direction == ShipDirection.Horizontal)
                        shipTexture = theContentManager.Load<Texture2D>("ship3h");
                    else
                        shipTexture = theContentManager.Load<Texture2D>("ship3v");
                    m_rect = new Rectangle(0, 0, shipTexture.Width, shipTexture.Height);
                    break;
                case 4:
                    Name = "BattleShip";
                    Type = ShipType.BattleShip;
                    Direction = sDir;
                    length = len;
                    if (Direction == ShipDirection.Horizontal)
                        shipTexture = theContentManager.Load<Texture2D>("ship4h");
                    else
                        shipTexture = theContentManager.Load<Texture2D>("ship4v");
                    m_rect = new Rectangle(0, 0, shipTexture.Width, shipTexture.Height);
                    break;
                case 5:
                    Name = "AirCraft";
                    Type = ShipType.AirCraft;
                    Direction = sDir;
                    length = len;
                    if (Direction == ShipDirection.Horizontal)
                        shipTexture = theContentManager.Load<Texture2D>("ship5h");
                    else
                        shipTexture = theContentManager.Load<Texture2D>("ship5v");
                    m_rect = new Rectangle(0, 0, shipTexture.Width, shipTexture.Height);
                    break;
                default:
                    Name = "UNKNOWN";
                    Type = ShipType.Unknown;
                    Direction = ShipDirection.Undefined;
                    length = 0;
                    break;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load a ship texture given ship length and direction.
        /// </summary>
        /// <param name="len">the ship length: from 2 to 5</param>
        /// <param name="isVert">true if ship direction is vertical, else assume horizontal direction</param>
        public void ChangeTexture(int len, bool isVert)
        {
            if (isVert)
            {
                shipTexture = contentManager.Load<Texture2D>("ship" + len + "v");
            }
            else
            {
                shipTexture = contentManager.Load<Texture2D>("ship" + len + "h");
            }

        }

        /// <summary>
        /// Returns ship rectangle
        /// </summary>
        public Rectangle ShipRectangle
        {
            get
            {
                return m_rect;
            }
            set
            {
                m_rect = value;
            }
        }

        // the ship current position
        public Vector2 Position { get => position; set => position = value; }

        public Rectangle Border => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        public int ZIndex { get => z_index; set => z_index = value; }

        public Texture2D Texture => shipTexture;

        /*public Texture2D ShipTexture
        {
            get
            {
                return shipTexture;
            }
        }*/

        public override int GetLength()
        {
            return length;
        }

        public void Update(GameTime gameTime)
        {

        }

        /// <summary>        
        /// Draw this ship.
        /// </summary>        
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
        #endregion

        #region DragonDrop Stuff

        public bool IsSelected { get; set; } = false;

        public bool IsMouseOver { get; set; }

        public bool IsDraggable { get; set; } = true;
        public bool Contains(Vector2 pointToCheck)
        {
            var mouse = new Point((int)pointToCheck.X, (int)pointToCheck.Y);
            return Border.Contains(mouse);
        }

        #endregion

        #region events

        public event EventHandler Selected;

        public void OnSelected()
        {

            if (IsDraggable)
            {
                IsSelected = true;
            }
            //ZIndex += ON_TOP;

            Selected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Deselected;

        public void OnDeselected()
        {

            IsSelected = false;
            
            Deselected?.Invoke(this, EventArgs.Empty);
        }


        public event EventHandler<CollusionEvent> Collusion;

        public void OnCollusion(IDragAndDropItem item)
        {

            var e = new CollusionEvent { item = item };

            Collusion?.Invoke(this, e);

        }
        
        public class CollusionEvent : EventArgs
        {

            public IDragAndDropItem item { get; set; }

        }

        //public event EventHandler<SaveEvent> Save;
        //public class SaveEvent : EventArgs
        //{
        //}

        #endregion
        
    }
}
