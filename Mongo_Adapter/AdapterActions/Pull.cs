/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using MongoDB.Bson;
using MongoDB.Driver;
using BH.oM.Data.Requests;
using BH.Engine.Adapters.Mongo;
using BH.oM.Adapter;
using BH.oM.Adapters.Mongo.Requests;
using BH.oM.Adapters.Mongo;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override IEnumerable<object> Pull(IRequest query, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            MongoConfig mongoConfig = new MongoConfig();
            if(actionConfig!=null)
            {
                if (actionConfig is MongoConfig)
                    mongoConfig = actionConfig as MongoConfig;
                else
                {
                    BH.Engine.Base.Compute.RecordError("ActionConfig provided is not a MongoConfig.");
                    return new List<object>();
                }
                    
            }
            // Check that the link is still alive
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<object>();

            // Get the results
            List<BsonDocument> pipeline = new List<BsonDocument>();

            if (query is CollectionNames)
            {
                List<string> collectionNames = new List<string>();
                if (m_Database != null)
                {
                    foreach (BsonDocument doc in m_Database.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
                    {
                        if (doc.Contains("name"))
                            collectionNames.Add(doc["name"].AsString);
                    }
                }
                return collectionNames;
            }
            else if (query is BatchRequest)
                pipeline = ((BatchRequest)query).Requests.Select(s => s.IToMongoQuery()).ToList();
            else
                pipeline.Add(query.IToMongoQuery());
            AggregateOptions aggregateOptions = new AggregateOptions() { AllowDiskUse = true };
            List<BsonDocument> result = m_Collection.Aggregate<BsonDocument>(pipeline, aggregateOptions).ToList();

            // Return as objects
            switch (mongoConfig.ResultType)
            {
                case Mongo_oM.Adapter.ResultType.Bhom:
                    return result.Select(x => Engine.Adapters.Mongo.Convert.FromBson(x)).ToList<object>();
                case Mongo_oM.Adapter.ResultType.Bson:
                    return result;
                case Mongo_oM.Adapter.ResultType.Json:
                    return result.ConvertAll(BsonTypeMapper.MapToDotNetValue);
                default:
                    return result.Select(x => Engine.Adapters.Mongo.Convert.FromBson(x)).ToList<object>();
            }
        }


        /***************************************************/
    }
}


