/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Adapters.Mongo.Commands;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override Output<List<object>, bool> Execute(IExecuteCommand command, ActionConfig actionConfig = null)
        {
            Output<List<object>, bool> result = new Output<List<object>, bool> { Item1 = null, Item2 = false };

            Transfer transferCmd = command as Transfer;
            MongoAdapter destination = transferCmd.Destination as MongoAdapter;

            if (transferCmd != null && destination != null)
            {
                result.Item2 = MoveCollection(destination, transferCmd.ReplaceContent);

                return result;
            }
            else
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
                DatabaseNamespace adminDBNameSpace = DatabaseNamespace.Admin;
                IMongoDatabase adminDb = m_Client.GetDatabase(adminDBNameSpace.DatabaseName);

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



