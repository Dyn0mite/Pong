#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Microsoft.Xna.Framework.Audio;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        
        /* * * * * * * * * * * TEXTURES * * * * * * * * * * * */

        Texture2D tree;
        Texture2D ground;
        Texture2D bush;
        Texture2D boundingbox; // Debug texture
        Texture2D paddle;
        Texture2D duck;
        Texture2D flyingaway;
        Texture2D laughingdog;
        Texture2D walkingdog;

        /* * * * * * * * * * * * SOUNDS * * * * * * * * * * * */

        SoundEffect dogbark;
        SoundEffect flapwing;
        SoundEffect doglaugh;
        SoundEffect duckquack;
        SoundEffect startround;

        /* * * * * * * * * * * ANIMATIONS * * * * * * * * * */
        
        AnimatedSprite dscduck = new AnimatedSprite();  // Descend Flight Duck
        AnimatedSprite ascduck = new AnimatedSprite(); // Ascend Flight Duck
        AnimatedSprite awayduck = new AnimatedSprite(); // Flyaway Duck
        AnimatedSprite ldog = new AnimatedSprite();     // Laughing Dog Animation
        AnimatedSprite wdog = new AnimatedSprite();     // Walking Dog Animation (with sound)
        AnimatedSprite wdog2 = new AnimatedSprite();    // Walking Dog Animation (without sound)
        AnimatedSprite sdog = new AnimatedSprite();     // Sniff Dog Animation
        AnimatedSprite sprdog = new AnimatedSprite();   // Suprised Dog Animation

        /* * * * * * * * * ANIMATION SCENE * * * * * * * * */

        AnimationScene m_dog_laugh = new AnimationScene();
        AnimationScene m_flyaway = new AnimationScene();
        AnimationScene wscene = new AnimationScene();

        // Needed
        Rectangle boxrec = new Rectangle(224, 64, 832, 512);
        //Start position for duck
        Rectangle startpos;


        /**************************/
        bool m_paused = false;
        bool m_played_intro = false;
        bool m_built_intermission = false;
        bool m_finished_intermission = false;

        Rectangle[] newbound;
        CollisionData[] bounddata;

        Paddle m_player1;
        Paddle m_player2;

        DuckHuntBall m_pongBall;
        AnimationScene m_intro = new AnimationScene();
        
        AnimationManager m_intermission = new AnimationManager();

        /****************************/

        float pauseAlpha;

        #endregion
        
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);       
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            /*  LOAD SOUNDS AND TEXTURES */

            tree = content.Load<Texture2D>("tree");
            ground = content.Load<Texture2D>("ground");
            bush = content.Load<Texture2D>("bush");
            paddle = content.Load<Texture2D>("panel");
            duck = content.Load<Texture2D>("blackduck");
            flyingaway = content.Load<Texture2D>("flyaway");
            laughingdog = content.Load<Texture2D>("laughdog");
            walkingdog = content.Load<Texture2D>("walkdog");
            startround = content.Load<SoundEffect>("startround");
            doglaugh = content.Load<SoundEffect>("doglaugh");
            //flapwing = content.Load<SoundEffect>("wingflaps");
            //duckquack = content.Load<SoundEffect>("quack");

            /* SETUP ALL ANIMATIONSPRITES */

            dscduck.BuildAnimation(duck, 1, 9, true, new int[16] {3, 4, 5, 4, 3, 4, 5, 4, 3, 4, 5, 4, 3, 4, 5, 4});
            dscduck.SetFrame(0, 5, flapwing);
            dscduck.SetFrame(1, 5, null);
            dscduck.SetFrame(2, 5, flapwing);
            dscduck.SetFrame(3, 5, null);
            dscduck.SetFrame(4, 5, flapwing);
            dscduck.SetFrame(5, 5, null);
            dscduck.SetFrame(6, 5, flapwing);
            dscduck.SetFrame(7, 5, null);
            dscduck.SetFrame(8, 5, flapwing); //duckquack);
            dscduck.SetFrame(9, 5, null);
            dscduck.SetFrame(10, 5, flapwing);
            dscduck.SetFrame(11, 5, null);
            dscduck.SetFrame(12, 5, flapwing);
            dscduck.SetFrame(13, 5, null);
            dscduck.SetFrame(14, 5, flapwing);
            dscduck.SetFrame(15, 5, null);

            ascduck.BuildAnimation(duck, 1, 9, true, new int[16] {0, 1, 2, 1, 0, 1 , 2, 1, 0, 1, 2, 1, 0, 1, 2, 1});
            ascduck.SetFrame(0, 5, null);
            ascduck.SetFrame(1, 5, flapwing);
            ascduck.SetFrame(2, 5, null);
            ascduck.SetFrame(3, 5, flapwing);
            ascduck.SetFrame(4, 5, null);
            ascduck.SetFrame(5, 5, flapwing);
            ascduck.SetFrame(6, 5, null);
            ascduck.SetFrame(7, 5, flapwing);
            ascduck.SetFrame(8, 5, null); //duckquack);
            ascduck.SetFrame(9, 5, flapwing);
            ascduck.SetFrame(10, 5, null);
            ascduck.SetFrame(11, 5, flapwing);
            ascduck.SetFrame(12, 5, null);
            ascduck.SetFrame(13, 5, flapwing);
            ascduck.SetFrame(14, 5, null);
            ascduck.SetFrame(15, 5, flapwing);

            awayduck.BuildAnimation(flyingaway, 1, 3, true, new int[4] { 0, 1, 2, 1 });
            awayduck.SetFrame(0, 8, null);
            awayduck.SetFrame(1, 8, null);
            awayduck.SetFrame(2, 8, null);
            awayduck.SetFrame(3, 8, null);

            ldog.BuildAnimation(laughingdog, 1, 2, true, new int[2] { 0, 1 });
            ldog.SetFrame(0, 8, null);
            ldog.SetFrame(1, 8, null);
           
            sprdog.BuildAnimation(walkingdog, 1, 8, true, new int[1] { 5 });
            sprdog.SetFrame(0, 100, null);

            wdog.BuildAnimation(walkingdog, 1, 8, true, new int[4] { 1, 2, 3, 4 });
            wdog.SetFrame(0, 8, null);
            wdog.SetFrame(1, 8, null);
            wdog.SetFrame(2, 8, null);
            wdog.SetFrame(3, 8, null);
            wdog.X_Pos = 150;
            wdog.Y_Pos = 550;
            
            sdog.BuildAnimation(walkingdog, 1, 8, true, new int[6] { 1, 0, 1, 0, 1, 0 });
            sdog.SetFrame(0, 8, null);
            sdog.SetFrame(1, 8, null);
            sdog.SetFrame(2, 8, null);
            sdog.SetFrame(3, 8, null);
            sdog.SetFrame(4, 8, null);
            sdog.SetFrame(5, 8, null);

            /*   INTRO ANIMATION MADE */
            m_intro.AddAnimation(new DirXYMover(wdog, 180, 0, 1.7f));
            m_intro.AddAnimation(new TimeOutDrawable(sdog, 49));
            m_intro.AddAnimation(new DirXYMover(wdog, 180, 0, 1.7f));
            m_intro.AddAnimation(new TimeOutDrawable(sdog, 49));
            m_intro.AddAnimation(new DirXYMover(wdog, 10, 0, 1.7f));
            m_intro.AddAnimation(new TimeOutDrawable(sprdog, 49));
            m_intro.BuildScene(new int[6] { 0, 1, 2, 3, 4, 5 });
            m_intro.Scene_State = DrawableState.Active;

            /* Intermission Animation */
            m_dog_laugh.AddAnimation(new DirXYMover(ldog, 0, -47, 1.0f), doglaugh);
            m_dog_laugh.AddAnimation(new TimeOutDrawable(ldog, 60));
            m_dog_laugh.AddAnimation(new DirXYMover(ldog, 0, 47, 1.0f));

            boundingbox = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            boundingbox.SetData(new Color[] { Color.White });

            startpos = new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2, 564, 1, 1);

            m_player1 = new Paddle(1, paddle, boxrec);
            m_player2 = new Paddle(0, paddle, boxrec);

            newbound = Boundary.CreateBoundRects(boxrec);
            bounddata = new CollisionData[newbound.Length];

            bounddata[0] = new CollisionData(newbound[0]);
            bounddata[1] = new CollisionData(newbound[1]);
            bounddata[2] = new CollisionData(newbound[2]);
            bounddata[3] = new CollisionData(newbound[3]);
            bounddata[4] = new CollisionData(newbound[4]);
            bounddata[5] = new CollisionData(newbound[5]);

            /* Setup Pong Ball */
            m_pongBall = new DuckHuntBall(startpos.X, 540, bounddata);
            m_pongBall.AddAnimation(dscduck);
            m_pongBall.AddAnimation(ascduck);
            m_pongBall.Init_Vel_Mag = 5.0f; // Must reset ball if you edit the magnitude;
            m_pongBall.ResetBall();
            if ((m_pongBall.X_Vel > 0 && m_pongBall.Y_Vel < 0) || (m_pongBall.X_Vel < 0 && m_pongBall.Y_Vel < 0))
            {
                m_pongBall.SetCurrentAnimation(1);
            }
            else
            {
                m_pongBall.SetCurrentAnimation(0);
            }

            //m_pongBall.X_Vel = 1;
            //    m_pongBall.Y_Vel = -1;
            
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (!m_paused)
            {   
                if(m_intro.Scene_State != DrawableState.Finished) {
                    if (!m_played_intro)
                    {
                        startround.Play();
                        m_played_intro = true;
                    }
                    m_intro.Update();  
                }
                else if (m_intro.Scene_State == DrawableState.Finished && 
                         m_pongBall.Ball_State != BallState.DeadBall)
                {
                    m_pongBall.Update(m_player1.Texture_Data, m_player2.Texture_Data);
                }
         
                else if (m_pongBall.Ball_State == BallState.DeadBall &&
                         !m_finished_intermission && 
                         m_intro.Scene_State == DrawableState.Finished)
                {
                    if (!m_built_intermission)
                    {
                        /* Flyaway Duck */
                        m_flyaway.Clear();
                        awayduck.X_Pos = m_pongBall.X_Pos;
                        awayduck.Y_Pos = m_pongBall.Y_Pos;
                        m_flyaway.AddAnimation(new DirXYMover(awayduck, 0, -(m_pongBall.Y_Pos - 32), m_pongBall.Curr_Vel_Mag));
                        m_flyaway.BuildScene(new int[1] { 0 });
                        m_flyaway.Scene_State = DrawableState.Active;
                          
                        /* Laughing Dog */
                        ldog.X_Pos = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                        ldog.Y_Pos = 530;
                        m_dog_laugh.BuildScene(new int[3] { 0, 1, 2 });
                        m_dog_laugh.Scene_State = DrawableState.Active;

                        /* Intermission */
                        m_intermission.AddScene(m_flyaway);
                        m_intermission.AddScene(m_dog_laugh);
                        m_intermission.Build(new int[2] { 0, 1 });

                        // Change
                        m_intermission.Manager_State = DrawableState.Active;
                       
                        // Built intermission scene
                        m_built_intermission = true;
                    }

                    m_intermission.Update();

                    if (m_intermission.Manager_State == DrawableState.Finished) 
                    {
                        m_finished_intermission = true;
                    }
                }
                else if (m_intermission.Manager_State == DrawableState.Finished && 
                         m_pongBall.Ball_State == BallState.DeadBall)
                {
                    m_pongBall.ResetBall();
                    m_intermission.ClearList();
                    m_built_intermission = false;
                    m_finished_intermission = false;
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            //int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState1 = input.CurrentKeyboardStates[0];
            GamePadState gamePadState1 = input.CurrentGamePadStates[0];
            KeyboardState keyboardState2 = input.CurrentKeyboardStates[1];
            GamePadState gamePadState2 = input.CurrentGamePadStates[1];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState1.IsConnected &&
                                       input.GamePadWasConnected[0] && 
                                       !gamePadState2.IsConnected &&
                                       input.GamePadWasConnected[1];

            if (input.IsPauseGame(null) || gamePadDisconnected)
            {
                m_paused = true;
                ScreenManager.AddScreen(new PauseMenuScreen(), null);
            }

            else if (m_pongBall.Ball_State != BallState.OutofBounds && m_intro.Scene_State == DrawableState.Finished)
            {
                m_paused = false;
                m_player1.Update(keyboardState1, gamePadState1);
                m_player2.Update(keyboardState2, gamePadState2);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(new Color(49, 192, 250, 255));
            /*
            if (m_pongBall.Ball_State == BallState.DeadBall &&
                         !m_finished_intermission &&
                         m_intro.Scene_State == DrawableState.Finished)
                ScreenManager.GraphicsDevice.Clear(Color.White);
            */
            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            //spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            // Draw tree
            spriteBatch.Draw(tree, new Vector2(250, 300), null, Color.White, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
            
            // Draw Bush
            spriteBatch.Draw(bush, new Vector2(950, 420), null, Color.White, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);

            // Draw Duck(ball)
            //m_pongBall.Draw(spriteBatch);

            // Draw Intermission
            m_intermission.Draw(spriteBatch);

            // Draw left ground + grass
            spriteBatch.Draw(ground, new Vector2(128, 500), null, Color.White, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);

            // Draw right ground + grass
            spriteBatch.Draw(ground, new Vector2(640, 500), null, Color.White, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);

            // Draw Paddles 
            m_player1.Draw(spriteBatch);
            m_player2.Draw(spriteBatch);

            // Draw Intro
            m_intro.Draw(spriteBatch);

            m_pongBall.Draw(spriteBatch);

            spriteBatch.Draw(boundingbox, boxrec, new Color(0, 0, 0, 128));

            //spriteBatch.Draw(boundingbox, startpos, Color.Beige);

            //spriteBatch.Draw(boundingbox, newbound[0], Color.Red);

            //spriteBatch.Draw(boundingbox, newbound[1], Color.Purple);

            //spriteBatch.Draw(boundingbox, newbound[2], Color.White);

            //spriteBatch.Draw(boundingbox, newbound[3], Color.White);

            //spriteBatch.Draw(boundingbox, newbound[4], Color.White);

            //spriteBatch.Draw(boundingbox, newbound[5], Color.White);

            

            //wdog.Draw(spriteBatch);
            //sdog.Draw(spriteBatch);
            //ltest.Draw(spriteBatch);
            //wscene.Draw(spriteBatch);
            //spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);

            //spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
            //                       enemyPosition, Color.DarkRed);

            //dscduck.Draw(spriteBatch);
            

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}