using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using BH.Adapter.Queries;
using BH.Adapter;

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

        public MongoAdapter(string serverName = "mongodb://localhost", int port = 27017, string databaseName = "project", string collectionName = "bhomObjects")
        {
            if (!serverName.StartsWith("mongodb://"))
                serverName = "mongodb://" + serverName;

            m_Client = new MongoClient(serverName + ":" + port.ToString());
            IMongoDatabase database = m_Client.GetDatabase(databaseName);
            m_Collection = database.GetCollection<BsonDocument>(collectionName);

            IMongoDatabase hist_Database = m_Client.GetDatabase(databaseName + "_History");
            m_History = hist_Database.GetCollection<BsonDocument>(collectionName);
        }


        /***************************************************/
        /**** Public Getter Methods                     ****/
        /***************************************************/

        public string GetServerName()
        {
            MongoServerAddress server = m_Collection.Database.Client.Settings.Server;
            return "mongodb://" + server.ToString();
        }

        /*******************************************/

        public string GetDatabaseName()
        {
            return m_Collection.Database.DatabaseNamespace.DatabaseName; 
        }

        /*******************************************/

        public string GetCollectionName()
        {
            return m_Collection.CollectionNamespace.CollectionName; 
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private MongoClient m_Client;
        private IMongoCollection<BsonDocument> m_Collection;
        private IMongoCollection<BsonDocument> m_History;

    }
}
