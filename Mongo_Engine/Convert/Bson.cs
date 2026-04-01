/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using MongoDB.Bson;
using System;
using System.ComponentModel;


namespace BH.Engine.Adapters.Mongo
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts a BHoM object to a BsonDocument and stamps it with a tag and timestamp.")]
        [Input("obj", "The object to serialise to BSON.")]
        [Input("tag", "A string tag to associate with the document, stored under the '__Tag__' key.")]
        [Input("timestamp", "The timestamp to associate with the document, stored under the '__Time__' key.")]
        [Output("document", "A BsonDocument representation of the object with tag and timestamp fields.")]
        public static BsonDocument ToBson(object obj, string tag, DateTime timestamp)
        {
            BsonDocument document = Engine.Serialiser.Convert.ToBson(obj);
            document["__Tag__"] = tag;
            document["__Time__"] = timestamp;

            return document;
        }

        /*******************************************/

        [Description("Converts a BHoM object to a BsonDocument and stamps it with a tag.")]
        [Input("obj", "The object to serialise to BSON.")]
        [Input("tag", "A string tag to associate with the document, stored under the '__Tag__' key.")]
        [Output("document", "A BsonDocument representation of the object with a tag field.")]
        public static BsonDocument ToBson(object obj, string tag)
        {
            BsonDocument document = Engine.Serialiser.Convert.ToBson(obj);
            document["__Tag__"] = tag;

            return document;
        }

        /*******************************************/

        [Description("Converts a BsonDocument to a BHoM object, removing the MongoDB metadata fields '__Tag__' and '__Time__' before deserialisation.")]
        [Input("document", "The BsonDocument to convert.")]
        [Output("obj", "The deserialised BHoM object.")]
        public static object FromBson(this BsonDocument document)
        {
            document.Remove("__Tag__");
            document.Remove("__Time__");
            return Engine.Serialiser.Convert.FromBson(document);
        }

        /***************************************************/
    }
}







