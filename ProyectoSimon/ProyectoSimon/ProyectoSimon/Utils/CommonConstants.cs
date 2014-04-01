using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProyectoSimon.Utils
{
    /// <summary>
    /// Constants are immutable values which are known at compile time and do not change for the life of the program.
    /// CommonConstants class has been defined in order to stored the common constants used by the systems.
    /// </summary>
    public class CommonConstants
    {
        // Margins of menu panel used by MenuScreen hierarchy.
        public const int MARGIN_LEFT_MENU_PANEL = 20;
        public const int MARGIN_UP_MENU_PANEL = 10;
        public const int MARGIN_DOWN_MENU_ENTRIES = 50;
        public const int MARGIN_ALIGN_MENU_ENTRIES = 20;
        public const int MARGIN_UP_GAME_INSTANCE = 30;
        public const int MARGIN_RIGHT_ELEMENTS_MENU_PANEL = 10;

        // Default constants used by MenuScreen in order to create a new user.
        public const string DEFAULT_USER_NAME = "nombre";
        public const string DEFAULT_USER_LASTNAME = "apellido";
        public const string DEFAULT_USER_AGE_WORD = "edad";
        public const string DEFAULT_USER_CAPTURE = "foto";
        public const int DEFAULT_USER_AGE_VALUE = 0;
        public const int NEW_USER_INDEX = -1;

        // Constants used to draw a default template in the MainMenuScreen.
        public const string DEFAULT_USER_AGE_STRING = "{0} años";
        public const string PROJECT_NAME = "simon";
        public const string PROJECT_DESCRIPTION = "PROYECTO ACADEMICO";

        // Default text used to exit from a current screen.
        public const string MENU_ENTRY_EXIT = "Salir";
        public const string MENU_ENTRY_CANCEL = "Cancelar";
        public const string MENU_ENTRY_ADD = "Agregar";
        public const string MENU_ENTRY_RESUME = "Reanudar";
        public const string MENU_ENTRY_EXIT_GAME = "Salir del juego";
        public const string MENU_ENTRY_PLAY = "Jugar";
        public const string MENU_ENTRY_STATISTICS = "Estadísticas";
        public const string MENU_ENTRY_NEW_USER = "Nuevo usuario";

        // Default title used for StatisticsMenuScreen.
        public const string STATISTIC_SCREEN_TITLE = "estadísticas";

        // String separators.
        public const char STATISTICAL_MEASURES_SEPARATOR = ' ';
        public const string STRING_BLANK_SPACE = " ";

        // Max number of fields used for UserFormMenuScreen.
        public const int USER_FORM_MAX_FIELDS = 3;

        // Used to store all captures taken for a new user.
        public const string RELATIVE_CAPTURES_PATH = "Data";

        // Constants used to represent a game.
        public const string CIRCLES_GAME_NAME = "círculos";
        public const string CHOOSER_GAME_NAME = "clasificador";
        public const string ARROWS_GAME_NAME = "flechas";

        // Default title used for UserFormMenuScreen.
        public const string NEWFORM_SCREEN_TITLE = "nuevo usuario";
        public const string DEFAULT_TEXT_CURSOR = "|";

        // Common messages used by PauseMenuScreen.
        public const string EXIT_GAME_QUESTION = "Desea salir del juego?";
        public const string EXIT_GAME_ANSWER_OPTIONS = "ACEPTAR = enter CANCELAR = esc";

        // Common messages used by MainMenuScreen.
        public const string TITLE_MESSAGE_UNPLUGGED_KINECT = "Dispositivo Kinect desconectado !!";
        public const string DESCRIPTION_MESSAGE_UNPLUGGED_KINECT = "Presione Enter para regresar al menú principal.";

        // Common messages used by MainMenuScreen.
        public const string EXIT_SYSTEM_QUESTION = "Desea salir del systema?";
        public const string EXIT_SYSTEM_ANSWER_OPTIONS = "ACEPTAR = enter CANCELAR = esc";

    }
}
