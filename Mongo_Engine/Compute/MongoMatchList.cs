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

        [Description("Creates a pair of MongoDB aggregation stages that filter documents where the specified field value is contained within the provided list. \n" +
            "Returns an $addFields stage to compute the membership check and a $match stage to filter on it.")]
        [Input("key", "The name of the document field to check for membership in the filter list.")]
        [Input("filter", "List of values to match the field against.")]
        [Output("aggregateCommand", "A list of two MongoDB aggregation stage strings: an $addFields stage and a $match stage.")]
        public static List<string> MongoMatchList(string key, List<object> filter)
        {
            string projectquery = "";
            string matchquery = "";
            string mongolist = "";
            string tempVar = "";
            List<string> aggregatecommand = new List<string>();
            mongolist = MongoCleanVariable(filter,tempVar);
  
             projectquery = "{$addFields: {isinfilterlist_" + key + " : {$in: [" + "\"$" + key + "\" , " + "[" + mongolist + "]] } } }"; 
             matchquery= "{$match: { " + "isinfilterlist_" + key  + " : true} }";

            aggregatecommand.Add(projectquery);
            aggregatecommand.Add(matchquery);

            return aggregatecommand;  
        }

        /***************************************************/
    }
}







