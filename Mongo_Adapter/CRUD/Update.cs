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
using MongoDB.Bson;
using MongoDB.Driver;
using BH.oM.Data.Requests;
using BH.Engine.Mongo;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override int UpdateProperty(FilterRequest filter, string property, object newValue, Dictionary<string, object> config = null)
        {
            BsonDocument definition = new BsonDocument();
            definition["__Time__"] = DateTime.Now;
            definition[property] = GetBsonValue(newValue);

            BsonDocument setter = new BsonDocument { { "$set", definition } };
            BsonDocument request = filter.ToMongoQuery().GetElement("$match").Value.AsBsonDocument;

            UpdateResult result = m_Collection.UpdateMany(request, setter);
            return (int)result.ModifiedCount;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private BsonValue GetBsonValue(object value)
        {
            if (value == null)
                return BsonNull.Value;

            Type type = value.GetType();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return new BsonBoolean((bool)value);

                case TypeCode.DateTime:
                    return new BsonDateTime(BsonUtils.ToUniversalTime((DateTime)value));

                case TypeCode.Double:
                    return new BsonDouble((double)value);

                case TypeCode.Int16:
                    return new BsonInt32((short)value);

                case TypeCode.Int32:
                    return new BsonInt32((int)value);

                case TypeCode.Int64:
                    return new BsonInt64((long)value);

                case TypeCode.Object:
                    if (type == typeof(Decimal128))
                        return new BsonDecimal128((Decimal128)value);
                    else if (type == typeof(Guid))
                        return new BsonBinaryData((Guid)value, GuidRepresentation.Standard);
                    else if (type == typeof(ObjectId))
                        return new BsonObjectId((ObjectId)value);
                    else
                        return value.ToBson();

                case TypeCode.String:
                    return new BsonString((string)value);

                default:
                    return value.ToBson();
            }
        }

        /***************************************************/
    }
}
