using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Base;
using BHoM.Global;
using MongoDB.Bson;
using MongoDB.Driver;
using BHoM.Databases;
using System.Text.RegularExpressions;
using System.IO;
using BHoM.Structural.Interface;
using BHoM.Base.Results;
using BHoM.Structural.Results;

namespace Mongo_Adapter.Structural.Interface
{
    public partial class MongoStructuralAdapter : IResultAdapter
    {
        private MongoDataBase m_database;

        public MongoStructuralAdapter(MongoDataBase database)
        {
            m_database = database;
        }

        public static string GetMultiOptionQueryString(List<string> values, string propName)
        {
            if (values == null || values.Count < 1)
                return "{$match: { } }";
            else
            {
                string str = "{$match: { $or: [";

                foreach (string ca in values)
                {
                    str += "{ " + propName + " : \"" + ca + "\" },";
                }
                str.Trim(',');
                str += "]}}";
                return str;
            }
        }

        public static string GetSortString()
        {
            return "{$sort: {Name : 1, Loadcase: 1, ForcePosition: 1 } }";
        }
        public static string GetLoadCaseQueryString(List<string> cases)
        {
            return GetMultiOptionQueryString(cases, "Loadcase");
        }

        public static string GetNameQueryString(List<string> names)
        {
            return GetMultiOptionQueryString(names, "Name");
        }

        public static List<string> GetLoadcaseAndNameQueryString(List<string> cases, List<string> names)
        {
            string caseString = GetLoadCaseQueryString(cases);
            string nameString = GetNameQueryString(names);

            List<string> queryStrings = new List<string>();


            if (caseString == null && nameString == null)
                queryStrings.Add("{$match: { } }");
            else
            {
                if (caseString != null)
                    queryStrings.Add(caseString);

                if (nameString != null)
                    queryStrings.Add(nameString);
            }
            //queryStrings.Add(GetSortString());
            return queryStrings;
        }

        private Dictionary<string, IResultSet> GetResultSet<T>(List<T> values, ResultOrder resultOrder) where T : IResult, new()
        {
            values.Sort();
            Dictionary<string, IResultSet> results = new Dictionary<string, IResultSet>();
            IResultSet rSet = null;// new ResultSet<T>();
            if (resultOrder == ResultOrder.None)
            {
                rSet = new ResultSet<T>();
                (rSet as ResultSet<T>).AddData(values);
                results.Add("All", rSet);
            }
            else
            {
                int orderCol = new T().ColumnHeaders.ToList().IndexOf(resultOrder.ToString());

                for (int i = 0; i < values.Count; i++)
                {
                    string key = values[i].GetData()[orderCol].ToString();
                    if (!results.TryGetValue(key, out rSet))
                    {
                        rSet = new ResultSet<T>();
                        results.Add(key, rSet);
                    }
                    rSet.AddData(values[i].GetData());
                }
            }

            return results;
        }

        public bool GetResult<T>(string collection, List<string> ids, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results) where T:class, IResult, new()
        {
            List<object> objs = m_database.Query(collection, GetLoadcaseAndNameQueryString(cases, ids));
            if (objs == null)
            {
                results = new Dictionary<string, IResultSet>();
                return false;
            }
            results = GetResultSet(objs.Select(x => x as T).ToList(), orderBy);
            return true;
        }

        public bool GetBarForces(List<string> bars, List<string> cases, int divisions, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<BarForce>("BarForces", bars, cases, orderBy, out results);     
        }

        public bool GetBarCoordinates(List<string> bars, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }
        public bool GetBarStresses(List<string> bars, List<string> cases, int divisions, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<BarStress>("BarStresses", bars, cases, orderBy, out results);
        }

        public bool GetBarUtilisation(List<string> bars, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<SteelUtilisation>("BarUtilisation", bars, cases, orderBy, out results);
        }

        public bool GetModalResults()
        {
            throw new NotImplementedException();
        }

        public bool GetNodeAccelerations(List<string> nodes, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<NodeAcceleration>("NodeAccelerations", nodes, cases, orderBy, out results);
        }

        public bool GetNodeCoordinates(List<string> nodes, out Dictionary<string, IResultSet> results)
        {
            throw new NotImplementedException();
        }

        public bool GetNodeDisplacements(List<string> nodes, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<NodeDisplacement>("NodeDisplacements", nodes, cases, orderBy, out results);
        }

