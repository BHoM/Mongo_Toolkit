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

using System.Collections.Generic;
using MongoDB.Driver;
using BH.oM.Data.Requests;
using BH.Engine.Adapters.Mongo;
using MongoDB.Bson;
using BH.oM.Adapter;
using System.Linq;
using BH.oM.Adapters.Mongo;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override bool Move(IBHoMAdapter to, IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig pullConfig = null, PushType pushType = PushType.AdapterDefault, ActionConfig pushConfig = null)
        {
            if (!(to is MongoAdapter))
                return base.Move(to, request, pullType, pullConfig, pushType, pushConfig);

            // Check that the links are still alive
            MongoAdapter toMongo = to as MongoAdapter;
            if (!IsConnected() || !toMongo.IsConnected())
                return false;

            // Get the results
            List<BsonDocument> pipeline = new List<BsonDocument>();
            if (request is BatchRequest)
                pipeline = ((BatchRequest)request).Requests.Select(s => s.IToMongoQuery()).ToList();
            else
                pipeline.Add(request.IToMongoQuery());

            AggregateOptions aggregateOptions = new AggregateOptions() { AllowDiskUse = true };
            List<BsonDocument> result = m_Collection.Aggregate<BsonDocument>(pipeline, aggregateOptions).ToList();

            // Get the push config
            bool replace = true;
            string tag = "";
            MongoConfig mongoConfig = pushConfig as MongoConfig;
            if (mongoConfig != null)
            {
                replace = mongoConfig.Replace;
                tag = mongoConfig.Tag;
            }

            // Push to the destination 
            toMongo.PushBson(result, tag, replace);
            return true;
        }

        /***************************************************/
    }
}


