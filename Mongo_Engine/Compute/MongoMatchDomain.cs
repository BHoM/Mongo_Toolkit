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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Mongo
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

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

