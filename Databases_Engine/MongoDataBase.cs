using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using System.IO;

namespace Mongo_Adapter
{
    //public class MongoDataBase : IDatabaseAdapter
    //{
    //    private Dictionary<string, MongoLink> m_links;

    //    public MongoDataBase(string serverLink = "mongodb://localhost:27017", string databaseName = "project")
    //    {
    //        m_links = new Dictionary<string, MongoLink>();
    //        DatabaseName = databaseName;
    //        ServerName = serverLink;
    //    }
    //    public string DatabaseName
    //    {
    //        get;
    //        private set;
    //    }

    //    public string ServerName
    //    {
    //        get;
    //        private set;
    //    }

    //    private MongoLink GetLink(string collection)
    //    {
    //        if (m_links.ContainsKey(collection))
    //            return m_links[collection];
    //        else
    //        {
    //            MongoLink link = new MongoLink(ServerName, DatabaseName, collection);
    //            m_links.Add(collection, link);
    //            return link;
    //        }

    //    }
    //    public bool Delete(string collection, string filterString = "")
    //    {
    //        MongoLink link = GetLink(collection);
    //        return link.Delete(filterString);
    //    }

    //    public List<object> Pull(string collection, string filterString = "", bool keepAsString = false)
    //    {
    //        MongoLink link = GetLink(collection);
    //        return link.Pull(filterString, keepAsString);
    //    }

    //    public bool Push(string collection, IEnumerable<object> objects, string key, List<string> tags = null)
    //    {
    //        MongoLink link = GetLink(collection);
    //        return link.Push(objects, key, tags);
    //    }

    //    public List<object> Query(string collection, List<string> queryStrings = null, bool keepAsString = false)
    //    {
    //        MongoLink link = GetLink(collection);
    //        return link.Query(queryStrings, keepAsString);
    //    }


    //}
}
