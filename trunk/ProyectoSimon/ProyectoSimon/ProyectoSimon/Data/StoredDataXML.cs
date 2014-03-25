using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProyectoSimon
{
    public class StoredDataXML
    {
        private XmlDocument xDoc;
        private string singularElement, pluralElement;
        public static string USERS_DATA_PATH = "Data//users.xml";
        public static string GAMES_DATA_PATH = "Data//games.xml";
        public static string USERS_STORED_PLURAL = "users";
        public static string USERS_STORED_SINGULAR = "user";
        public static string USER_NAME = "name";
        public static string USER_SURNAME = "surname";
        public static string USER_PICTURE = "picture";
        public static string USER_AGE = "age";
        public static string STATISTIC_SINGULAR = "statistic";
        public static string STATISTIC_GAMENAME = "gamename";
        public static string STATISTIC_ATTRIBUTE = "attribute";
        public static string ATTRIBUTE_NAME = "name";
        public static string ATTRIBUTE_VALUE = "value";

        public StoredDataXML(string path, string pluralElem, string singularElem)
        {
            xDoc = new XmlDocument();
            xDoc.Load(path);
            //"XMLFile1.xml"
            singularElement = singularElem;
            pluralElement = pluralElem;
        }

        public void setStoredElements(List<User> users)
        {
            XmlTextWriter writer = new XmlTextWriter("Data//users.xml", System.Text.Encoding.UTF8);
            Statistics statistic;
            int statsCount;
            // Use indentation for readability.
            writer.Formatting = Formatting.Indented;
            // Writes the xml declaration.
            writer.WriteStartDocument();
            // Writes out a start tag with the specified local name.
            writer.WriteStartElement("users");

            foreach (User user in users)
            {
                writer.WriteStartElement("user");
                // Writes an element with the specified local name and value.
                writer.WriteElementString("name", user.getName().ToLower());
                writer.WriteElementString("surname", user.getSurname().ToLower());
                writer.WriteElementString("picture", user.getPath().ToLower());
                writer.WriteElementString("age", user.getAge().ToString());
                statsCount = user.getStatisticsCount();

                // Go through user's statistics.
                for (int j = 0; j < statsCount; j++)
                {
                    writer.WriteStartElement("statistic");
                    statistic = user.getStatistics(j);
                    writer.WriteElementString("gamename", statistic.getGameName());
                    string[] attribs = statistic.getKeys();

                    // Go through statistics' attributes.
                    for (int k = 0; k < attribs.Length; k++)
                    {
                        writer.WriteStartElement("attribute");
                        writer.WriteElementString("name", attribs[k]);
                        writer.WriteElementString("value", statistic.getAttribute(attribs[k]).ToString());
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            // Close the writer.
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Get stored elements from an specific XML.
        /// </summary>
        /// <returns>XmlNodeList</returns>
        public XmlNodeList getStoredElements()
        {
            XmlNodeList elements = xDoc.GetElementsByTagName(pluralElement);

            return ((XmlElement) elements[0]).GetElementsByTagName(singularElement);
        }
    }
}