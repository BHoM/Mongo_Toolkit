using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using System.Threading;
using MongoDB.Driver.Core;

namespace BH.Engine.Mongo
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string MongoServer(string folderName)
        {
            string alert = "";

            if (m_Process != null && !m_Process.HasExited && m_FolderName != folderName)
            {
                alert = "A Mongo Server is already running you machine.";
            }

            if (m_Process == null || m_Process.HasExited) 
            {
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                m_FolderName = folderName;
                m_Process = System.Diagnostics.Process.Start("mongod", "--dbpath " + "\""+folderName + "\"");
                m_Process.Exited += M_Process_Exited;
                m_Process.Disposed += M_Process_Exited;

                Thread.Sleep(1000);
                m_Client = new MongoClient(@"mongodb://localhost:27017");
                alert = "MongoDB Files Stored in " + folderName;
            }
            return alert;
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> Databases()
        {
            List<BsonDocument> bsonList = m_Client.ListDatabases().ToList();

            return bsonList.Select(x => x.GetElement("name").Value.ToString()).ToList();
        }

        /***************************************************/

        public static bool DeleteDatabase(string name)
        {
            m_Client.DropDatabase(name);
            return true;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void M_Process_Exited(object sender, EventArgs e)
        {
            m_Process = null;
            m_FolderName = "";

            if (Killed != null)
                Killed.Invoke();
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        public static event Action Killed; 

        private static string m_FolderName = "";
        private static MongoClient m_Client = null;
        private static System.Diagnostics.Process m_Process = null;


        /***************************************************/
    }
}
