/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel;
using System.Threading;

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

        [Description("Create an adapter to a Mongo database. Use this constructor if you have the name of the server hosting the database and its port number.")]
        [Input("serverName", "address of the server hosting the database. Will generally start with \"mongodb://\".")]
        [Input("port", "port number used to access the database. This will generally be the number after the ':' at the end of the address of the database.")]
        [Input("databaseName", "name of the database itself.")]
        [Input("collectionName", "name of the collection you want to access inside that database.")]
        [Input("useHistory", "If true, will store a copy of the data pushed to Mongo in a separate collection. Data from the last 5 pushes is available there.")]
        [Output("adapter", "Adapter to Mongo Database.")]
        public MongoAdapter(string serverName = "mongodb://localhost", int port = 27017, string databaseName = "project", string collectionName = "bhomObjects", bool useHistory = true)
        {

            if (!serverName.StartsWith("mongodb://"))
                serverName = "mongodb://" + serverName;

            m_Client = new MongoClient(serverName + ":" + port.ToString());

            // Give it maximum 5 seconds to establish connection 
            for (int i = 0; i < 50; i++)
            {
                if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                    Thread.Sleep(100);
                else
                    break;
            }

            // Consider the server is unavailable if connection failed after 5 seconds
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
            {
                BH.Engine.Base.Compute.RecordError($"Connection to the host server {serverName} " +
                    $"on port {port} failed");
                return;
            }

            m_Database = m_Client.GetDatabase(databaseName);
            m_Collection = m_Database.GetCollection<BsonDocument>(collectionName);

            if (useHistory)
            {
                IMongoDatabase hist_Database = m_Client.GetDatabase(databaseName + "_History");
                m_History = hist_Database.GetCollection<BsonDocument>(collectionName);
            }
        }

        /***************************************************/

        [Description("Create an adapter to a Mongo database. Use this constructor if you have a single long string representing the location of your database (and potentially identification).")]
        [Input("connectionString", "Text given to you to access the database. Will generally start with \"mongodb://\".")]
        [Input("databaseName", "name of the database itself.")]
        [Input("collectionName", "name of the collection you want to access inside that database.")]
        [Input("useHistory", "If true, will store a copy of the data pushed to Mongo in a separate collection. Data from the last 5 pushes is available there.")]
        [Output("adapter", "Adapter to Mongo Database.")]
        public MongoAdapter(string connectionString, string databaseName = "project", string collectionName = "bhomObjects", bool useHistory = false)
        {

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            m_Client = new MongoClient(settings);

            // Give it maximum 5 seconds to establish connection 
            for (int i = 0; i < 50; i++)
            {
                if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                    Thread.Sleep(100);
                else
                    break;
            }

            // Consider the server is unavailable if connection failed after 5 seconds
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
            {
                Engine.Base.Compute.RecordError($"Connection to the host server {settings.Server.Host} " +
                    $"on port {settings.Server.Port} failed using credentials {settings.Credential}");
                return;
            }

            m_Database = m_Client.GetDatabase(databaseName);
            m_Collection = m_Database.GetCollection<BsonDocument>(collectionName);

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
            if (m_Collection == null)
                return "";

            MongoServerAddress server = m_Collection.Database.Client.Settings.Server;
            return "mongodb://" + server.ToString();
        }

        /*******************************************/

        public string DatabaseName()
        {
            if (m_Collection == null)
                return "";

            return m_Collection.Database.DatabaseNamespace.DatabaseName; 
        }

        /*******************************************/

        public string CollectionName()
        {
            if (m_Collection == null)
                return "";

            return m_Collection.CollectionNamespace.CollectionName; 
        }

        /*******************************************/

        public bool IsConnected()
        {
            return m_Client != null && m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private MongoClient m_Client;
        private IMongoDatabase m_Database;
        private IMongoCollection<BsonDocument> m_Collection = null;
        private IMongoCollection<BsonDocument> m_History = null;


        /***************************************************/
    }
}





