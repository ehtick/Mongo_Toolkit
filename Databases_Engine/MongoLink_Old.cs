﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Base;
using BHoM.Global;
using MongoDB.Bson;
using MongoDB.Driver;
using BHoM.Databases;

/*namespace Mongo_Adapter
{
    public class MongoLink_Old : IDatabaseAdapter
    {
        private IMongoCollection<BsonDocument> collection;
        private IMongoCollection<BsonDocument> depCollection;

        public MongoLink_Old(string serverLink = "mongodb://localhost:27017", string databaseName = "project", string collectionName = "bhomObjects")
        {
            var mongo = new MongoClient(serverLink);
            IMongoDatabase database = mongo.GetDatabase(databaseName);
            collection = database.GetCollection<BsonDocument>(collectionName);
            depCollection = database.GetCollection<BsonDocument>(collectionName + "__dep");
        }

        public string ServerName
        {
            get
            {
                MongoServerAddress server = collection.Database.Client.Settings.Server;
                return "mongodb://" + server.ToString();
            }
        }

        public string DatabaseName
        {
            get { return collection.Database.DatabaseNamespace.DatabaseName;  }
        }

        public string CollectionName
        {
            get { return collection.CollectionNamespace.CollectionName;  }
        }

        public bool PushObjects(IEnumerable<BHoMObject> objects, List<string> tags = null)
        {
            // Extract the key
            string key = "";
            if (tags != null && tags.Count > 0)
                key = tags[0];

            // Create the bulk query for the object to replace/insert
            List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
            bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("Key", key)));
            foreach (BHoMObject obj in objects)
                bulk.Add(new InsertOneModel<BsonDocument>(ToBson(obj, key)));

            // Send that query
            BulkWriteOptions bulkOptions = new BulkWriteOptions();
            bulkOptions.IsOrdered = true;
            collection.BulkWrite(bulk, bulkOptions);

            // Get all the dependencies
            Dictionary<Guid, BHoMObject> dependencies = new Dictionary<Guid, BHoMObject>();
            foreach (BHoMObject bhomObject in objects)
            {
                if (bhomObject == null) continue;
                foreach (KeyValuePair<Guid, BHoMObject> kvp in bhomObject.GetDeepDependencies())
                {
                    if (!dependencies.ContainsKey(kvp.Key))
                        dependencies[kvp.Key] = kvp.Value;
                }
            }

            // Create the bulk query for the dependencies to replace/insert
            List<WriteModel<BsonDocument>> depBulk = new List<WriteModel<BsonDocument>>();
            depBulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("Key", key)));
            foreach (BHoMObject obj in dependencies.Values)
                depBulk.Add(new InsertOneModel<BsonDocument>(ToBson(obj, key)));

            // Send that query
            depCollection.BulkWrite(depBulk, bulkOptions);

            return true;
        }

        public bool Push(IEnumerable<string> objects, List<string> tags = null)
        {
            // Extract the key
            string key = "";
            if (tags != null && tags.Count > 0)
                key = tags[0];

            // Create the bulk query for the object to replace/insert
            List<WriteModel<BsonDocument>> bulk = new List<WriteModel<BsonDocument>>();
            bulk.Add(new DeleteManyModel<BsonDocument>(Builders<BsonDocument>.Filter.Eq("Key", key)));
            foreach (string obj in objects)
                bulk.Add(new InsertOneModel<BsonDocument>(ToBson(obj, key)));

            // Send that query
            BulkWriteOptions bulkOptions = new BulkWriteOptions();
            bulkOptions.IsOrdered = true;
            collection.BulkWrite(bulk, bulkOptions);
            return true;
        }

        public bool Delete(string filterString)
        {
            FilterDefinition<BsonDocument> filter = filterString;
            collection.DeleteMany(filter);
            return true;
        }

        public void Clear()
        {
            collection.DeleteMany(new BsonDocument());
        }

        public IEnumerable<BHoMObject> GetObjects(string filterString)
        {
            // Add the queried objects to a temp project
            Project tempProject = new Project();
            FilterDefinition<BsonDocument> filter = filterString;
            var result = collection.Find(filter);
            var ret = result.ToList().Select(x => BHoMObject.FromJSON(x.ToString(), tempProject));
            foreach (BHoMObject obj in ret)
            {
                if (obj != null)
                    tempProject.AddObject(obj);
            }

            // Sort out the dependencies
            SortDependencies(tempProject);
            return ret;
        }

        public List<string> Pull(string filterString)
        {
            FilterDefinition<BsonDocument> filter = filterString;
            var result = collection.Find(filter);
            return result.ToList().Select(x => x.ToString()).ToList();
        }

        public IEnumerable<BHoMObject> QueryObjects(List<string> queryStrings)
        {
            // Add the queried objects to a temp project
            Project tempProject = new Project();
            var pipeline = queryStrings.Select(s => BsonDocument.Parse(s)).ToList();
            var aggregation = collection.Aggregate<BsonDocument>(pipeline);
            var ret = aggregation.ToList().Select(x => BHoMObject.FromJSON(x.ToString(), tempProject));
            foreach (BHoMObject obj in ret)
            {
                if (obj != null)
                    tempProject.AddObject(obj);
            }

            // Sort out the dependencies
            SortDependencies(tempProject);
            return ret;
        }

        public List<string> Query(List<string> queryStrings)
        {
            var pipeline = queryStrings.Select(s => BsonDocument.Parse(s)).ToList();
            var aggregation = collection.Aggregate<BsonDocument>(pipeline);
            return aggregation.ToList().Select(x => x.ToString()).ToList();
        }

        private BsonDocument ToBson(BHoMObject obj, string key)
        {
            var document = BsonDocument.Parse(obj.ToJSON());  //TODO: need to get a standards ToJSON on all objects, not just BHoMObjects
            if (key != "")
                document["Key"] = key;
            return document;
        }

        private BsonDocument ToBson(string obj, string key)
        {
            var document = BsonDocument.Parse(obj);
            if (key != "")
                document["Key"] = key;
            return document;
        }

        private void SortDependencies(Project tempProject)
        {
            var deps = depCollection.Find<BsonDocument>(Builders<BsonDocument>.Filter.In("Properties.BHoM_Guid", tempProject.GetTaskValues()));
            foreach (BsonDocument doc in deps.ToList())
                tempProject.AddObject(BHoMObject.FromJSON(doc.ToString(), tempProject));
            tempProject.RunTasks();
        }
    }
}*/
