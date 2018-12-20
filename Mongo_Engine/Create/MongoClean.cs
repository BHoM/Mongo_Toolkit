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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Mongo
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        //Need to clean variable from mongo operations.  PEMDAS functions can take in ints, doubles, and strings (mongo document properties or mongo expression strings)
        public static string MongoCleanVariable(List<object> cleanMe, string outputObject)
        {
            foreach (object item in cleanMe)
            {
                /// test first to see if the item can be cast as an integer or double
                bool testdouble = false;
                double itemdbl = new double();
                bool testint = false;
                double itemint = new int();
                testdouble = double.TryParse(item.ToString(), out itemdbl);
                testint = double.TryParse(item.ToString(), out itemint);

                if (item is double || item is int || testdouble || testint) //if you give me a number, then we'll treat it as a number
                    outputObject = outputObject + "," + item.ToString();
                else if (item is string && item.ToString().StartsWith("{")) //a mongo expression is a JSON object and will start with a {
                    outputObject = outputObject + "," + item.ToString();
                else if (item is string && !item.ToString().StartsWith("{"))
                    outputObject = outputObject + "," + "\"$" + item.ToString() + "\""; //mongo document properties used as vairables must be preceded with a dollar sign
                else if (item is List<object>)
                    outputObject = MongoCleanVariable((List<object>)item, outputObject); //recursive definition to handle list of lists
            }
            return outputObject.TrimStart(',');
        }
    }
}