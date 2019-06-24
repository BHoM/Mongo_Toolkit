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

using BH.oM.Data.Requests;
using MongoDB.Bson;

namespace BH.Engine.Mongo
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
    }
}
