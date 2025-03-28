/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Reflection;
using BH.oM.Base.Attributes;
using BH.oM.Data.Requests;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Adapters.Mongo
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Interface                          ****/
        /***************************************************/

        [Description("Converts a given IRequest into Bson document.")]
        [Input("query", "Request to be converted to a Bson document.")]
        [Output("document", "Bson document that resulted from converting the input request.")]
        public static BsonDocument IToMongoQuery(this IRequest query)
        {
            return ToMongoQuery(query as dynamic);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts a given CustomRequest into Bson document.")]
        [Input("query", "Request to be converted to a Bson document.")]
        [Output("document", "Bson document that resulted from converting the input request.")]
        public static BsonDocument ToMongoQuery(this CustomRequest query)
        {
            return BsonDocument.Parse(query.Body);
        }

        /***************************************************/

        [Description("Converts a given FilterRequest into Bson document.")]
        [Input("query", "Request to be converted to a Bson document.")]
        [Output("document", "Bson document that resulted from converting the input request.")]
        public static BsonDocument ToMongoQuery(this FilterRequest query)
        {
            BsonDocument document = new BsonDocument();

            // Define the match
            if (query.Equalities == null)
                query.Equalities = new Dictionary<string, object>();

            BsonDocument equalities = query.Equalities.ToBsonDocument();

            if (query.Type != null)
            {
                BsonArray typeEqualities = new BsonArray();

                // Add requested type if it is not an interface
                if (!query.Type.IsInterface)
                    typeEqualities.Add(new BsonDocument { { "_t", query.Type.ToString() } });

                // Add subtypes of the requested type
                foreach (Type subtype in query.Type.Subtypes())
                {
                    typeEqualities.Add(new BsonDocument { { "_t", subtype.ToString() } });
                }

                equalities.Add(new BsonElement("$or", typeEqualities));
            }

            if (query.Tag != "")
                equalities["__Tag__"] = query.Tag;

            document.Add(new BsonElement("$match", equalities));

            return document;
        }

        /***************************************************/

        [Description("Converts a given IResultRequest into Bson document.")]
        [Input("query", "Request to be converted to a Bson document.")]
        [Output("document", "Bson document that resulted from converting the input request.")]
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
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static BsonDocument ToMongoQuery(this IRequest query)
        {
            return new BsonDocument();
        }

        /***************************************************/
    }
}






