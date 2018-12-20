/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using MongoDB.Bson;
using MongoDB.Driver;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public int HistorySize { get; set; } = 5;


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public MongoAdapter(string serverName = "mongodb://localhost", int port = 27017, string databaseName = "project", string collectionName = "bhomObjects", bool useHistory = true)
        {
            AdapterId = "Mongo_id";

            if (!serverName.StartsWith("mongodb://"))
                serverName = "mongodb://" + serverName;

            m_Client = new MongoClient(serverName + ":" + port.ToString());
            IMongoDatabase database = m_Client.GetDatabase(databaseName);
            m_Collection = database.GetCollection<BsonDocument>(collectionName);

            if (useHistory)
            {
                IMongoDatabase hist_Database = m_Client.GetDatabase(databaseName + "_History");
                m_History = hist_Database.GetCollection<BsonDocument>(collectionName);
            }
        }

        /***************************************************/

        public MongoAdapter(string connectionString, string databaseName = "project", string collectionName = "bhomObjects", bool useHistory = false)
        {

            AdapterId = "Mongo_id";
            if (!connectionString.StartsWith("mongodb://"))
                connectionString = "mongodb://" + connectionString;

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            m_Client = new MongoClient(settings);

            IMongoDatabase database = m_Client.GetDatabase(databaseName);
            m_Collection = database.GetCollection<BsonDocument>(collectionName);

            if (useHistory)
            {
                IMongoDatabase hist_Database = m_Client.GetDatabase(databaseName + "_History");
                m_History = hist_Database.GetCollection<BsonDocument>(collectionName);
            }
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public string ServerName()
        {
            MongoServerAddress server = m_Collection.Database.Client.Settings.Server;
            return "mongodb://" + server.ToString();
        }

        /*******************************************/

        public string DatabaseName()
        {
            return m_Collection.Database.DatabaseNamespace.DatabaseName; 
        }

        /*******************************************/

        public string CollectionName()
        {
            return m_Collection.CollectionNamespace.CollectionName; 
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private MongoClient m_Client;
        private IMongoCollection<BsonDocument> m_Collection = null;
        private IMongoCollection<BsonDocument> m_History = null;


        /***************************************************/
    }
}
