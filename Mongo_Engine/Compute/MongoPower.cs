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
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Adapters.Mongo
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a MongoDB aggregation expression that raises the base to the power of the exponent.")]
        [Input("baseValue", "The base value or MongoDB field reference.")]
        [Input("exponent", "The exponent value or MongoDB field reference.")]
        [Output("mongoExpression", "A MongoDB $pow aggregation expression string.")]
        public static string MongoPower(object baseValue, object exponent)
        {
            string mongoExpression = "";
            List<object> powArray = new List<object> { baseValue, exponent };
            mongoExpression = "{$pow: [" + MongoCleanVariable(powArray, mongoExpression) + "] }";
            return mongoExpression;
        }

        /***************************************************/
    }
}






