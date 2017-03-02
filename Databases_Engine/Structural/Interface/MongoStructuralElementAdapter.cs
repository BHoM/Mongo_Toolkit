using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Base;
using BHoM.Structural.Elements;
using BHoM.Structural.Interface;
using BHoM.Structural.Loads;

namespace Mongo_Adapter.Structural.Interface
{
    public partial class MongoStructuralAdapter : IElementAdapter
    {
        public string Filename
        {
            get
            {
                return m_database.ServerName;
            }
        }

        public ObjectSelection Selection
        {
            get;
            set;
        }

        public List<string> GetObjects<T>(string collection, out List<T> outObjs, List<string> ids = null) where T:class, IBase
        {
            List<object> objs = m_database.Query(collection, new List<string>() { GetNameQueryString(ids) });
            if (objs == null)
            {
                outObjs = new List<T>();
                return new List<string>();
            }

            outObjs = objs.Select(x => x as T).ToList();
            return outObjs.Select(x => x.Name).ToList();
        }

        public List<string> GetBars(out List<Bar> bars, List<string> ids = null)
        {
            return GetObjects("Bars", out bars, ids);
        }

        public List<string> GetFEMeshes(out List<FEMesh> meshes, List<string> ids = null)
        {
            return GetObjects("FEMeshes", out meshes, ids);
        }

        public List<string> GetGrids(out List<Grid> grids, List<string> ids = null)
        {
            return GetObjects("Grids", out grids, ids);
        }

        public List<string> GetGroups(out List<IGroup> groups, List<string> ids = null)
        {
            return GetObjects("Groups", out groups, ids);
        }

        public List<string> GetLevels(out List<Storey> levels, List<string> ids = null)
        {
            return GetObjects("Storeys", out levels, ids);
        }

        public List<string> GetLoadcases(out List<ICase> cases)
        {
            return GetObjects("Loadcases", out cases);
        }

        public bool GetLoads(out List<ILoad> loads, List<Loadcase> ids = null)
        {
            GetObjects("Loads", out loads);
            return true;
        }

        public List<string> GetNodes(out List<Node> nodes, List<string> ids = null)
        {
            return GetObjects("Nodes", out nodes, ids);
        }

        public List<string> GetOpenings(out List<Opening> opening, List<string> ids = null)
        {
            return GetObjects("Openings", out opening, ids);
        }

        public List<string> GetPanels(out List<Panel> panels, List<string> ids = null)
        {
            return GetObjects("Panels", out panels, ids);
        }

        public List<string> GetRigidLinks(out List<RigidLink> links, List<string> ids = null)
        {
            return GetObjects("RigidLinks", out links, ids);
        }

        public bool Run()
        {
            throw new NotImplementedException();
        }

        public bool SetBars(List<Bar> bars, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("Bars", bars, "");
        }

        public bool SetFEMeshes(List<FEMesh> meshes, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("FEMeshes", meshes, "");
        }

        public bool SetGrids(List<Grid> grid, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("Grids", grid, "");
        }

        public bool SetGroups(List<IGroup> groups, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("Groups", groups, "");
        }

        public bool SetLevels(List<Storey> stores, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("Storeys", stores, "");
        }

        public bool SetLoadcases(List<ICase> cases)
        {
            return m_database.Push("Loadcases", cases, "");
        }

        public bool SetLoads(List<ILoad> loads)
        {
            return m_database.Push("Loads", loads, "");
        }

        public bool SetNodes(List<Node> nodes, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("Nodes", nodes, "");
        }

        public bool SetOpenings(List<Opening> opening, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("Openings", opening, "");
        }

        public bool SetPanels(List<Panel> panels, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("Panels", panels, "");
        }

        public bool SetRigidLinks(List<RigidLink> rigidLinks, out List<string> ids)
        {
            ids = new List<string>();
            return m_database.Push("RigidLinks", rigidLinks, "");
        }
    }
}
