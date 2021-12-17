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

using BH.oM.Data.Requests;
using MongoDB.Bson;
using System.Linq;

namespace BH.Engine.Adapters.Mongo
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Interface                           ****/
        /***************************************************/

        public static BsonDocument IToMongoQuery(this IRequest query)
        {
            return ToMongoQuery(query as dynamic);
        }


        /***************************************************/
        /**** Private  Methods - Curves                 ****/
        /***************************************************/

        public static BsonDocument ToMongoQuery(this BH.oM.Adapters.Mongo.Requests.CustomRequest query)
        {
            return BsonDocument.Parse(query.Body);
        }

        /***************************************************/

        public static BsonDocument ToMongoQuery(this CustomRequest query)
        {
            return BsonDocument.Parse(query.Body);
        }

        /***************************************************/

        public static BsonDocument ToMongoQuery(this FilterRequest query)
        {
            BsonDocument document = new BsonDocument();

            // Define the match
            BsonDocument equalities = query.Equalities.ToBsonDocument();

            if (query.Type != null)
                equalities["_t"] = query.Type.ToString();
            if (query.Tag != "")
                equalities["__Tag__"] = query.Tag;

            document.Add(new BsonElement("$match", equalities));

            return document;
        }

        /***************************************************/

        public static BsonDocument ToMongoQuery(this IResultRequest query)
        {
            BsonDocument document = new BsonDocument();

            // Define the match
            BsonDocument equalities = new BsonDocument();

            if (query.Cases != null && query.Cases.Count > 0)
            {
                BsonDocument cases = new BsonDocument();
                cases["$in"] = new BsonArray(query.Cases);
                equalities["ResultCase"] = cases;
            }
            if (query.ObjectIds != null && query.ObjectIds.Count > 0)
            {
                BsonDocument ids = new BsonDocument();
                ids["$in"] = new BsonArray(query.ObjectIds);
                equalities["ObjectId"] = ids;
            }

            document.Add(new BsonElement("$match", equalities));

            return document;
        }


        /***************************************************/

        private static BsonDocument ToMongoQuery(this IRequest query)
        {
            return new BsonDocument();
        }

        /***************************************************/
    }
}


