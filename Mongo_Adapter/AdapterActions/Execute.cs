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

using System;
using System.Collections.Generic;
using BH.oM.Adapter;
using BH.oM.Adapter.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override bool Execute(string command, Dictionary<string, object> parameters = null, ActionConfig actionConfig = null)
        {
            switch (command)
            {
                case "Transfer":
                    if (parameters != null && parameters.ContainsKey("Destination"))
                    {
                        // Get the config
                        bool replaceContent = false;

                        MongoConfig mongoConfig = actionConfig as MongoConfig;
                        if (actionConfig != null)
                            replaceContent = mongoConfig.Replace;

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
