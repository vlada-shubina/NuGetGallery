﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NuGetGallery.Operations
{
    public class DeploymentEnvironment
    {
        public SqlConnectionStringBuilder MainDatabase { get; private set; }

        public DeploymentEnvironment(IDictionary<string, string> deploymentSettings)
        {
            MainDatabase = new SqlConnectionStringBuilder(deploymentSettings["Gallery.Sql.NuGetGallery"]);
        }

        public static DeploymentEnvironment FromConfigFile(string configFile)
        {
            // Load the file
            var doc = XDocument.Load(configFile);

            // Build a dictionary of settings
            var settings = BuildSettingsDictionary(doc);

            // Construct the deployment environment
            return new DeploymentEnvironment(settings);
        }

        private static IDictionary<string, string> BuildSettingsDictionary(XDocument doc)
        {
            return (from s in doc.Element("ServiceConfiguration")
                        .Element("Role")
                        .Element("ConfigurationSettings")
                        .Elements("Setting")
                    select new KeyValuePair<string, string>(
                        s.Attribute("name").Value,
                        s.Attribute("value").Value))
                    .ToDictionary(p => p.Key, p => p.Value);
        }
    }
}
