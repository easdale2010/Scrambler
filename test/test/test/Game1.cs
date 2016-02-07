using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using gamelib2d;


namespace test
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int displaywidth = 800;
        int displayheight = 480;
        int lives;
        Boolean gameover;
        int score;
        Boolean gunbuttonreleased2 = true; // varible used to control the speed the player can fire upwards
        int highscore;
        Random randomiser = new Random();
        Vector3 spawnpoint;
        int spawncooldown;
        float sspawncooldown; // varaible used to control the spider spawns cool down
        float ssspawncooldown; // variable used to control the scorpion spawn cool down
        int spiderlives ;
        int scorpionlives = 20;
        float playerguncooldown =0;
        SoundEffect song;
        SoundEffectInstance music;
        SoundEffect goodfire, badfire,gooddeath,spiderfalls;
        float vibration;
        float invunerabletime;
        Boolean bullet2visible = true;
        float spiderwebcooldown = 20000; // varible  used to control the speed the spider can fire its webbing
        float backgroundcount;
       // Boolean enemyspawned = false;

        Boolean gunbuttonreleased = true; // varible used to control the speed the player can fire to the right

        struct sprites2d // strusture for creating proerties graphics
        {
            public Rectangle rect;
            public Texture2D image;
            public BoundingBox bbox;
            public BoundingSphere bsphere;
            public Vector3 position;
            public Vector3 oldposition;
            public Vector3 velocity;
            public Vector2 origin;
            public float size;
            public float rotation;
            public Boolean visible;
            public float power;
            public Boolean spawned;

            // Use this method to draw the image at a specified position
            public void drawme(ref SpriteBatch sbatch)// meathod used for drawing sprits that are visible
            {
                if (visible)
                {
                     sbatch.Draw(image, rect, null, Color.White, rotation, origin, SpriteEffects.None, 0);
                }
            }

            public void drawmeflip(ref SpriteBatch sbatch)// method used for drawing backgrounds with spriteeffects
            {
                    sbatch.Draw(image, rect, null, Color.White, rotation, origin, SpriteEffects.FlipHorizontally, 0);
            }

           

            public void drawme2(ref SpriteBatch sbatch)// method used for drawing backgrounds
            {
                sbatch.Draw(image,rect, Color.White);
            }
        }


        animation explosion1, explosion2,goodexplosion, spiderdeath;
        sprites2d background, ship, spider, background1,gameoverimage,scorpion,scorpionsting; // graphics used in the game
        sprites2d []playerbullet2 = new sprites2d[8];// Array of bullets for the player to fire upwards
        sprites2d[] playerbullet = new sprites2d[5]; // Array of bullets for the player to fire to the right
       
        sprites2d[] enemyship = new sprites2d[40]; // array of enemy ships
        sprites2d[] enemyship2 = new sprites2d[10]; // 2nd array of enemy ships
        sprites2d[] enemybullet = new sprites2d[40]; // array of enemy bullets
        sprites2d[] enemybullet2 = new sprites2d[10];// 2nd array of enemy bullets
        sprites2d[] spiderweb = new sprites2d[8];// array of spiders webs
       
        SpriteFont mainfont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = displaywidth; // set the screen width to the displaywidth varible
            this.graphics.PreferredBackBufferHeight = displayheight; // set the screen width to the displayheight varible
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        void loadsprites(ref sprites2d sprite, string spritename, int x, int y, float msize) // creats void to for loading the smaller images
        {
            sprite.image = Content.Load<Texture2D>(spritename);
            sprite.position = new Vector3((float)x, (float)y, 0);
            sprite.size = msize; 
            sprite.rect.Y = y;
            sprite.rect.X = x;
            sprite.rect.Width = (int)(sprite.image.Width * msize);
            sprite.rect.Height = (int)(sprite.image.Height * msize);
            sprite.origin.Y = sprite.image.Height / 2;
            sprite.origin.X = sprite.image.Width / 2;
        }

        void loadbackgrounds(ref sprites2d bsprites, string sname, int x, int y, float bsize) // creats a void for loading bcckgrounds
        {
            bsprites.image = Content.Load<Texture2D>(sname); // loads the structures textures into the texture sname
            bsprites.position = new Vector3((float)x, (float)y, 0);
            bsprites.size = bsize; // loads the size varible into to bsize
            bsprites.rect.Y = y;
            bsprites.rect.X = x;
            bsprites.rect.Width = displaywidth; // sets the size of the image to the size of the displaywidth
            bsprites.rect.Height = displayheight;  // sets the size of the image to the size of the displayheight
            bsprites.origin.Y = bsprites.image.Height / 2; // sets the Y origin to half the size of images height
            bsprites.origin.X = bsprites.image.Width / 2; // sets the X origin to half the size of images width


        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (File.Exists(@"highscore.txt")) // load in high score file
            {
                string line;
                StreamReader sr = new StreamReader(@"highscore.txt");
                line = sr.ReadLine();// loads sr into th evarible line
                sr.Close(); // closes the stream reader
                line = line.Trim();// trimss any excess white space
                highscore = Convert.ToInt32(line);// converts string to int 
            }

            loadbackgrounds(ref background, "backgroundcave", 0, 0, 0); // loads the background image position and size based on the background void
            background.velocity = new Vector3(-1, 0, 0); // moves the background from right to  left

            loadsprites(ref scorpionsting, "splat", 0, 0, 0.2f);// loads the scorpoion image and its position and its size 
            scorpionsting.velocity = new Vector3(-5, 0, 0);// creats a velocity for the spider to attack from above

            background1.image = Content.Load<Texture2D>("backgroundcave");
            background1.position.Y = 0; // poitions background at the top of the screen
            background1.position.X = displaywidth; // poitions background at the left of the screen
            background1.rect.Width = displaywidth; // sets the backgrounds size to the size of displaywidth
            background1.rect.Height = displayheight; // sets the backgrounds size to the size of displayheight
            background1.velocity = new Vector3(-1, 0, 0); // moves the background from right to  left


            loadsprites(ref gameoverimage, "gameoverimage", 0, 0, 1);// loads th egameoverimges image , positionand size;
     

            spiderfalls = Content.Load<SoundEffect>("crisps walls"); //load sound effect for enemys


            loadsprites(ref ship, "spaceship", 0, 0, 0.25f); // loads  based on the loadsprites
            ship.power = 1.5f; // gives the player ship some power

            loadsprites(ref spider, "spider", randomiser.Next(400) + 80, -30, 1); // loads  based on the loadsprites
            spider.velocity = new Vector3(0, 4, 0); // sets the velocity of which the spdier should attack at

            for (int y = 0; y < playerbullet.Count(); y++) // creats a for loop that will run playerbullet amout of time (see varible)
            {
                loadsprites(ref playerbullet[y], "bullet2", 0, 0, 0.1f); // loads  based on the loadsprites
                playerbullet[y].velocity = new Vector3(5, 0, 0); // sets the velocity of the player bullet
            }

            for (int y = 0; y < enemybullet.Count(); y++) // creats a for loop that will run enemybullet amout of time (see varible)
            {
                loadsprites(ref enemybullet[y], "bullet", 0, 0, 0.2f); // loads  based on the loadsprites
                enemybullet[y].velocity = new Vector3(-5, 0, 0); // sets the velocity of the first enemys bullet
            }
            for (int y = 0; y < enemybullet2.Count(); y++)  // creats a for loop that will run enemybullet2 amout of time (see varible)
            {
                loadsprites(ref enemybullet2[y], "bullet3", 0, 0, 0.1f); // loads  based on the loadsprites
                enemybullet2[y].velocity = new Vector3(-5, 0, 0); // sets the velocity of the 2nd enemys bullet
            }

            for (int y = 0; y < enemyship.Count(); y++) // creats a for loop that will run enemyship amout of time (see varible)
            {
                loadsprites(ref enemyship[y], "enemyship1", 0, 0, 0.5f); // loads  based on the loadsprites
               

            }


            for (int y = 0; y < enemyship2.Count(); y++) // creats a for loop that will run enemyship2 amout of time (see varible)
            {
                loadsprites(ref enemyship2[y], "enemyship2", 0, 0, 1f); // loads  based on the loadsprites
    

            }

            for (int y = 0; y < playerbullet2.Count(); y++) // creats a for loop that will run playerbullet2 amout of time (see varible)
            {
                playerbullet2[y].velocity = new Vector3(0, -10, 0); // loads  based on the loadsprites
                loadsprites(ref playerbullet2[y], "bulletup", 0, 0, 0.15f); // sets the velocity of the players 2nd bullet

            }

            loadsprites(ref scorpion, "scorpion", randomiser.Next(400) + 30, 530, 0.15f); // loads  based on the loadsprites
            scorpion.velocity = new Vector3(0, -15, 0); // sets the velocity of the scorpion 





            for (int y = 0; y < spiderweb.Count(); y++) // creats a for loop that will run spiderweb amout of time (see varible)
            {
                spiderweb[y].velocity = new Vector3(0, 10, 0); // load spider web into the game
                loadsprites(ref spiderweb[y], "spiderweb", 0, 0, 0.1f); // setes the spider web velocity
            }

            explosion1 = new animation(Content, "explosion", 0, 0, 0.5f, Color.White, false, 24, 1, 6, false, false, false); // loads animation
            explosion2 = new animation(Content, "cast_003", 0, 0, 0.5f, Color.White, false, 24, 4, 5, false, false, false); // loads animation
            goodexplosion = new animation(Content, "darkness_001", 0, 0, 0.2f, Color.White, false, 24, 5, 6, false, false, false); // loads animation
            spiderdeath = new animation(Content, "explosion2", 0, 0, 0.5f, Color.White, false, 24, 2, 5, false, false, false); // loads animation

            mainfont = Content.Load<SpriteFont>("font"); // load font 
            song = Content.Load<SoundEffect>("superfly"); // load sound track into game
            music = song.CreateInstance();// allow for sound trck to be played
            gooddeath = Content.Load<SoundEffect>("goodguylaserdeath"); //load sound effect for player death
            goodfire = Content.Load<SoundEffect>("laserfire"); //load sound effect for players guns
            badfire = Content.Load<SoundEffect>("laserfire2"); //load sound effect for enemys guns
            resetgame();
            resetship();
        }

        void resetship() // creats a void the will spawn the ship after it has died
        {
            ship.position = new Vector3(30, displayheight / 2, 0);// spawns the ship off screen
            ship.visible = true;// makes the ship visible
            invunerabletime = 3000;// makes the invincible for a short period

        }

        void resetspider() // creats a void the will spawn the spider until it dies
        {
            if (spiderlives >= 1)
            {
                spider.position.X = randomiser.Next(displaywidth /2 - 160) + 160;// uses the randomiser to spawn the spiders x position
                spider.visible = true;// makes the spider visible
                spider.velocity = new Vector3(0, 4, 0); // moves the spider down wards
                spiderfalls.Play();// playes sound effect
          
            }
        }
        void resetscorpion() // creats a void the will spawn the scorpion until the  dies
        {
            if (spiderlives >= 1)// only runs if the spider has livs remaining
            {
                scorpion.position.Y = 500;// sets the scorpions position just off the bottom of the screen
                scorpion.position.X =displaywidth / 2 + 16;// spawns the scorpion just after the half way mark 
                scorpion.visible = true;// makes the scorpion visible
                scorpion.velocity = new Vector3(0, -4, 0);// give the scorpion some atacking speed
             
            }

        }

        void resetlevel() // creats a void for resttin gthe game to give it replay ablity
        {
            

                for (int y = 0; y < enemyship.Count(); y++)// creats a for loop based on the amount of enemy ships
                    if (!enemyship[y].visible)
                    {

                        enemyship[y].position.X = randomiser.Next(4000) + 820;// spawns the enemy ship in random  X places off the screen
                        enemyship[y].position.Y = (randomiser.Next(390) + 10);// spawns enemy ships in random Y places off the screen
                        enemyship[y].visible = true;// makes enemy ship visible when they have been spanwed
                        enemyship[y].velocity = new Vector3(-2, 0, 0); // sets the velocity of the enemyship
                       // enemyspawned = true;

                    }
                for (int y = 0; y < enemyship2.Count(); y++)// creats a for loop based on the amount of enemyship2
                    if (!enemyship2[y].visible || !enemyship2[y].spawned)
                    {
                        enemyship2[y].position.X = randomiser.Next(8000) + 4000;// spawns the enmysship2 in random X positions after the wave of enemyship 
                        enemyship2[y].position.Y = randomiser.Next(390) + 10;// spawns the enmysship2 in random Y positions after the wave of enemyship 
                        enemyship2[y].visible = true;// makes enemyship2 visible when it has been spawnded
                        enemyship2[y].velocity = new Vector3(-2, 0, 0); // sets the velocity of the enemy2 ship
                        //enemyspawned = true;

                    }
            
            spiderlives = 35;
            //resetship(); // loads in reset ship

        }


        void resetgame() // set game to the beginnings 
        {
            gameover = false;// restarts the game

            resetlevel();// resets the level
            lives = 3;// gives the player lives
            score = 0;// rests the score to 0
        }


      
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            StreamWriter sw = new StreamWriter(@"highscore.txt");// unloads highscore
            sw.WriteLine(highscore.ToString());// wites line to high score file
            sw.Close();// closes new stream writer
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            GamePadState[] pad = new GamePadState[1];// creats an arrray of game pads
            pad[0] = GamePad.GetState(PlayerIndex.One);// allows the player to control the ship with game pad
            const float friction = 0.75f; // creats a variable for friction
            float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds; // creats a varible based on the amount of time the game has been running
            spiderwebcooldown -= timebetweenupdates;// count down for spider wweb ability to be refired;

            if (!gameover) // makes the program run only when the game is not  over
            {
                invunerabletime -= timebetweenupdates;// counts down player invunerable time 

                if (pad[0].Buttons.Back == ButtonState.Pressed)// makes the game over if the back button is pressed
                    gameover = true;

                if (lives <= 0)// make the game over if the player has no lives
                {
                    gameover = true;//makes the game over
                }

                if (music.State == SoundState.Stopped)// plays the background music
                    music.Play();

                background.position += background.velocity; // adds background velocity to the background position;
                background.rect.Y = (int)background.position.Y;
                background.rect.X = (int)background.position.X;

                background1.position += background1.velocity; // adds the background velocity to backgrounds postion so it can moves
                background1.rect.Y = (int)background1.position.Y;// set the rect position to the backgrounds position;
                background1.rect.X = (int)background1.position.X;

                if (background.rect.Right < 0)
                    background.position.X = background1.rect.Right; // set the backgrounds left position to bacground1 right position if it moves off the screen;
                if (background1.rect.Right < 0)
                    background1.position.X = background.rect.Right; // set the background1s left position to bacground right position if it moves off the screen;

                sspawncooldown -= timebetweenupdates; // makes a count down timer for the scorpion to attack

                spider.position += spider.velocity; // adds the spiders velocity to the position
                spider.rect.Y = (int)spider.position.Y;// set the rect position to the backgrounds position;
                spider.rect.X = (int)spider.position.X;

                spider.bbox = new BoundingBox(new Vector3(spider.position.X - spider.rect.Width / 2, spider.position.Y - spider.rect.Height / 2, 0),
                    new Vector3(spider.position.X + spider.rect.Width / 2, spider.position.Y + spider.rect.Height / 2, 0)); // creates a bounding box for the spider

                if (spider.position.Y > displayheight / 2)// asks if spider position is more than the displayheight /2;
                {
                    spider.position.Y = displayheight / 2; // makes spider stop half way
                    spider.velocity.Y = -spider.velocity.Y; // reverses the spider velocity
                }

                if (spider.position.Y <= -100) // asks if the speder position is less than -100
                {
                    sspawncooldown = 3000;// sets makes the spider wait before it can attack again
                    spider.position.Y = -99; // give the spider a new position
                    spider.velocity.Y = 0;// stop the spider from moving 
                }


                for (int b = 0; b < enemyship2.Count(); b++)
                    for (int y = 0; y < playerbullet2.Count(); y++)
                        if (sspawncooldown <= 0 && spider.velocity.Y == 0 && enemyship2[0].position.X + enemyship2[0].rect.Width / 2 < 0
                            && enemyship2[1].position.X + enemyship2[1].rect.Width / 2 < 0 && enemyship2[2].position.X + enemyship2[2].rect.Width / 2 < 0
                            && enemyship2[3].position.X + enemyship2[3].rect.Width / 2 < 0 && enemyship2[4].position.X + enemyship2[4].rect.Width / 2 < 0
                            && enemyship2[5].position.X + enemyship2[5].rect.Width / 2 < 0 && enemyship2[6].position.X + enemyship2[6].rect.Width / 2 < 0
                            && enemyship2[5].position.X + enemyship2[7].rect.Width / 2 < 0 && enemyship2[8].position.X + enemyship2[8].rect.Width / 2 < 0
                            && enemyship2[9].position.X + enemyship2[9].rect.Width / 2 < 0)// asking if all enemy ships have left the screen and if they have it calls reset spider
                        {
                            resetspider();

                        }

                ship.velocity.Y -= pad[0].ThumbSticks.Left.Y * ship.power;// gives the player control of ship
                ship.velocity.X += pad[0].ThumbSticks.Left.X * ship.power; // gives the player control of ship
                ship.position += ship.velocity;// makes the player use velocity to control the ship
                ship.velocity *= friction;// adds  friction to the velocity;

                if (ship.position.X > displaywidth / 2 - ship.rect.Width / 2)// make the ship stop half way along the x-axis
                {
                    ship.position.X = displaywidth / 2 - ship.rect.Width / 2;
                    ship.velocity.X = 0;
                }
                if (ship.position.X < ship.rect.Width / 2)// stops the ship leaving the screen on the x-axis
                {
                    ship.position.X = ship.rect.Width / 2;
                    ship.velocity.X = 0;
                }

                if (ship.position.Y < ship.rect.Height / 2)// stops the ship leaving the screen on the y-axis
                {
                    ship.position.Y = ship.rect.Height / 2;
                    ship.velocity.Y = 0;
                }
                if (ship.position.Y > displayheight - ship.rect.Height / 2)// stops the ship leaving the screen on the x-axis
                {
                    ship.position.Y = displayheight - ship.rect.Height / 2;
                    ship.velocity.Y = 0;
                }

                ship.rect.Y = (int)ship.position.Y;// sets the ship the rect position to the ship position
                ship.rect.X = (int)ship.position.X;

                ship.bbox = new BoundingBox(new Vector3(ship.position.X - ship.rect.Width / 2, ship.position.Y - ship.rect.Height / 2, 0),
                    new Vector3(ship.position.X + ship.rect.Width / 2, ship.position.Y - ship.rect.Height / 2, 0));// creats a bounding box for the player ship

                playerguncooldown -= timebetweenupdates;// creats a timer for the player gun

                for (int y = 0; y < playerbullet.Count(); y++)// creats a for loop based on the amount of player bullets
                {
                    if (pad[0].Buttons.A == ButtonState.Pressed && !playerbullet[y].visible && playerguncooldown <= 0 && gunbuttonreleased)// fires a player bullet based on the button being pressed and timer being at 0 and the button is released and one is not bieng fired
                    {
                        gunbuttonreleased = false;// resets the gun so the player can fire again
                        playerbullet[y].position = ship.position;// makes the staring point of the bullet the ships position
                        playerbullet[y].visible = true;// makes the bullet visible
                        goodfire.Play();// playes the sound effect for the player bullet
                        playerguncooldown = 50;// make the player wait before he can refire
                        //  vibration = 1;
                    }

                    if (playerbullet[y].visible)// only runs if the player bulet is visible
                    {
                        playerbullet[y].position += playerbullet[y].velocity;// adds the player bullets velocity to player bulet position
                        playerbullet[y].rect.X = (int)playerbullet[y].position.X;// set the player bullet rectangle to the player bullet position
                        playerbullet[y].rect.Y = (int)playerbullet[y].position.Y;

                        playerbullet[y].bbox = new BoundingBox(new Vector3(playerbullet[y].position.X - playerbullet[y].rect.Width / 2, playerbullet[y].position.Y - playerbullet[y].rect.Height / 2, 0),
                   new Vector3(playerbullet[y].position.X + playerbullet[y].rect.Width / 2, playerbullet[y].position.Y + playerbullet[y].rect.Height / 2, 0));// creats a bounding box for the player bullet

                        if (playerbullet[y].position.X > displaywidth + playerbullet[y].rect.Width / 2)// makes  the player bullet invisible if leaves the screen
                        {
                            playerbullet[y].visible = false;
                            score--;// if the player misses with a shot a point is taken away
                        }
                    }
                }

                if (pad[0].Buttons.A == ButtonState.Released)// cheaks that the player has released the a button bofore the player can refire
                    gunbuttonreleased = true;

                for (int b = 0; b < enemyship.Count(); b++)// creats for loop based on the amount of enemyships
                {

                    enemyship[b].position += enemyship[b].velocity;// adds enemyship velcoity to enemyship position;
                    enemyship[b].rect.Y = (int)enemyship[b].position.Y;// set the enemyship rectangle to enemyship position
                    enemyship[b].rect.X = (int)enemyship[b].position.X;

                    enemyship[b].bbox = new BoundingBox(new Vector3(enemyship[b].position.X - enemyship[b].rect.Width / 2, enemyship[b].position.Y - enemyship[b].rect.Height / 2, 0),
                    new Vector3(enemyship[b].position.X + enemyship[b].rect.Width / 2, enemyship[b].position.Y + enemyship[b].rect.Height / 2, 0));// creats a boungind box for the enemy ship


                    for (int y = 0; y < playerbullet.Count(); y++)// creats a for loop based on the amount of player bullets
                    {
                        if (playerbullet[y].visible && playerbullet[y].bbox.Intersects(enemyship[b].bbox) && enemyship[b].visible)// cheack for collision between the player bullet and  the enemyship
                        {
                            enemyship[b].visible = false;// makes the enemy ship in visible if it get hit by the player bullet
                            playerbullet[y].visible = false;// make the player bullet invisible if it hits the enemy ship
                            score += 10;// gives the player 10 points
                            explosion1.start(enemyship[b].position);// creats an animation where the ship got hit
                            vibration = 1f;// adds 1 to the vibration varible to be used in the virbation motors

                        }
                    }
                }



                explosion1.update(timebetweenupdates);// allows the animation to play frame by frame
                explosion2.update(timebetweenupdates);// allows the animation to play frame by frame
                goodexplosion.update(timebetweenupdates);// allows the animation to play frame by frame
                spiderdeath.update(timebetweenupdates);// allows the animation to play frame by frame



                for (int b = 0; b < enemyship2.Count(); b++)// creats a for loop based the the amount of enemyship2's
                {

                    enemyship2[b].position += enemyship2[b].velocity;// adds enemy shp2 velocity to enemy ship2 position
                    enemyship2[b].rect.Y = (int)enemyship2[b].position.Y;// sets the rect position the position
                    enemyship2[b].rect.X = (int)enemyship2[b].position.X;

                    enemyship2[b].bbox = new BoundingBox(new Vector3(enemyship2[b].position.X - enemyship2[b].rect.Width / 2, enemyship2[b].position.Y - enemyship2[b].rect.Height / 2, 0),
new Vector3(enemyship2[b].position.X + enemyship2[b].rect.Width / 2, enemyship2[b].position.Y + enemyship2[b].rect.Height / 2, 0));// creats a bounding box for enemy ship2

                    for (int y = 0; y < playerbullet.Count(); y++)// creats a for loop based on the amount of player bullets
                    {

                        if (playerbullet[y].visible && playerbullet[y].bbox.Intersects(enemyship2[b].bbox) && enemyship2[b].visible)// ask if the player bullet hits the enemy ship 2 
                        {
                            enemyship2[b].visible = false;// makes enemy ship 2 invisible
                            playerbullet[y].visible = false;// makes player bullet invisible
                            score += 20;// gives the player points
                            explosion2.start(enemyship2[b].position);// plays animation
                            vibration = 1f;// adds one to virbation varaible to be use in vibration motors

                        }
                    }


                }


                for (int b = 0; b < enemyship.Count(); b++)// creates a for loop based on the amount of enemy ship
                {
                    if (enemyship[b].position.X - enemyship[b].rect.Width / 2 <= 0)// set enemy ship to in visible if it leaves the left had side of the screen
                        enemyship[b].visible = false;

                    if (enemyship[b].visible && enemyship[b].position.Y - enemyship[b].rect.Height / 2 < ship.position.Y && enemyship[b].position.Y + enemyship[b].rect.Height / 2 > ship.position.Y && enemyship[b].position.X - enemyship[b].rect.Width / 2 < displaywidth)
                        if (!enemybullet[b].visible)// fires enemy bullet when player is level wi th enemy ship 
                        {
                            enemybullet[b].visible = true;// make senemy bullet visile
                            enemybullet[b].position = enemyship[b].position;// make the enemy bullet starting position the enemyships position
                            badfire.Play();// plays a sound effect

                        }


                    if (enemybullet[b].visible)// runs if the enemy bullet is visible
                    {
                        enemybullet[b].position += enemybullet[b].velocity;// adds enemy bullet velocity to enemy bullet position

                        enemybullet[b].rect.X = (int)enemybullet[b].position.X;// sets the enemyship recatmgle position to enemy bullets position
                        enemybullet[b].rect.Y = (int)enemybullet[b].position.Y;

                        enemybullet[b].bbox = new BoundingBox(new Vector3(enemybullet[b].position.X - enemybullet[b].rect.Width / 2, enemybullet[b].position.Y - enemybullet[b].rect.Height / 2, 0),
                            new Vector3(enemybullet[b].position.X + enemybullet[b].rect.Width / 2, enemybullet[b].position.Y + enemybullet[b].rect.Height / 2, 0));// creats a bounding box

                        if (enemybullet[b].position.X + enemybullet[b].rect.Width / 2 <= 0)// makes the enemy bullet invisible if leaves the left hand side of the screen
                            enemybullet[b].visible = false;

                        if (enemybullet[b].bbox.Intersects(ship.bbox) && invunerabletime < 0)// check for collision between enemybullet and player ship
                        {
                            enemybullet[b].visible = false;// makes the enemy bullet invisible
                            gooddeath.Play();// plays a sound effect
                            // goodexplosion.start(ship.position);
                            lives--;// takes a life away
                            vibration = 1f;// adds 1 to vibration
                            resetship();//  calls reset ship
                            
                        }
                    }


                    if (enemyship[b].bbox.Intersects(ship.bbox) && invunerabletime < 0)// checks to see if the enemy ship sollides with the player ship andds runs if it does
                        if (enemyship[b].visible)
                        {
                            enemyship[b].visible = false;// makes the enmy ship invisible
                            lives--;// takes a life away
                            vibration = 1f;// adds one to vibration
                            gooddeath.Play();// play a sound effect
                            // goodexplosion.start(ship.position);
                            explosion1.start(enemyship[b].position);// start an animation where the collision occured
                            resetship();// resets the ship
                        }

                }

                for (int b = 0; b < enemyship2.Count(); b++)//creates a for loop based on the amount
                {
                    if (enemyship2[b].position.X - enemyship2[b].rect.Width / 2 <= 0)// checks to see if the enemy ship has left the screen and sets it to invisible if it has
                        enemyship2[b].visible = false;

                    if (enemyship2[b].visible && enemyship2[b].position.Y - enemyship2[b].rect.Height / 2 < ship.position.Y && enemyship2[b].position.Y + enemyship2[b].rect.Height / 2 > ship.position.Y && enemyship2[b].position.X - enemyship2[b].rect.Width / 2 < displaywidth)
                        if (!enemybullet2[b].visible)// fires an enemyship2 bullet when the player is level and the enemy ship is on screen
                        {
                            enemybullet2[b].visible = true;// makes the enemy bullet visible
                            enemybullet2[b].position = enemyship2[b].position;// makes the startin point of the bullets the enemy ship position
                            badfire.Play();// plays a sound effect

                        }


                    if (enemybullet2[b].visible)// runs if the enemy bullet2 is visible
                    {
                        enemybullet2[b].position += enemybullet2[b].velocity;// adds enemy bullet2 velocity to enemy bullet2 position

                        enemybullet2[b].rect.X = (int)enemybullet2[b].position.X;// makes enemybullet2 the rectangles position the same as the enemybullet2 position
                        enemybullet2[b].rect.Y = (int)enemybullet2[b].position.Y;

                        enemybullet2[b].bbox = new BoundingBox(new Vector3(enemybullet2[b].position.X - enemybullet2[b].rect.Width / 2, enemybullet2[b].position.Y - enemybullet2[b].rect.Height / 2, 0),
                            new Vector3(enemybullet2[b].position.X + enemybullet2[b].rect.Width / 2, enemybullet2[b].position.Y + enemybullet2[b].rect.Height / 2, 0));//creats a bounding box

                        if (enemybullet2[b].position.X + enemybullet2[b].rect.Width / 2 <= 0)//makes the enemybullet2 invisible if it leaves the screen
                            enemybullet2[b].visible = false;

                        if (enemybullet2[b].bbox.Intersects(ship.bbox) && invunerabletime < 0)// asks if the enemy bullet2 intersects the player ship and the player ship is vunrerable and runs is it is
                        {
                            enemybullet2[b].visible = false;// makes enemy bullet2 invisible
                            gooddeath.Play();// plays sound effect
                            // goodexplosion.start(ship.position);
                            lives--;// take a life away
                            vibration = 1f;// adds one to vibration
                            resetship();// resets the ship

                        }
                    }


                    if (enemyship2[b].bbox.Intersects(ship.bbox) && invunerabletime < 0)// checks for collision between enemyship2 and player ship and runs if it happens
                        if (enemyship2[b].visible)// only runs if the enemyship2 was visible
                        {
                            enemyship2[b].visible = false;// makes the enemyship2 invisible
                            lives--;// takes a life away
                            vibration = 1f;// adds one to vibration
                            gooddeath.Play();// plays sound effect
                            // goodexplosion.start(ship.position);
                            explosion1.start(enemyship2[b].position);// plays animation
                            resetship();// rests the player ship
                        }

                }

                if (spiderlives <= 0)// stop the player being able fire up wards when the spider has died
                    bullet2visible = false;


                for (int b = 0; b < enemyship2.Count(); b++)//creats a for loop based on the amount of enemyship2's
                    for (int y = 0; y < playerbullet2.Count(); y++)// creats a for loop based on the amount of playerbulet2'2
                        if (pad[0].Buttons.Y == ButtonState.Pressed && !playerbullet2[y].visible && gunbuttonreleased2 && playerguncooldown < 0 && bullet2visible
                             && enemyship2[1].position.X + enemyship2[1].rect.Width / 2 < 0 && enemyship2[2].position.X + enemyship2[2].rect.Width / 2 < 0
                             && enemyship2[3].position.X + enemyship2[3].rect.Width / 2 < 0 && enemyship2[4].position.X + enemyship2[4].rect.Width / 2 < 0
                             && enemyship2[5].position.X + enemyship2[5].rect.Width / 2 < 0 && enemyship2[6].position.X + enemyship2[6].rect.Width / 2 < 0
                             && enemyship2[5].position.X + enemyship2[7].rect.Width / 2 < 0 && enemyship2[8].position.X + enemyship2[8].rect.Width / 2 < 0
                             && enemyship2[9].position.X + enemyship2[9].rect.Width / 2 < 0)// asks if all enemy ship2 have left the screen
                        {
                            background1.velocity = new Vector3(0, 0, 0);// stop background1 from moving
                            background.velocity = new Vector3(0, 0, 0);// stop background from moving
                            playerbullet2[y].visible = true;// allows the player to fire upwards 
                            playerbullet2[y].position = ship.position;// makes the playerbullet2 position start at player ship position
                            goodfire.Play();// plays sound effects
                            music.Pause();// pauses music
                            playerguncooldown = 50;// makes the player wait before he can refiire
                            gunbuttonreleased2 = false;// make the player releses the button before he can refire
                        }

                if (pad[0].Buttons.Y == ButtonState.Released)// resets the player bullet2 when gun button is released
                    gunbuttonreleased2 = true;

                for (int y = 0; y < playerbullet2.Count(); y++)// creats a for loop based on the amount of playerbullet2's
                    if (playerbullet2[y].visible)// run if player bullet2 is visible
                    {
                        playerbullet2[y].position += playerbullet2[y].velocity;// adds playerbullet2 velocity to playerbullet2 position
                        playerbullet2[y].rect.Y = (int)playerbullet2[y].position.Y;// makes the position of the playerbullet2 rectangle the same as the playbeullet2 position for drawing purposes
                        playerbullet2[y].rect.X = (int)playerbullet2[y].position.X;

                        playerbullet2[y].bbox = new BoundingBox(new Vector3(playerbullet2[y].position.X - playerbullet2[y].rect.Width / 2, playerbullet2[y].position.Y - playerbullet2[y].rect.Height / 2, 0),
                            new Vector3(playerbullet2[y].position.X + playerbullet2[y].rect.Width / 2, playerbullet2[y].position.Y + playerbullet2[y].rect.Height / 2, 0));// creats a bounding box

                        if (playerbullet2[y].position.Y + playerbullet2[y].rect.Height / 2 <= 0)// makes the playerbullet2 invisible if it leaves the right hand side of the screen
                        {
                            playerbullet2[y].visible = false;
                            score--;// take a point away if the player misses with a bullet
                        }
                    }

                
                for (int y = 0; y < playerbullet2.Count(); y++)//creats a loop based on the amount of payerbullet2's
                    if (playerbullet2[y].visible && playerbullet2[y].bbox.Intersects(spider.bbox) && spider.visible && spider.position.Y + spider.rect.Height / 2 > 0)// check for collision betwen playebullet2 and the spider
                    {
                        spiderlives--;// takes a life away from spider
                        score += 20;// adds points to the player score
                        vibration = 1f;// adds one to vibration
                        spiderdeath.start(spider.position);// plays animation
                        playerbullet2[y].visible = false;// sets the playerbullets 2 to invisible
                        if (spiderlives <= 0)//runs if the spider has No lives
                        {
                            background.velocity.X = -1;// moves the bakground
                            background1.velocity.X = -1;// moves the background
                            music.Resume();// resumes the music
                            
                        }
                    }

                if (spiderlives <=0)
                    backgroundcount += timebetweenupdates;

                if (backgroundcount >= 4000)
                {
                    backgroundcount = 0;   
                    resetlevel();
                }

            

                if (spider.bbox.Intersects(ship.bbox) && ship.visible && spider.visible && invunerabletime < 0)// check for the pider colliding with the player when the player is vunerable
                {
                    lives--;// takes  a life off the player
                    gooddeath.Play();// plays a sound effect
                    resetship();// reset the player ship
                }

                for (int y = 0; y < spiderweb.Count(); y++)// creats a for loop based on the amount of spider web
                    if (spider.visible && spider.position.X - spider.rect.Width / 2 < ship.position.X && spider.position.X + spider.rect.Width / 2 > ship.position.X && spider.position.Y + spider.rect.Height / 2 > 0 && !spiderweb[y].visible && spiderwebcooldown <= 0)
                        // runs if the player ship is directly belwo the spider 
                    {
                        spiderweb[y].visible = true;// makes the spider web visible
                        spiderweb[y].position = spider.position;// start the spider web at the spider poition
                        goodfire.Play();// plays sound effect
                        spiderwebcooldown = 50;// makes the spider wait bofre it can refire(gives a look of along web)


                    }

                for (int y = 0; y < spiderweb.Count(); y++)// creats a for loop based on the amount of spider webs
                    if (spiderweb[y].visible)
                    {
                        spiderweb[y].position += spiderweb[y].velocity;// adds spider web velocity to spder web position
                        spiderweb[y].rect.Y = (int)spiderweb[y].position.Y;// sets the spider webs rectangle based on the spider web position
                        spiderweb[y].rect.X = (int)spiderweb[y].position.X;

                        spiderweb[y].bbox = new BoundingBox(new Vector3(spiderweb[y].position.X - spiderweb[y].rect.Width / 2, spiderweb[y].position.Y - spiderweb[y].rect.Height / 2, 0),
                        new Vector3(spiderweb[y].position.X + spiderweb[y].rect.Width / 2, spiderweb[y].position.Y + spiderweb[y].rect.Height / 2, 0));// creates a bounding box for the spider web

                        if (spiderweb[y].position.Y - spiderweb[y].rect.Height / 2 > displayheight)
                            spiderweb[y].visible = false;// makes the spider web invisible if it leaves the screen
                    }

                for (int y = 0; y < spiderweb.Count(); y++)// creats a for loop for based on the amount of spider webs
                    if (spiderweb[y].visible)// only runs if the spider web is visible
                        if (spiderweb[y].visible && spiderweb[y].bbox.Intersects(ship.bbox))// cheaks for collisions between the spider web and the player ship
                        {
                            spiderweb[1].visible = false;// sets the webbing to false is it collides with the player
                            spiderweb[2].visible = false;
                            spiderweb[3].visible = false;
                            spiderweb[4].visible = false;
                            spiderweb[5].visible = false;
                            spiderweb[6].visible = false;
                            spiderweb[7].visible = false;
                            spiderweb[0].visible = false;
                            lives--;// takes a life away from the player if he is hit by the webbing
                            gooddeath.Play();
                        }


                ssspawncooldown -= timebetweenupdates;// creats a timer for the scorpoin to spawn 

                scorpion.position += scorpion.velocity;// adds the scorpion velocity to the scorpoins position
                scorpion.rect.Y = (int)scorpion.position.Y;// sets the scorpions rectangle based on the scrpions  position
                scorpion.rect.X = (int)scorpion.position.X;

                scorpion.bbox = new BoundingBox(new Vector3(scorpion.position.X - scorpion.rect.Width / 2, scorpion.position.Y - scorpion.rect.Height / 2, 0),
                    new Vector3(scorpion.position.X + scorpion.rect.Width / 2, scorpion.position.Y + scorpion.rect.Height / 2, 0));// creats a bounding box for the scorpion

                if (scorpion.position.Y < displayheight / 2)// runs if the scorpoin position is half way up the screen
                {
                    scorpion.position.Y = displayheight / 2;// stops the scorpoin half way
                    scorpion.velocity.Y = -scorpion.velocity.Y;// reverses the scorpoind diretion
                }

                if (scorpion.position.Y >= 500)// run if scorpoin position is 500 on the y axis
                {
                    ssspawncooldown = 10000;// makes the scorpion wait before it can attack
                    scorpion.position.Y = 499;// move the scorpoin off 500 so the scode does not repeat
                    scorpion.velocity.Y = 0;// stop the scorpoin from moving 
                }
                for (int b = 0; b < enemyship2.Count(); b++)// creats a for loop based on the amount of enmeyship2
                    for (int y = 0; y < playerbullet2.Count(); y++)// creats a for loop based on the amount of playerbullet2
                        if (ssspawncooldown <= 0 && scorpion.velocity.Y == 0 && enemyship2[0].position.X + enemyship2[0].rect.Width / 2 < 0
                            && enemyship2[1].position.X + enemyship2[1].rect.Width / 2 < 0 && enemyship2[2].position.X + enemyship2[2].rect.Width / 2 < 0
                            && enemyship2[3].position.X + enemyship2[3].rect.Width / 2 < 0 && enemyship2[4].position.X + enemyship2[4].rect.Width / 2 < 0
                            && enemyship2[5].position.X + enemyship2[5].rect.Width / 2 < 0 && enemyship2[6].position.X + enemyship2[6].rect.Width / 2 < 0
                            && enemyship2[5].position.X + enemyship2[7].rect.Width / 2 < 0 && enemyship2[8].position.X + enemyship2[8].rect.Width / 2 < 0
                            && enemyship2[9].position.X + enemyship2[9].rect.Width / 2 < 0 && scorpionlives > 0)// resets the scorpoin based on a timer, and that the spider is still alive and that  all the minior ship have passed and it is not already spawned 
                        {
                            resetscorpion();

                        }
                if (scorpion.position.Y - scorpion.rect.Width / 2 < ship.position.Y && scorpion.position.Y + scorpion.rect.Y > ship.position.Y && !scorpionsting.visible)// attempts to sting the player when the player is level with it 
                {
                    scorpionsting.visible = true;// makes the scorpionsting visible
                    scorpionsting.position = scorpion.position;// makes the scorpionsting start point the scorpoins start point
                    badfire.Play();// plays a sound effect

                }
                if (scorpionsting.visible)// only runs if the scorpion has tried to sting
                {
                    scorpionsting.position += scorpionsting.velocity;// adds scorpionsting velocity to scorpionsting position
                    scorpionsting.rect.Y = (int)scorpionsting.position.Y;// make the rectangle position the same as the ship position
                    scorpionsting.rect.X = (int)scorpionsting.position.X;

                    if (scorpionsting.position.X - scorpionsting.rect.Width / 2 < 0)// makes the scorpionsting invisible if it leaves the screen
                        scorpionsting.visible = false;
                }

                scorpionsting.bbox = new BoundingBox(new Vector3(scorpionsting.position.X - scorpionsting.rect.Width/2,scorpionsting.position.Y - scorpionsting.rect.Height/2,0),
                    new Vector3(scorpionsting.position.X + scorpionsting.rect.Width/2 , scorpionsting.position.Y + scorpionsting.rect.Height/2,0));// creats bounding box

                if (scorpionsting.bbox.Intersects(ship.bbox) && invunerabletime <= 0 && scorpionsting.visible)// check for cillision betwwen player ship and scorpon sting while the player is vunerable and the sting is visible
                {
                    resetship();// rests the ship
                    lives--;// takes a life way
                    gooddeath.Play();// plays sound effect
                    scorpionsting.visible = false;// makes scorpion sting invisible
                }


                GamePad.SetVibration(PlayerIndex.One, vibration, vibration);// set the vibration motors to the vibration variable


                if (vibration > 0)// take 0.1 away from vibration to stop it vibrating after time
                {
                    vibration -= 0.1f;

                }

            }


            else
            {
                // Game is over
                if (score >= highscore)// makes a new highscore if the player has one
                    highscore = score;

                if (pad[0].Buttons.Back == ButtonState.Pressed)// if the back button is pressed the game exits
                    this.Exit();
                if (pad[0].Buttons.Start == ButtonState.Pressed)// if the start button is pressed the game is reset
                    resetgame();
            }
            base.Update(gameTime);
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();// starts spritebatch

            background.drawme2(ref spriteBatch);// draws background
    
            background1.drawmeflip(ref spriteBatch);// draws background1
              

            if (invunerabletime % 200 < 100)// makes the player flash
                ship.drawme(ref spriteBatch);// drawms the players ship with a flashing effect
