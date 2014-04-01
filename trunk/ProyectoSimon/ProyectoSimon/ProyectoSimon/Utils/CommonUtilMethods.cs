using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProyectoSimon.Utils
{

    /// <summary>
    /// Common class implemented to store all utils method used for all system.
    /// </summary>
    public class CommonUtilMethods
    {
        public const string PNG = ".png";
        public const string JPG = ".jpg";
        public const string JPEG = ".jpeg";

        /// <summary>
        /// Returns the value of an statistical attribute.
        /// </summary>
        public static string GetStatisticsValue(string[] p)
        {
            return p[p.Length - 1];
        }

        /// <summary>
        /// Returns the name of an statistical attribute.
        /// </summary>
        public static string GetStatisticsName(string[] p)
        {
            string name = p[0];

            // Thinking that a name can contain spaces ...
            for (int i = 1; i < p.Length; i++)
            {
                if (i != p.Length - 1)
                {
                    name += CommonConstants.STRING_BLANK_SPACE + p[i];
                }
            }

            return name;
        }

        /// <summary>
        /// Returns relative path for the captures directory.
        /// </summary>
        public static string GenerateCapturePath(string nameField, string surnameField, String extension)
        {
            // Returns the following format: path//name_surname.extension
            return String.Format("{0}//{1}_{2}{3}", CommonConstants.RELATIVE_CAPTURES_PATH, nameField.ToLower(), surnameField.ToLower(), extension);
        }
    }

}
