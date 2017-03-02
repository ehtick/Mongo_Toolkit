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
using System.IO;
using System.Threading;

namespace Mongo_Adapter
{
    public class MongoServer
    {
        public MongoServer(string folderName)
        {
            if (m_Process != null && !m_Process.HasExited && m_FolderName != folderName)
                throw new Exception("A Mongo Server is already running you machine.");

            if (m_Process == null || m_Process.HasExited)
            {
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                m_FolderName = folderName;
                m_Process = System.Diagnostics.Process.Start("mongod", "--dbpath " + folderName);
                m_Process.Exited += M_Process_Exited;
                m_Process.Disposed += M_Process_Exited;

                Thread.Sleep(1000);
                m_Client = new MongoClient(@"mongodb://localhost:27017");
            }
        }

        public List<string> GetAllDatabases()
        {
            List<BsonDocument> bsonList = m_Client.ListDatabases().ToList();

            return bsonList.Select(x => x.GetElement("name").Value.ToString()).ToList();
        }


        public bool DeleteDatabase(string name)
        {
            m_Client.DropDatabase(name);
            return true;
        }

        private void M_Process_Exited(object sender, EventArgs e)
        {
            /*m_Process = null;
            m_FolderName = "";

            if (Killed != null)
                Killed.Invoke();*/

            // Doesn't work at the moment - This is never called
        }

        public event Action Killed; 

        private static string m_FolderName = "";
        private static MongoClient m_Client = null;
        private static System.Diagnostics.Process m_Process = null;
    }
}
