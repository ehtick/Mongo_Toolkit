﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using BHC = BH.Adapter.Convert;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using BH.Adapter.Queries;
using BH.Adapter;

namespace BH.Adapter.Mongo
{
    public partial class MongoAdapter 
    {
        public IEnumerable<object> Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null)
        {
            // Check that the link is still alive
            if (m_Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected)
                return new List<object>();

            // Get the results
            var pipeline = query.Select(s => s.ToMongoQuery()).ToList();
            var aggregateOptions = new AggregateOptions() { AllowDiskUse = true };
            List<BsonDocument> result = m_Collection.Aggregate<BsonDocument>(pipeline, aggregateOptions).ToList();

            // Return as objects
            return result.Select(x => Convert.FromBson(x)).ToList<object>();
        }

    }
}
