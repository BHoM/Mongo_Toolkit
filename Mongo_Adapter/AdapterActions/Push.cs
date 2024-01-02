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

using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using BH.oM.Data.Requests;
using BH.oM.Adapter;
using BH.oM.Adapters.Mongo;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override List<object> Push(IEnumerable<object> objects, string tag = "", PushType pushType = PushType.AdapterDefault, ActionConfig actionConfig = null)
        {
            // Check that the link is still alive
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<object>();

            // Create the bulk query for the object to replace/insert
            DateTime timestamp = DateTime.Now;
            IEnumerable<BsonDocument> documents = objects.Select(x => Engine.Adapters.Mongo.Convert.ToBson(x, tag, timestamp));

            if (pushType == PushType.DeleteThenCreate)
            {
                List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
                bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Tag__", tag)));
                foreach (BsonDocument doc in documents)
                    bulk.Add(new InsertOneModel<BsonDocument>(doc));
                m_Collection.BulkWrite(bulk);
            }
            else if (pushType == PushType.UpdateOrCreateOnly)
            {
                var bulkOps = new List<WriteModel<BsonDocument>>();
                foreach (BsonDocument doc in documents)
                {
                    var newDoc = new BsonDocument { { "$set", new BsonDocument(doc) } };
                    var upsertOne = new UpdateOneModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("__Tag__", tag), newDoc) { IsUpsert = true };
                    bulkOps.Add(upsertOne);
                }
                m_Collection.BulkWrite(bulkOps);
            }
            else
                m_Collection.InsertMany(documents);

            if (pushType != PushType.AdapterDefault && pushType != PushType.DeleteThenCreate && pushType != PushType.CreateOnly && pushType != PushType.UpdateOrCreateOnly)
                BH.Engine.Base.Compute.RecordNote($"{this.GetType().Name} only supports the following {nameof(PushType)}s:" +
                    $"\n\t- {nameof(PushType.CreateOnly)} => appends content (default setting)" +
                    $"\n\t- {nameof(PushType.DeleteThenCreate)} => replaces all content" +
                    $"\n\t- {nameof(PushType.UpdateOrCreateOnly)} => upsert" +
                    $"\nEvery other {nameof(PushType)} will behave as {nameof(PushType.CreateOnly)}.");

            // Push in the history database as well
            if (m_History != null)
            {
                BatchRequest queries = new BatchRequest
                {
                    Requests = new List<IRequest> {
                    new CustomRequest { Body = "{$group: {_id: \"$__Time__\"}}" },
                    new CustomRequest { Body = "{$sort: {_id: -1}}" }
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





