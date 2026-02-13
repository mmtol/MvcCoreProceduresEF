using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;

namespace MvcCoreProceduresEF.Repositories
{
    #region VIEWS
    //    create view V_EMPLEADOS_DEPARTAMENTOS
    //as
    //	select isnull(
    //    ROW_NUMBER() over (order by EMP.APELLIDO), 0) as ID,
    //	EMP.APELLIDO, EMP.OFICIO, EMP.SALARIO, DEPT.DNOMBRE AS DEPARTAMENTO, DEPT.LOC as LOCALIIDAD
    //    from EMP inner join DEPT on EMP.DEPT_NO = DEPT.DEPT_NO
    //go
    //    create view V_TRABAJADORES
    //as
    //	select EMP_NO as IDTRABAJADOR,
    //	APELLIDO, OFICIO, SALARIO from EMP
    //    union

    //    select DOCTOR_NO, APELLIDO, ESPECIALIDAD, SALARIO from DOCTOR

    //    union
    //    select EMPLEADO_NO, APELLIDO, FUNCION, SALARIO from PLANTILLA
    //go
    #endregion

    #region STORED PROCEDURE
    //    create procedure SP_TRABAJADORES_OFICIO
    //(@oficio nvarchar(50), @personas int out, @media int out, @suma int out)
    //as
    //	select* from V_TRABAJADORES where OFICIO = @oficio
    //    select @personas = count(IDTRABAJADOR),
    //			@media = avg(SALARIO),
    //			@suma = sum(SALARIO)

    //    from V_TRABAJADORES where OFICIO = @oficio
    //go
    #endregion

    public class RepositoryEmpleados
    {
        private HospitalContext context;

        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<TrabajadoresModel> GetTrabajadoresModelAsync()
        {
            //primero con linq
            var consulta = from datos in context.Trabajadores
                           select datos;
            TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = await consulta.CountAsync();
            model.Suma = await consulta.SumAsync(z => z.Salario);
            model.Media = (int) consulta.Average(z => z.Salario);

            return model;
        }

        public async Task<TrabajadoresModel> GetTrabajadoresModelOficioAsync(string oficio)
        {
            //ya que tenemos model, vamos a llamarlo con ef
            //la unica diferencia cuando tenemos parametros de salida es indicar
            //la palabra out en la declaracion de las variables
            string sql = "SP_TRABAJADORES_OFICIO @oficio, @personas out, @suma out, @media out";
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamPersonas = new SqlParameter("@personas", -1);
            pamPersonas.Direction = System.Data.ParameterDirection.Output;
            SqlParameter pamMedia = new SqlParameter("@media", -1);
            pamMedia.Direction = System.Data.ParameterDirection.Output;
            SqlParameter pamSuma = new SqlParameter("@suma", -1);
            pamSuma.Direction = System.Data.ParameterDirection.Output;
            //ejecutams la consulta con el model
            var consulta = context.Trabajadores.FromSqlRaw(sql, pamOficio, pamPersonas, pamMedia, pamSuma);
            TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = int.Parse(pamPersonas.Value.ToString());
            model.Media = int.Parse(pamMedia.Value.ToString());
            model.Suma = int.Parse(pamSuma.Value.ToString());

            return model;
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in context.Trabajadores
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        public async Task<List<VistaEmpleado>> GetVistaEmpleadosAsync()
        {
            var consulta = from datos in context.VistaEmpleados
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