        public bool GetNodeReactions(List<string> nodes, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<NodeReaction>("NodeReactions", nodes, cases, orderBy, out results);
        }

        public bool GetNodeVelocities(List<string> nodes, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<NodeVelocity>("NodeVelocities", nodes, cases, orderBy, out results);
        }

        public bool GetPanelForces(List<string> panels, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<PanelForce>("PanelForces", panels, cases, orderBy, out results);
        }

        public bool GetPanelStress(List<string> panels, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<PanelStress>("PanelStresses", panels, cases, orderBy, out results);
        }

        public bool GetSlabReinforcement(List<string> panels, List<string> cases, ResultOrder orderBy, out Dictionary<string, IResultSet> results)
        {
            return GetResult<SlabReinforcement>("SlabReinforcements", panels, cases, orderBy, out results);
        }

        public bool PushToDataBase(IDatabaseAdapter dbAdapter, List<ResultType> resultTypes, List<string> loadcases, string key, bool append = false)
        {
            foreach (ResultType type in resultTypes)
            {
                Dictionary<string, IResultSet> results = new Dictionary<string, IResultSet>();
                switch (type)
                {
                    case ResultType.Undefined:
                        break;
                    case ResultType.NodeDisplacement:
                        GetNodeDisplacements(null, loadcases, ResultOrder.Name, out results);
                        dbAdapter.Push("NodeDisplacements", results.Values.Select(x => x.AsList<NodeDisplacement>()).ToList(), "");
                        break;
                    case ResultType.NodeReaction:
                        GetNodeReactions(null, loadcases, ResultOrder.Name, out results);
                        dbAdapter.Push("NodeReactions", results.Values.Select(x => x.AsList<NodeReaction>()).ToList(), "");
                        break;
                    case ResultType.NodeVelocity:
                        GetNodeVelocities(null, loadcases, ResultOrder.Name, out results);
                        dbAdapter.Push("NodeVelocities", results.Values.Select(x => x.AsList<NodeVelocity>()).ToList(), "");
                        break;
                    case ResultType.NodeAcceleration:
                        GetNodeAccelerations(null, loadcases, ResultOrder.Name, out results);
                        dbAdapter.Push("NodeAccelerations", results.Values.Select(x => x.AsList<NodeAcceleration>()).ToList(), "");
                        break;
                    case ResultType.BarForce:
                        GetBarForces(null, loadcases, 5, ResultOrder.Name, out results);
                        dbAdapter.Push("BarForces", results.Values.Select(x => x.AsList<BarForce>()).ToList(), "");
                        break;
                    case ResultType.BarStress:
                        GetBarStresses(null, loadcases, 5, ResultOrder.Name, out results);
                        dbAdapter.Push("BarStresses", results.Values.Select(x => x.AsList<BarStress>()).ToList(), "");
                        break;
                    case ResultType.PanelForce:
                        GetPanelForces(null, loadcases, ResultOrder.Name, out results);
                        dbAdapter.Push("PanelForces", results.Values.Select(x => x.AsList<PanelForce>()).ToList(), "");
                        break;
                    case ResultType.PanelStress:
                        GetPanelStress(null, loadcases, ResultOrder.Name, out results);
                        dbAdapter.Push("PanelStresses", results.Values.Select(x => x.AsList<PanelStress>()).ToList(), "");
                        break;
                    case ResultType.Modal:
                        break;
                    case ResultType.Utilisation:
                        GetBarUtilisation(null, loadcases, ResultOrder.Name, out results);
                        dbAdapter.Push("BarUtilisations", results.Values.Select(x => x.AsList<SteelUtilisation>()).ToList(), "");
                        break;
                    case ResultType.SlabReinforcement:
                        GetSlabReinforcement(null, loadcases, ResultOrder.Name, out results);
                        dbAdapter.Push("SlabReinforcements", results.Values.Select(x => x.AsList<SlabReinforcement>()).ToList(), "");
                        break;
                    case ResultType.NodeCoordinates:
                        break;
                    case ResultType.BarCoordinates:
                        break;
                    default:
                        break;
                }
            }

            return true;
        }

        public bool StoreResults(string filename, List<ResultType> resultTypes, List<string> loadcases, bool append = false)
        {
            throw new NotImplementedException();
        }
    }
}
