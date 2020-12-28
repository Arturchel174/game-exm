using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Battleship.Screens
{
    class MainMenuScreen : MenuScreen
    {
        #region Fields

        GameplayScreen gpScreen;
        MenuEntry playGameMenuEntry;
        //MenuEntry optionsMenuEntry;
        MenuEntry exitMenuEntry;
        Texture2D gradientTexture;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
            gpScreen = new GameplayScreen();

            // Create our menu entries.
            playGameMenuEntry = new MenuEntry("Single Game");
            //optionsMenuEntry = new MenuEntry("Options");
            exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            //optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += ConfirmExitMessageBoxAccepted; //OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            //MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                ContentManager content = ScreenManager.Game.Content;
                gradientTexture = content.Load<Texture2D>("gradient");
            }
        }

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (gpScreen.ScreenManager != null)
            {
                gpScreen.Unload();
            }
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               gpScreen);
            ScreenManager.RemoveScreen(this);
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {            

            /*
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
            */
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            //Unload();
            ScreenManager.Game.Exit();
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {

            //    SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //    SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            //    // Center the message text in the viewport.
            //    Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            //    Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            //    Vector2 textSize = font.MeasureString("  HOLA            ");
            //    Vector2 textPosition = (viewportSize - textSize) / 2;

            //    // The background includes a border somewhat larger than the text itself.
            //    const int hPad = 32;
            //    const int vPad = 16;

            //    Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
            //                                                  (int)textPosition.Y - vPad,
            //                                                  (int)textSize.X + hPad * 2,
            //                                                  (int)textSize.Y + vPad * 2);

            //    spriteBatch.Begin();

            //    // Draw the background rectangle.
            //    spriteBatch.Draw(gradientTexture, backgroundRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            //    spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Public Methods

        public void Dispose()
        {
            playGameMenuEntry.Selected -= PlayGameMenuEntrySelected;
            exitMenuEntry.Selected -= ConfirmExitMessageBoxAccepted;
        }

        //public override void Unload()
        //{
        //playGameMenuEntry.Selected -= PlayGameMenuEntrySelected;
        //exitMenuEntry.Selected -= ConfirmExitMessageBoxAccepted;
        //}

        #endregion
    }
}
