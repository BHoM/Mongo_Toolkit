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
        public static string MongoGroup(List<string> GroupBy, List<string> ProjectForward)
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
            string tempVar = "";

            foreach (string x in GroupBy)
            {
                mongoExpressionB = mongoExpressionB + "," + x +" : "+ " \"$" + x + "\" ";
            }

            foreach (string x in ProjectForward)
            {
                mongoExpressionD = mongoExpressionD + "," + x +" : "+"{$push:"+ " \"$" + x + "\" }";
            }

            aggregatecommand = mongoExpressionA + mongoExpressionB.TrimStart(',') + mongoExpressionC + mongoExpressionD.TrimStart(',')+mongoExpressionE;

            return aggregatecommand;

        }

    }
}