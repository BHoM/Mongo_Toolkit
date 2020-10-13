/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using MongoDB.Bson;
using System;


namespace BH.Engine.Adapters.Mongo
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BsonDocument ToBson(object obj, string tag, DateTime timestamp)
        {
            BsonDocument document = Engine.Serialiser.Convert.ToBson(obj);
            document["__Tag__"] = tag;
            document["__Time__"] = timestamp;

            return document;
        }

        /*******************************************/

        public static BsonDocument ToBson(object obj, string tag)
        {
            BsonDocument document = Engine.Serialiser.Convert.ToBson(obj);
            document["__Tag__"] = tag;

            return document;
        }

        /*******************************************/

        public static object FromBson(BsonDocument document)
        {
            document.Remove("__Tag__");
            document.Remove("__Time__");
            return Engine.Serialiser.Convert.FromBson(document);
        }

        /***************************************************/
    }
}