//         

            for (int y = 0; y < playerbullet.Count(); y++)// creats afor loop based on the amount of player bullets and draws them if visible
            {
              playerbullet[y].drawme(ref spriteBatch);    
            }


            for (int y = 0; y < enemyship.Count(); y++)// creats a for loop based on the amount of enemy ship ans draw them if they are visible enemybullet uses this for loop in the same way
            {
                enemyship[y].drawme(ref spriteBatch);
                enemybullet[y].drawme(ref spriteBatch);
            }


            for (int y = 0; y < enemyship2.Count(); y++)// creats a for loop based on the amount of enemy ship 2 and draws them if visible enemybullet2 uses this for loop in the sameway
            {
                enemyship2[y].drawme(ref spriteBatch);
                enemybullet2[y].drawme(ref spriteBatch);
            }


            for (int y = 0; y < playerbullet.Count(); y++)// creats a for loop base don the amount of player bullets and draws them if visible
                playerbullet2[y].drawme(ref spriteBatch);

  
            for (int y = 0; y < spiderweb.Count(); y++)// creats a for loop base don the amount of webbing and draw it if its visible
                   spiderweb[y].drawme(ref spriteBatch);// draws the spiders web if its visible
    
            explosion1.drawme(ref spriteBatch);// draws the explosion
            explosion2.drawme(ref spriteBatch);
            goodexplosion.drawme(ref spriteBatch);
            spiderdeath.drawme(ref spriteBatch);


            spriteBatch.DrawString(mainfont, " SCORE " + score.ToString("0"), new Vector2(350, 50), Color.White);// draws the score
            spriteBatch.DrawString(mainfont, "Lives " + lives.ToString("0"), new Vector2(50, 50), Color.White);// draws the amount of lives left
            spriteBatch.DrawString(mainfont,"High Score "+ highscore.ToString(), new Vector2(displaywidth/2, displayheight-50),Color.White);// draws the high score
            
     
            spider.drawme(ref spriteBatch);// draws the spider if its visible
            scorpion.drawme(ref spriteBatch);// draws the scorpion if it visible
            scorpionsting.drawme(ref spriteBatch);// draws the scorpion sting if its visible

            if (gameover)// draws the game over image when the gameis over
                gameoverimage.drawme2(ref spriteBatch);


            spriteBatch.End();// ends spritebatch


            base.Draw(gameTime);
        }
    }
}
