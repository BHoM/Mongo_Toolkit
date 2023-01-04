/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

namespace BH.Engine.Adapters.Mongo
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static string MongoCrossRef(string otherColl, List<string> props2Match, List<string> props2Return, string ouputname)
        {
            //example mongo expression (4 main parts):
            //{$lookup: {from: "Sections Database",               let: { a: "$Bar_Number" },                 pipeline: [{$match: {$expr: {$and: [{$eq: [ "$Bar_Number",  "$$a" ] }]}}}, {project}],                 as: "holidays"}}])
            //           --What collection we cross refing--    ==common variables between both colls==     ******pipline operations to do on the second collectoin**********************        %%%%return result under what property%%%%%

            string mongoExpressionA = "{$lookup: {from: \"";
            string mongoExpressionB = "\", let: {";
            string letstatement = "";
            string mongoExpressionC = "}, pipeline: [{$match: {$expr: {$and: [";
            string equalitystatement = "";
            string mongoExpressionD = " ] } } }, ";
            string projectquery = "";
            string mongoExpressionE = " ], as: \"";
            string mongoExpressionF = "\"}}";

            //let: { a: "$Bar_Number", b: "$Force_Position" }
            foreach (object x in props2Match)
            {
                letstatement = letstatement + ", " + "othercollections_" +x+ ": \"$"+x+"\"";
            }

            //pipeline: [{$match: {$expr: {$and: [{$eq: [ "$Bar_Number",  "$$a" ] }]}}}, {project}],
            //------------------------------------                                -----           -
            foreach (string x in props2Match)
            {
                equalitystatement = equalitystatement + ", {$eq: [" + "\"$" + x + "\" , \"$$" + "othercollections_" + x + "\"]}";
            }

            //pipeline: [{$match: {$expr: {$and: [{$eq: [ "$Bar_Number",  "$$a" ] }]}}}, {project}],
            //---------------------------------------------------------------------------         -          
            projectquery = Mongo.Compute.MongoProject(props2Return);

            string aggregatecommand = mongoExpressionA + otherColl + mongoExpressionB + letstatement.Trim(',') + mongoExpressionC + equalitystatement.Trim(',') + mongoExpressionD + projectquery + mongoExpressionE + ouputname + mongoExpressionF;
            return aggregatecommand;
        }

        /***************************************************/
    }
}



