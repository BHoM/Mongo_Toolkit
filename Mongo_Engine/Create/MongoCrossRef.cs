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
        public static string MongoCrossRef(string OtherColl, List<string> Props2Match, List<string> Props2Return, string ouputname)
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
            foreach (object x in Props2Match)
            {
                letstatement = letstatement + ", " + "othercollections_" +x+ ":\"$"+x+"\"";
            }

            //pipeline: [{$match: {$expr: {$and: [{$eq: [ "$Bar_Number",  "$$a" ] }]}}}, {project}],
            //------------------------------------                                -----           -
            foreach (string x in Props2Match)
            {
                equalitystatement = equalitystatement + ", {$eq: [" + "\"$" + x + "\" , \"$$" + "othercollections_" + x + "\"]}";
            }

            //pipeline: [{$match: {$expr: {$and: [{$eq: [ "$Bar_Number",  "$$a" ] }]}}}, {project}],
            //---------------------------------------------------------------------------         -          
            projectquery = Mongo.Create.MongoProject(Props2Return);

            string aggregatecommand = mongoExpressionA + OtherColl + mongoExpressionB + letstatement.Trim(',') + mongoExpressionC + equalitystatement.Trim(',') + mongoExpressionD + projectquery + mongoExpressionE + ouputname + mongoExpressionF;
            return aggregatecommand;
        }

    }
}