using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

    //    create procedure SP_INSERT_ENFERMO
    //(@apellido nvarchar(50), @direccion nvarchar(50), @fechanac DateTime, @sexo nvarchar(50), @nss nvarchar(50))
    //as
    //	declare @inscripcion nvarchar(50);
    //    select @inscripcion = Max(INSCRIPCION) from ENFERMO;
    //    set @inscripcion = @inscripcion + 1;
    //    insert into ENFERMO values(@inscripcion, @apellido, @direccion, @fechanac, @sexo, @nss);
    //    go

    //    create procedure SP_ALL_ESPECIALIDADES
    //as
    // select distinct ESPECIALIDAD from DOCTOR
    //go

    //create procedure SP_UPDATE_SALARIO_ESPECIALIDAD
    //(@especialidad nvarchar(50), @cantidad int)
    //as
    // update DOCTOR set SALARIO = SALARIO + @cantidad where ESPECIALIDAD = @especialidad
    //go

    //create procedure SP_MOSTRAR_DOCTORES_ESPECIALIDAD
    //(@especialidad nvarchar(50))
    //as
    // select* from DOCTOR where ESPECIALIDAD = @especialidad
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

        public async Task<Enfermo> FindEnfermoAsync(string inscripcion)
        {
            //para llamar a un procedimiento quw contiene params
            //la llamada se realiza mediante el nombre del procedure
            //y cada param acontinuacion en la declaracion del sql
            string sql = "SP_FIND_ENFERMO @inscripcion";
            SqlParameter paramInsc = new SqlParameter("@inscripcion", inscripcion);
            //si los datos que devuelve el procedure estan mapeados
            //con un model, podemo utilizar el metodo FromSqlRaw para recuperar directamente el model
            //no podemos consultar y extraer a la vez con linq
            //se debe realizar siempre en 2 pasos
            var consulta = context.Enfermos.FromSqlRaw(sql, paramInsc);
            //debemos utilizar AsEnumerable() para extraer los datos
            Enfermo enfermo = consulta.AsEnumerable().FirstOrDefault();
            return enfermo;
        }

        public async Task DeleteEnfermoAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO";
            SqlParameter paramInsc = new SqlParameter("@inscripcion", inscripcion);
            using (DbCommand com = context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Clear();
                com.Parameters.Add(paramInsc);
                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
            }
        }

        public async Task DeleteEnfermoRawAsync(string inscripsion)
        {
            string sql = "SP_DELETE_ENFERMO @inscripcion";
            SqlParameter paramInsc = new SqlParameter("@inscripcion", inscripsion);
            await context.Database.ExecuteSqlRawAsync(sql, paramInsc);
        }

        public async Task InsertEnfermoAsync(string apellido, string direccion, DateTime fechanac, string sexo, string nss)
        {
            string sql = "SP_INSERT_ENFERMO @apellido, @direccion, @fechanac, @sexo, @nss";
            SqlParameter paramApellido = new SqlParameter("@apellido", apellido);
            SqlParameter paramDirec = new SqlParameter("@direccion", direccion);
            SqlParameter paramFechaNac = new SqlParameter("@fechanac", fechanac);
            SqlParameter paramSexo = new SqlParameter("@sexo", sexo);
            SqlParameter paramNss = new SqlParameter("@nss", nss);
            await context.Database.ExecuteSqlRawAsync(sql, paramApellido, paramDirec, paramFechaNac, paramSexo, paramNss);
        }

        public async Task<List<string>> GetEspecialidadesAsync()
        {
            string sql = "SP_ALL_ESPECIALIDADES";
            List<string> especialidades = new List<string>();
            using (DbCommand com = context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    string especialidad = reader["ESPECIALIDAD"].ToString();
                    especialidades.Add(especialidad);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return especialidades;
            }
        }

        public async Task UpdateSalarioEspecialidadAsync(string especialidad, int cantidad)
        {
            string sql = "SP_UPDATE_SALARIO_ESPECIALIDAD @especialidad, @cantidad";
            SqlParameter paramEspecialidad = new SqlParameter("@especialidad", especialidad);
            SqlParameter paramCantidad = new SqlParameter("@cantidad", cantidad);
            await context.Database.ExecuteSqlRawAsync(sql, paramEspecialidad, paramCantidad);
        }

        public async Task<List<Doctor>> GetDoctoresEspecialidadAsync(string especialidad)
        {
            string sql = "SP_MOSTRAR_DOCTORES_ESPECIALIDAD";
            SqlParameter paramEspecialidad = new SqlParameter("@especialidad", especialidad);
            List<Doctor> doctores = new List<Doctor>();
            using (DbCommand com = context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Clear();
                com.Parameters.Add(paramEspecialidad);
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Doctor d = new Doctor();
                    d.HospitalCod = int.Parse(reader["HOSPITAL_COD"].ToString());
                    d.DoctorNo = int.Parse(reader["DOCTOR_NO"].ToString());
                    d.Apellido = reader["APELLIDO"].ToString();
                    d.Especialidad = reader["ESPECIALIDAD"].ToString();
                    d.Salario = int.Parse(reader["SALARIO"].ToString());
                    doctores.Add(d);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return doctores;
            }
        }

        public async Task UpdateSalarioEspecialidadEF(string especialidad, int cantidad)
        {
            var consulta = from datos in this.context.Doctores
                           where datos.Especialidad == especialidad
                           select datos;
            List<Doctor> doctores = await consulta.ToListAsync();
            foreach (Doctor doctor in doctores)
            {
                doctor.Salario += cantidad;
            }
            await this.context.SaveChangesAsync();
        }
    }
}
