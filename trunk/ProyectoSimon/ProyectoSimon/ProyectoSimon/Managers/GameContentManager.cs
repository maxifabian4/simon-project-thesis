using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon
{
    class GameContentManager
    {
        private static GameContentManager instance;
        private ContentManager contentManager;
        // Constants textures.
        public static String TEXTURE_MAIN_MENU = "mainmenu";
        public static String TEXTURE_BLANK = "blank";
        public static String TEXTURE_BACKGROUND_MAIN = "mainBackground";
        public static String TEXTURE_BACKGROUND_GAME = "gameBackground";
        public static String TEXTURE_BACKGROUND_GAME2 = "gameBackground2";
        public static String TEXTURE_BACKGROUND_ARROWS = "backgroundArrows";
        public static String TEXTURE_USER_TEMPLATE = "userTemplate";
        public static String TEXTURE_GAMEBOX = "boxempty";
        public static String TEXTURE_ALGO = "algo";
        // Constants fonts.
        public static String GAME_INSTANCE_FONT = "gameInstanceFont";
        public static String USER_MODULE_FONT = "userModuleFont";
        public static String FONT_BEAUTIFULEVERYTIME = "BeautifulEveryTime";
        public static String FONT_BEAUTIFULEVERYTIMEPROJECT = "BeautifulEveryTimeProject";
        public static String FONT_STATISTICSFONT = "StatisticFont";
        //Structures
        private IDictionary<String, SpriteFont> fonts;
        private IDictionary<String, Texture2D> textures;

        private GameContentManager()
        {
            //vars initialitation just in case non statics vars
            fonts = new Dictionary<String, SpriteFont>();
            textures = new Dictionary<String, Texture2D>();

        }

        public static GameContentManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameContentManager();
                }
                return instance;
            }
        }

        public void initialize()
        {
            // Load the fonts;
            loadFonts();
            // Load textures.
            loadTextures();
        }

        public void setContent(ContentManager cm)
        {
            contentManager = cm;
        }
        /// <summary>
        /// Loads textures from the system.
        /// </summary>
        private void loadTextures()
        {
            textures.Add(TEXTURE_BLANK, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BLANK));
            textures.Add(TEXTURE_BACKGROUND_MAIN, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BACKGROUND_MAIN));
            textures.Add(TEXTURE_USER_TEMPLATE, contentManager.Load<Texture2D>("Textures/" + TEXTURE_USER_TEMPLATE));
            textures.Add(TEXTURE_BACKGROUND_GAME, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BACKGROUND_GAME));
            textures.Add(TEXTURE_BACKGROUND_GAME2, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BACKGROUND_GAME2));
            textures.Add(TEXTURE_BACKGROUND_ARROWS, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BACKGROUND_ARROWS));
            textures.Add(TEXTURE_GAMEBOX, contentManager.Load<Texture2D>("Textures/" + TEXTURE_GAMEBOX));
            textures.Add(TEXTURE_ALGO, contentManager.Load<Texture2D>("Textures/" + TEXTURE_ALGO));
            textures.Add(TEXTURE_MAIN_MENU, contentManager.Load<Texture2D>("Textures/" + TEXTURE_MAIN_MENU));
        }
        /// <summary>
        /// Loads fonts from the system.
        /// </summary>
        private void loadFonts()
        {
            fonts.Add(GAME_INSTANCE_FONT, contentManager.Load<SpriteFont>("SpriteFont/" + GAME_INSTANCE_FONT));
            fonts.Add(USER_MODULE_FONT, contentManager.Load<SpriteFont>("SpriteFont/" + USER_MODULE_FONT));
            fonts.Add(FONT_BEAUTIFULEVERYTIME, contentManager.Load<SpriteFont>("SpriteFont/" + FONT_BEAUTIFULEVERYTIME));
            fonts.Add(FONT_BEAUTIFULEVERYTIMEPROJECT, contentManager.Load<SpriteFont>("SpriteFont/" + FONT_BEAUTIFULEVERYTIMEPROJECT));
            fonts.Add(FONT_STATISTICSFONT, contentManager.Load<SpriteFont>("SpriteFont/" + FONT_STATISTICSFONT));
        }
        /// <summary>
        /// Gets a specific font.
        /// </summary>
        public SpriteFont getFont(String typeFont)
        {
            return fonts[typeFont];
        }
        /// <summary>
        /// Gets a specific texture.
        /// </summary>
        public Texture2D getTexture(String nameTexture)
        {
            return textures[nameTexture];
        }
    }
}
