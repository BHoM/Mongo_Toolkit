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
        public static string MongoProject(List<string> Filter)
        {
            string aggregatecommand = "";
            string mongoExpressionA = "{$project: {";
            string mongoExpressionB = "";
            string mongoExpressionC = "} }";

            //example mongo query:
            // {$project: { Loadcase: 1, Bar_Number: 1, F_X: 1,M_Y: 1, M_Z: 1, __Key__: 1, Force_Position: 1, isInBarList: {$in: ["$Bar_Number", [19931]]}, isInLoadCaseList: {$in: ["$Loadcase", [1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102]]}}}
            foreach (object x in Filter)
            {
                mongoExpressionB = mongoExpressionB + "," + x + ":1";
            }

            aggregatecommand = mongoExpressionA + mongoExpressionB.TrimStart(',') + mongoExpressionC;

            return aggregatecommand;
            }

    }
}
