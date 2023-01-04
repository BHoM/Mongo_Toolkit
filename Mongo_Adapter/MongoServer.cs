/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using System.Threading;

namespace BH.Adapter.Mongo
{
    public class MongoServer
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public MongoServer(string folderName)
        {
            if (m_Process != null && !m_Process.HasExited && m_FolderName != folderName)
                throw new Exception("A Mongo Server is already running you machine.");

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
            }
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public List<string> Databases()
        {
            List<BsonDocument> bsonList = m_Client.ListDatabases().ToList();

            return bsonList.Select(x => x.GetElement("name").Value.ToString()).ToList();
        }

        /***************************************************/

        public bool DeleteDatabase(string name)
        {
            m_Client.DropDatabase(name);
            return true;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private void M_Process_Exited(object sender, EventArgs e)
        {
            m_Process = null;
            m_FolderName = "";

            if (Killed != null)
                Killed.Invoke();
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        public event Action Killed; 

        private static string m_FolderName = "";
        private static MongoClient m_Client = null;
        private static System.Diagnostics.Process m_Process = null;


        /***************************************************/
    }
}




