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
using System.Linq;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using BH.oM.DataManipulation.Queries;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            // Check that the link is still alive
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<IObject>();

            // Get the config
            bool replace = true;
            if (config != null)
            {
                if (config.ContainsKey("Tag"))
                    tag = config["Tag"] as string;
                if (config.ContainsKey("Replace"))
                    bool.TryParse(config["Replace"] as string, out replace);
            }

            // Create the bulk query for the object to replace/insert
            DateTime timestamp = DateTime.Now;
            IEnumerable<BsonDocument> documents = objects.Select(x => Engine.Mongo.Convert.ToBson(x, tag, timestamp));
            if (replace)
            {
                List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
                bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Tag__", tag)));
                foreach (BsonDocument doc in documents)
                    bulk.Add(new InsertOneModel<BsonDocument>(doc));
                m_Collection.BulkWrite(bulk);
            }
            else
                m_Collection.InsertMany(documents);

            // Push in the history database as well
            if (m_History != null)
            {
                BatchQuery queries = new BatchQuery
                {
                    Queries = new List<IQuery> {
                    new CustomQuery { Query = "{$group: {_id: \"$__Time__\"}}" },
                    new CustomQuery { Query = "{$sort: {_id: -1}}" }
                }
                };
                List<object> times = Pull(queries) as List<object>;
                if (times.Count > HistorySize)
                    m_History.DeleteMany(Builders<BsonDocument>.Filter.Lte("__Time__", times[HistorySize]));
                m_History.InsertMany(documents);
            }
            return objects.ToList();
        }

        /***************************************************/
    }
}
