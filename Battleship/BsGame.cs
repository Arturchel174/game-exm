using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Battleship.ViewportAdapters;
using GameStateManagement;
using Battleship.Audio;
using Battleship.Screens;

namespace Battleship
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class BsGame : Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        MainMenuScreen mMenuScr;
        ScreenFactory screenFactory;

        AudioManager audioManager;
        
        KeyboardState previousState;
        BoxingViewportAdapter viewport;        
        DragonDrop<IDragAndDropItem> dragonDrop;
        //private bool gameStarted;
        #endregion

        #region Initialization
        public BsGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set the window title
            this.Window.Title = Constants.GAME_TITLE;

            // set the screen resolution
            graphics.PreferredBackBufferWidth = Constants.SCR_WIDTH;
            graphics.PreferredBackBufferHeight = Constants.SCR_HEIGHT;

            this.IsFixedTimeStep = false;  //true
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 25d);

            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Create audio manager component
            audioManager = new AudioManager(this);
            Components.Add(audioManager);
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Add your initialization logic here
            Init();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {            
            dragonDrop = new DragonDrop<IDragAndDropItem>(this, viewport);
            
            Components.Add(dragonDrop);

            // the main menu
            mMenuScr = new MainMenuScreen();
            screenManager.AddScreen(mMenuScr, null);            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
            dragonDrop.Clear();
            Components.Remove(dragonDrop);
            Components.Remove(audioManager);
            Components.Remove(screenManager);
            Services.RemoveService(typeof(IScreenFactory));
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {            
            //table.Update(gameTime);

            base.Update(gameTime);
            HandleKeybInput();            
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            base.Draw(gameTime);
        }

        /// <summary>
        /// This is called when exiting the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            mMenuScr.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles keyboard input from user.
        /// </summary>
        private void HandleKeybInput()
        {
            KeyboardState state = Keyboard.GetState();

            // Show or hide main menu if Escape is pressed.
            if (state.IsKeyDown(Keys.Escape)
                & !previousState.IsKeyDown(Keys.Escape))
            {
                bool mMenuScrPresent = false;
                if (screenManager.GetScreens().Length == 0)
                    screenManager.AddScreen(mMenuScr, null);
                else
                {
                    foreach (GameScreen gs in screenManager.GetScreens())
                    {
                        if (gs.GetType() == typeof(MainMenuScreen))
                        {
                            mMenuScrPresent = true;
                            break;
                        }
                    }
                    if (mMenuScrPresent)
                        screenManager.RemoveScreen(mMenuScr);
                    else
                        screenManager.AddScreen(mMenuScr, null);
                }
            }
            previousState = state;
        }

        /// <summary>
        /// Performs main initialization.
        /// </summary>
        private void Init()
        {            
            viewport = new BoxingViewportAdapter(Window, 
                GraphicsDevice, 
                (int)graphics.PreferredBackBufferWidth, 
                (int)graphics.PreferredBackBufferHeight);

            previousState = Keyboard.GetState();

            IsMouseVisible = true;
        }

        #endregion
    }
}
