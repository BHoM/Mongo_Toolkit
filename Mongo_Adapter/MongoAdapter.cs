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

        public MongoAdapter(string serverName = "mongodb://localhost", int port = 27017, string databaseName = "project", string collectionName = "bhomObjects")
        {
            AdapterId = "Mongo_id";

            if (!serverName.StartsWith("mongodb://"))
                serverName = "mongodb://" + serverName;

            m_Client = new MongoClient(serverName + ":" + port.ToString());
            IMongoDatabase database = m_Client.GetDatabase(databaseName);
            m_Collection = database.GetCollection<BsonDocument>(collectionName);

            IMongoDatabase hist_Database = m_Client.GetDatabase(databaseName + "_History");
            m_History = hist_Database.GetCollection<BsonDocument>(collectionName);
        }

        /***************************************************/

        public MongoAdapter(string connectionString, string databaseName = "project", string collectionName = "bhomObjects")
        {

            AdapterId = "Mongo_id";
            if (!connectionString.StartsWith("mongodb://"))
                connectionString = "mongodb://" + connectionString;

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            m_Client = new MongoClient(settings);

            IMongoDatabase database = m_Client.GetDatabase(databaseName);
            m_Collection = database.GetCollection<BsonDocument>(collectionName);

            IMongoDatabase hist_Database = m_Client.GetDatabase(databaseName + "_History");
            m_History = hist_Database.GetCollection<BsonDocument>(collectionName);
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
        private IMongoCollection<BsonDocument> m_Collection;
        private IMongoCollection<BsonDocument> m_History;


        /***************************************************/
    }
}
