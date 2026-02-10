using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System;
using System.Data;
using System.Data.Common;

namespace MvcCoreProceduresEF.Repositories
{
    #region STORED PROCEDURES
    //    create procedure SP_ALL_ENFERMOS
    //as
    //	select* from ENFERMO
    //go

    //create procedure SP_FIND_ENFERMO
    //(@inscripcion nvarchar(50))
    //as
    //	select* from ENFERMO where INSCRIPCION = @inscripcion
    //go

    //create procedure SP_DELETE_ENFERMO
    //(@inscripcion nvarchar(50))
    //as
    //	delete from ENFERMO where INSCRIPCION = @inscripcion
    //go

    #endregion

    public class RepositoryEnfermos
    {
        private EnfermosContext context;

        public RepositoryEnfermos(EnfermosContext context)
        {
            this.context = context;
        }

        public async Task<List<Enfermo>> GetEnfermosAsync()
        {
            //necesitamos un command, vamos a utilizar un using para todo
            //el command en su creacion necesita una cadena de conn
            //el obj conn nos lo ofrece ef
            //las conn se crean a partir del context

            List<Enfermo> enfermos = new List<Enfermo>();
            using (DbCommand com = context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_ALL_ENFERMOS";
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                //abrimos la conn a partir del command
                await com.Connection.OpenAsync();
                //ejecutamos nuestro reader
                DbDataReader reader = await com.ExecuteReaderAsync();
                //debemos mapear los datos manualmente
                while (await reader.ReadAsync())
                {
                    Enfermo e = new Enfermo();
                    e.Inscripcion = reader["INSCRIPCION"].ToString();
                    e.Apellido = reader["APELLIDO"].ToString();
                    e.Direccion = reader["DIRECCION"].ToString();
                    e.FechaNac = DateTime.Parse(reader["FECHA_NAC"].ToString());
                    e.Sexo = reader["S"].ToString();
                    e.NSS = reader["NSS"].ToString();

                    enfermos.Add(e);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();

                return enfermos;
            }
        }
    }
}
