using Battleship.Audio;
using Battleship.Engine;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Battleship.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        private Board p1Board;
        private Board cpuBoard;
        private CollisionTile tile;
        internal SpriteBatch spriteBatch;
        private TableClassic table;
        private SpriteFont gridFont;
        private TileGrid tileGrid;
        Texture2D redCross;
        Texture2D redCircle;
        Texture2D blueCross;
        Texture2D blueCircle;
        
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //pauseAction = new InputAction(
            //    new Buttons[] { Buttons.Start, Buttons.Back },
            //    new Keys[] { Keys.Escape },
            //    true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        /// <param name="instancePreserved"></param>
        public override void Activate(bool instancePreserved)
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // constructs game objects
            p1Board = new Board();
            cpuBoard = new Board();

            tile = new CollisionTile(content, 1, new Rectangle(0, 0, CollisionTile.DIM, CollisionTile.DIM));

            gridFont = content.Load<SpriteFont>("gridPos");
            tileGrid = new TileGrid(CollisionTile.DIM, CollisionTile.DIM, Board.DIM, Board.DIM, tile, gridFont);

            for (int i = 0; i < Board.DIM; i++)
                for (int j = 0; j < Board.DIM; j++)
                    tileGrid.SetTile(i, j, 1);

            var screenManager = (ScreenManager)ScreenManager.Game.Components[0];
            var audioManager = (AudioManager)ScreenManager.Game.Components[1];
            var dragonDrop = (DragonDrop<IDragAndDropItem>)ScreenManager.Game.Components[2];

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = ScreenManager.SpriteBatch;

            blueCircle = content.Load<Texture2D>("ob_let");
            blueCross = content.Load<Texture2D>("xb_let");
            redCircle = content.Load<Texture2D>("or_let");
            redCross = content.Load<Texture2D>("xr_let");

            // creates a fresh deck
            table = new TableClassic(this, spriteBatch, dragonDrop, tileGrid, p1Board, cpuBoard, gridFont, screenManager, audioManager, 20, 30,
                blueCircle, blueCross, redCircle, redCross);

            table.InitializeTable();

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

            if (table != null)
            {
                table.ResetGame();
            }

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            //if (coveredByOtherScreen)
            //    pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            //else
            //    pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                table.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background.Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                Color.LightSkyBlue, 0, 0);

            tileGrid.Draw(spriteBatch);

            spriteBatch.Begin();
            table.Draw(gameTime);
            spriteBatch.End();


            // If the game is transitioning on or off, fade it out to black.
            //if (TransitionPosition > 0 || pauseAlpha > 0)
            //{
            //    float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

            //    ScreenManager.FadeBackBufferToBlack(alpha);
            //}
        }

        #endregion
    }
}
