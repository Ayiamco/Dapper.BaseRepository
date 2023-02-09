using Dapper.BaseRepository.Attributes;
using Dapper.BaseRepository.Config;
using System.Data;

namespace Dapper.BaseRepository.Components
{
    internal static class BaseUtility
    {
        #region private methods
        internal static string GetLogMessage(string name, Exception ex) =>
            @$"Error occured at while running query from function :{name}; Message:{ex.Message}. 
{ex.StackTrace}";

        internal static string GetConnectionString(string? conn, DbType sqlType)
        {
            if (!string.IsNullOrWhiteSpace(conn)) return conn;


            //TODO: Finish map
            var connectionStringMap = new Dictionary<DbType, string>()
            {
                {DbType.SqlServer,ConnectionStrings.SqlServerConnection },
                {DbType.Sybase,ConnectionStrings.SybaseConnection },
                {DbType.Oracle,ConnectionStrings.OracleConnection },
            };

            var dbTypeNameMap = new Dictionary<DbType, string>()
            {
                {DbType.SqlServer,"SqlServer database" },
                {DbType.Sybase,"Sybase database" },
                {DbType.Oracle,"Oracle database" },
            };
            var connectionString = connectionStringMap[sqlType];
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException($"Default Connection string for {dbTypeNameMap[sqlType]} is not setup.Please pass in connectionString or setup a default connection string.");
            return connectionString;
        }

        internal static void AddOutputParam(DynamicParameters dynamicParameters, string propName, Attribute[] customAttributes)
        {
            foreach (var attr in customAttributes)
            {
                var attributeType = attr.GetType();
                var attributeName = attributeType.Name;
                switch (attributeName)
                {
                    //Stored Procedure Output section
                    case nameof(SpOutputString):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.String, direction: ParameterDirection.Output, size: (int)attributeType.GetProperties().Where(x => x.Name == nameof(SpOutputString.Length)).First().GetValue(attr));
                        break;
                    case nameof(SpOutputStringFixed):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.String, direction: ParameterDirection.Output, size: (int)attributeType.GetProperties().Where(x => x.Name == nameof(SpOutputStringFixed.Length)).First().GetValue(attr));
                        break;
                    case nameof(SpOutputAnsiString):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.AnsiString, direction: ParameterDirection.Output, size: (int)attributeType.GetProperties().Where(x => x.Name == nameof(SpOutputAnsiString.Length)).First().GetValue(attr));
                        break;
                    case nameof(SpOutputAnsiStringFixed):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.AnsiString, direction: ParameterDirection.Output, size: (int)attributeType.GetProperties().Where(x => x.Name == nameof(SpOutputAnsiStringFixed.Length)).First().GetValue(attr));
                        break;
                    case nameof(SpOutputInt):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.Int32, direction: ParameterDirection.Output);
                        break;
                    case nameof(SpOutputBigInt):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.Int64, direction: ParameterDirection.Output);
                        break;
                    case nameof(SpOutputDateTime):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.DateTime, direction: ParameterDirection.Output);
                        break;
                    case nameof(SpOutputDate):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.Date, direction: ParameterDirection.Output);
                        break;
                    case nameof(SpOutputGuid):
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.Guid, direction: ParameterDirection.Output);
                        break;


                    //Stored Procedure return section
                    case "SpReturnStringAttribute":
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.String, direction: ParameterDirection.ReturnValue, size: (int)attributeType.GetProperties().Where<System.Reflection.PropertyInfo>(x => x.Name == nameof(SpReturnString.Length)).First().GetValue(attr));
                        break;
                    case "SpReturnIntAttribute":
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.Int32, direction: ParameterDirection.ReturnValue);
                        break;
                    case "SpReturnBigIntAttribute":
                        dynamicParameters.Add(propName, dbType: System.Data.DbType.Int64, direction: ParameterDirection.ReturnValue);
                        break;
                    default:
                        break;
                }
            }
        }

        internal static TResult GetStoredProcedureResult<TResult>(DynamicParameters storedProcedureRespone)
        {
            var resultInstance = Activator.CreateInstance<TResult>();
            var properties = resultInstance.GetType().GetProperties();
            var DynamicParametersGetFuncGenericRef = typeof(DynamicParameters).GetMethod("Get");
            foreach (var prop in properties)
            {
                var propName = prop.Name;
                var type = prop.PropertyType;

                var DynamicParametersGetFuncRef = DynamicParametersGetFuncGenericRef.MakeGenericMethod(type);
                var value = DynamicParametersGetFuncRef.Invoke(storedProcedureRespone, new string[] { propName });
                prop.SetValue(resultInstance, value);
            }
            return resultInstance;
        }


        /// <summary>
        /// Creates <see cref="DynamicParameters"/> object or adds additional parameters from the storedProcParams.
        /// </summary>
        /// <param name="storedProcParams">Object containing stored proc input, outpust and return paramaters</param>
        /// <param name="dynamicParameters">Dynamic parameter that the storedProcParams would be added to.</param>
        /// <returns><see cref=" DynamicParameters "/> containing the stored procedure input, output and return parameters.</returns>
        internal static DynamicParameters CreateDynamicParameter(object storedProcParams, DynamicParameters? dynamicParameters = default)
        {
            dynamicParameters = dynamicParameters == null ? new DynamicParameters() : dynamicParameters;
            if (storedProcParams == null)
                return dynamicParameters;

            var properties = storedProcParams.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var key = prop.Name;
                Attribute[] attrs = Attribute.GetCustomAttributes(prop);
                if (attrs.Length == 0)
                {
                    var value = prop.GetValue(storedProcParams);
                    dynamicParameters.Add(key, value);
                    continue;
                }
                BaseUtility.AddOutputParam(dynamicParameters, key, attrs);
            }
            return dynamicParameters;
        }
        #endregion
    }
}
