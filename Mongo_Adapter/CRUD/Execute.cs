using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, object> config = null)
        {
            switch (command)
            {
                case "Transfer":
                    if (parameters != null && parameters.ContainsKey("Destination"))
                    {
                        bool replaceContent = false;
                        if (config != null && config.ContainsKey("Replace"))
                            bool.TryParse(config["Replace"] as string, out replaceContent);
                        return MoveCollection(parameters["Destination"] as MongoAdapter, replaceContent);
                    }
                    break;
            }

            throw new NotImplementedException(command + " is not a recognised command for the Mongo adapter.");
        }


        /*******************************************/
        /****  Execute Methods                  ****/
        /*******************************************/

        public bool MoveCollection(MongoAdapter other, bool replaceContent = true)
        {
            try
            {
                //Access the admin namespace and admin db needed to be able to rename collections
                var adminDBNameSpace = DatabaseNamespace.Admin;
                var adminDb = m_Client.GetDatabase(adminDBNameSpace.DatabaseName);

                //Create the renaming command
                Command<BsonDocument> command = "{ renameCollection: \"" +
                                                this.DatabaseName() + "." + this.CollectionName() +
                                                "\", to:\"" +
                                                other.DatabaseName() + "." + other.CollectionName() +
                                                "\", dropTarget:\"" + replaceContent.ToString() + "\"}";

                //Execute command
                adminDb.RunCommand(command);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /***************************************************/
    }
}
