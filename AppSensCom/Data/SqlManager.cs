using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppSensCom
{
	/// <summary>Clase que engloba el manejo de base de datos.</summary>
	public static class SqlManager
	{
		/// <summary>TimeOut utilizado en las llamadas.</summary>
		public static int TimeOut = 1000000;

		/// <summary>Se crea la conexión con MySql en forma permanente.</summary>
		/// <remarks>Debe generarse la opción de cerrar y reabrir la conexión en caso de error.</remarks>
		public static SqlConnection GetConnetion()
		{
			return new SqlConnection(ConfigurationManager.ConnectionStrings[@"AppSensConnectionString"].ConnectionString);
		}

		/// <summary>Se crea la conexión con MySql en forma permanente.</summary>
		/// <param name="strConnexion">Cadena de conexión.</param>
		/// <remarks>Debe generarse la opción de cerrar y reabrir la conexión en caso de error.</remarks>
		public static SqlConnection GetConnetion(string strConnexion)
		{
			return new SqlConnection(strConnexion);
		}

		/// <summary>Ejecutar querys sin esperar respuesta.</summary>
		/// <param name="strQuery">Query a ejecutar.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		public static int ExecuteNonQuery(string strQuery)
		{
			return ExecuteNonQuery(strQuery);
		}

		/// <summary>Ejecutar querys sin esperar respuesta.</summary>
		/// <param name="strQuery">Query a ejecutar.</param>
		/// <param name="strConnexion">Cadena de conexión.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		public static int ExecuteNonQuery(string strQuery, string strConnexion)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(strConnexion))
				{
					using (SqlCommand command = new SqlCommand(strQuery, conn))
					{
						command.CommandTimeout = TimeOut;
						conn.Open();
						int resp;
						using (SqlTransaction ts = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
						{
							resp = command.ExecuteNonQuery();
							ts.Commit();
						}
						conn.Close();
						return resp;
					}
				}
			}
			catch (SqlException mex)
			{
				if (mex.Number == 1040)
				{
					KillService();
				}
				return -1;
			}
			catch (Exception)
			{
				//eventlog.WriteEntry(@"Error ejecutando [" + strQuery + @"]:" + Environment.NewLine + ex, EventLogEntryType.Error);
				return 0;
			}
		}

		/// <summary>Ejecutar querys sin esperar respuesta.</summary>
		/// <param name="strQuery">Query a ejecutar.</param>
		/// <param name="connexion">Objeto de conexión.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		public static int ExecuteNonQuery(string strQuery, SqlConnection connexion)
		{
			try
			{
				using (SqlCommand command = new SqlCommand(strQuery, connexion))
				{
					command.CommandTimeout = TimeOut;
					connexion.Open();
					return command.ExecuteNonQuery();
				}
			}
			catch (SqlException mex)
			{
				if (mex.Number == 1040)
				{
					KillService();
				}
				return -1;
			}
			catch (Exception)
			{
				//eventlog.WriteEntry(@"Error ejecutando [" + strQuery + @"]:" + Environment.NewLine + ex, EventLogEntryType.Error);
				throw;
			}
		}

		public static bool ValidateConnection()
		{
			SqlConnection connexion = GetConnetion();
			try
			{
				using (connexion)
				{
					connexion.Open();
				}
				return true;
			}
			catch (SqlException)
			{
				return false;
			}
		}

		/// <summary>Ejecutar querys y obtener un dataset.</summary>
		/// <param name="strQuery">Query a ejecutar.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		/// <returns>DataSet con los datos retornados.</returns>
		public static DataSet GetDataSet(string strQuery)
		{
			return GetDataSet(strQuery);
		}

		/// <summary>Ejecutar querys y obtener un dataset.</summary>
		/// <param name="strQuery">Query a ejecutar.</param>
		/// <param name="strConnexion">Cadena de conexión.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		/// <returns>DataSet con los datos retornados.</returns>
		public static DataSet GetDataSet(string strQuery, string strConnexion)
		{
			DataSet ds = new DataSet();
			try
			{
				using (SqlConnection conn = new SqlConnection(strConnexion))
				{
					using (SqlCommand command = conn.CreateCommand())
					{
						command.CommandTimeout = TimeOut;
						command.CommandText = strQuery;
						using (SqlDataAdapter da = new SqlDataAdapter())
						{
							da.SelectCommand = command;
							conn.Open();
							using (SqlTransaction ts = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
							{
								da.Fill(ds);
								ts.Commit();
							}
						}
					}
					conn.Close();
				}
			}
			catch (SqlException mex)
			{
				if (mex.Number == 1040)
				{
					KillService();
					return null;
				}
			}
			catch (Exception)
			{
				//eventlog.WriteEntry(@"Error ejecutando [" + strQuery + @"]:" + Environment.NewLine + ex, EventLogEntryType.Error);
				throw;
			}
			return ds;
		}

		/// <summary>Ejecutar querys y obtener un dataset.</summary>
		/// <param name="strQuery">Query a ejecutar.</param>
		/// <param name="connexion">Objeto de conexión.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		/// <returns>DataSet con los datos retornados.</returns>
		public static DataSet GetDataSet(string strQuery, SqlConnection connexion)
		{
			DataSet ds = new DataSet();
			try
			{
				using (SqlCommand command = connexion.CreateCommand())
				{
					command.CommandTimeout = TimeOut;
					command.CommandText = strQuery;
					using (SqlDataAdapter da = new SqlDataAdapter())
					{
						da.SelectCommand = command;
						da.Fill(ds);
					}
				}
			}
			catch (SqlException mex)
			{
				if (mex.Number == 1040)
				{
					KillService();
					return null;
				}
			}
			catch (Exception)
			{
				//eventlog.WriteEntry(@"Error ejecutando [" + strQuery + @"]:" + Environment.NewLine + ex, EventLogEntryType.Error);
				throw;
			}
			return ds;
		}

		/// <summary>Obtener un valor simple de un query.</summary>
		/// <param name="strQuery">Texto del query a usar.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		/// <returns>Objeto con el valor.</returns>
		public static object GetScalar(string strQuery)
		{
			return GetScalar(strQuery);
		}

		/// <summary>Obtener un valor simple de un query.</summary>
		/// <param name="strQuery">Texto del query a usar.</param>
		/// <param name="strConnexion">Cadena de conexión.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		/// <returns>Objeto con el valor.</returns>
		public static object GetScalar(string strQuery, string strConnexion)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(strConnexion))
				{
					using (SqlCommand command = new SqlCommand(strQuery, conn))
					{
						conn.Open();
						command.CommandTimeout = TimeOut;
						object obj = command.ExecuteScalar();
						conn.Close();
						return obj;
					}
				}
			}
			catch (SqlException mex)
			{
				if (mex.Number == 1040)
				{
					KillService();
				}
				return null;
			}
			catch (Exception)
			{
				//eventlog.WriteEntry(@"Error ejecutando [" + strQuery + @"]:" + Environment.NewLine + ex, EventLogEntryType.Error);
				throw;
			}
		}

		/// <summary>Obtener un valor simple de un query.</summary>
		/// <param name="strQuery">Texto del query a usar.</param>
		/// <param name="conn">Objeto de conexión a base de datos.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		/// <returns>Objeto con el valor.</returns>
		public static object GetScalar(string strQuery, SqlConnection connexion)
		{
			if (connexion == null)
				return GetScalar(strQuery);
			try
			{
				using (SqlCommand command = new SqlCommand(strQuery, connexion))
				{
					command.CommandTimeout = TimeOut;
					if (connexion.State == ConnectionState.Closed)
						connexion.Open();
					object obj;
					using (SqlTransaction ts = connexion.BeginTransaction(IsolationLevel.ReadUncommitted))
					{
						obj = command.ExecuteScalar();
						ts.Commit();
					}
					return obj;
				}
			}
			catch (SqlException mex)
			{
				if (mex.Number == 1040)
				{
					KillService();
				}
				return null;
			}
			catch (Exception)
			{
				//eventlog.WriteEntry(@"Error ejecutando [" + strQuery + @"]:" + Environment.NewLine + ex, EventLogEntryType.Error);
				throw;
			}
		}

		/// <summary>Método utilitario que retorna un DataReader para lectura secuencias.</summary>
		/// <param name="strQuery">Query a ejecutar.</param>
		/// <param name="conn">Connexion para el Reader.</param>
		/// <param name="eventlog">Objeto para almacenar log.</param>
		/// <param name="behavior">Command behavior.</param>
		/// <returns>DataReader para barrer la data.</returns>
		public static SqlDataReader GetDataReader(string strQuery, SqlConnection connexion, CommandBehavior behavior = CommandBehavior.Default)
		{
			try
			{
				using (SqlCommand command = new SqlCommand(strQuery, connexion))
				{
					command.CommandTimeout = TimeOut;
					return command.ExecuteReader(behavior);
				}
			}
			catch (SqlException mex)
			{
				if (mex.Number == 1040)
				{
					KillService();
				}
				return null;
			}
			catch (Exception)
			{
				//eventlog.WriteEntry(@"Error ejecutando [" + strQuery + @"]:" + Environment.NewLine + ex, EventLogEntryType.Error);
				throw;
			}
		}

		/// <summary>Limpia el pool de conexiones para evitar el error de MySQL.NET.</summary>
		/// <param name="connexion">Conexión que generó el error.</param>
		public static void ClearPool(SqlConnection connexion)
		{
			if (connexion != null)
				SqlConnection.ClearPool(connexion);
			else
				SqlConnection.ClearAllPools();
			Thread.Sleep(Properties.Settings.Default.ConexionRetryTime);
		}

		/// <summary>Limpia el pool de conexiones para evitar el error de MySQL.NET.</summary>
		/// <param name="strConnexion">Connexión string que generó el error.</param>
		public static void ClearPool(string strConnexion)
		{
			if (!string.IsNullOrWhiteSpace(strConnexion))
			{
				using (SqlConnection conn = new SqlConnection(strConnexion))
					SqlConnection.ClearPool(conn);
			}
			else
				SqlConnection.ClearAllPools();
			Thread.Sleep(Properties.Settings.Default.ConexionRetryTime);
		}

		/// <summary>Método para reiniciar un proceso.</summary>
		private static void KillService()
		{
			Process[] process = Process.GetProcessesByName(@"DcimWebApp");
			if (process.Length > 0)
				process[0].Kill();
		}
	}
}
