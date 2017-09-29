using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using BHC = BH.Adapter.Convert;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using BH.Adapter.Queries;
using BH.Adapter;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        public override bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, string> config = null)
        {
            switch (command)
            {
                case "Transfer":
                    if (parameters != null && parameters.ContainsKey("Destination"))
                    {
                        bool replaceContent = false;
                        if (config != null && config.ContainsKey("Replace"))
                            bool.TryParse(config["Replace"], out replaceContent);
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
                                                this.GetDatabaseName() + "." + this.GetCollectionName() +
                                                "\", to:\"" +
                                                other.GetDatabaseName() + "." + other.GetCollectionName() +
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

    }
}
