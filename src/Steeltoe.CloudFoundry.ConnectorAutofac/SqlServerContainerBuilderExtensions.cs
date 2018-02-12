﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.Configuration;
using Steeltoe.CloudFoundry.Connector;
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.CloudFoundry.Connector.SqlServer;
using System;
using System.Data;

namespace Steeltoe.CloudFoundry.ConnectorAutofac
{
    public static class SqlServerContainerBuilderExtensions
    {
        private static string[] sqlServerAssemblies = new string[] { "System.Data.SqlClient" };
        private static string[] sqlServerTypeNames = new string[] { "System.Data.SqlClient.SqlConnection" };

        public static IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> RegisterSqlServerConnection(this ContainerBuilder container, IConfiguration config)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Type sqlServerConnection = ConnectorHelpers.FindType(sqlServerAssemblies, sqlServerTypeNames);
            if (sqlServerConnection == null)
            {
                throw new ConnectorException("Unable to find System.Data.SqlClient.SqlConnection, are you missing a System.Data.SqlClient reference?");
            }

            var sqlServerConfig = new SqlServerProviderConnectorOptions(config);
            var info = config.GetSingletonServiceInfo<SqlServerServiceInfo>();
            var factory = new SqlServerProviderConnectorFactory(info, sqlServerConfig, sqlServerConnection);
            return container.Register(c => factory.Create(null)).As<IDbConnection>();
        }
    }
}
