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

        [Description("Creates a MongoDB $group aggregation stage that groups documents by the specified fields and pushes the projected fields into arrays.")]
        [Input("groupBy", "List of field names to group documents by.")]
        [Input("projectForward", "List of field names to carry forward into the grouped output as arrays using $push.")]
        [Output("mongoExpression", "A MongoDB $group aggregation stage string.")]
        public static string MongoGroup(List<string> groupBy, List<string> projectForward)
        {
            //example mongo query:
            //{$group : {_id : { Bar_Number: "$Bar_Number", Node_Number: "$Force_Position"},P: {$push: "$F_X"},My: {$push: "$M_Y"},Mz: {$push: "$M_Z"},Bar_Number: {$push: "$Bar_Number"},Node_Number: {$push: "$Force_Position"},Data_Source: {$push: "$__Key__"}}}
            //                 ----This part is the group by portion-----------------------=================================This part is what to project forward (save on memory demand for group)=================================================================

            string aggregatecommand = "";
            string mongoExpressionA = "{$group: {_id : {";
            string mongoExpressionB = "";
            string mongoExpressionC = "},";
            string mongoExpressionD = "";
            string mongoExpressionE = " } }";

            foreach (string x in groupBy)
            {
                mongoExpressionB = mongoExpressionB + "," + x +" : "+ " \"$" + x + "\" ";
            }

            foreach (string x in projectForward)
            {
                mongoExpressionD = mongoExpressionD + "," + x +" : "+"{$push:"+ " \"$" + x + "\" }";
            }

            aggregatecommand = mongoExpressionA + mongoExpressionB.TrimStart(',') + mongoExpressionC + mongoExpressionD.TrimStart(',')+mongoExpressionE;

            return aggregatecommand;

        }

        /***************************************************/
    }
}






