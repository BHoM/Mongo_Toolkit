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

        [Description("Creates a pair of MongoDB aggregation stages that filter documents where the specified field value falls within the given lower and upper bounds (inclusive). \n" +
            "Returns an $addFields stage to compute the domain check and a $match stage to filter on it.")]
        [Input("key", "List containing the document field reference to evaluate.")]
        [Input("upperbound", "List containing the upper bound value or expression.")]
        [Input("lowerbound", "List containing the lower bound value or expression.")]
        [Output("matchQuery", "A list of two MongoDB aggregation stage strings: an $addFields stage and a $match stage.")]
        public static List<string> MongoMatchDomain(List<object> key, List<object> upperbound, List<object> lowerbound)
        {
            List<string> matchquery = new List<string>();
            string domainexpression,matchexpression = "";
            string tempvar = "";
            domainexpression = "{$addFields: {matchdomain_"+ key[0].ToString() +": { $and: [ {$gte: [" + MongoCleanVariable(key, tempvar) + "," + MongoCleanVariable(lowerbound, tempvar) +"] },{$lte: [" + MongoCleanVariable(key, tempvar) + "," + MongoCleanVariable(upperbound, tempvar) + "] }] } } }";
            matchexpression = "{$match: {matchdomain_" + key[0].ToString() + " : true} }";
            matchquery.Add(domainexpression);
            matchquery.Add(matchexpression);
            return matchquery;
        }

        /***************************************************/
    }
}







