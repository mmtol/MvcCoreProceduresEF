using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;

namespace MvcCoreProceduresEF.Controllers
{
    public class EnfermosController : Controller
    {
        private RepositoryEnfermos repo;

        public EnfermosController(RepositoryEnfermos repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Enfermo> enfermos = await repo.GetEnfermosAsync();
            return View(enfermos);
        }

        public async Task<IActionResult> Details(string inscripcion)
        {
            Enfermo enfermo = await repo.FindEnfermoAsync(inscripcion);
            return View(enfermo);
        }

        public async Task<IActionResult> Delete(string inscripcion)
        {
            await repo.DeleteEnfermoRawAsync(inscripcion);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Insert()
        {
            return View();
        }

        public async Task<IActionResult> SubirSalario()
        {
            List<string> especialidades = await repo.GetEspecialidadesAsync();
            ViewData["Especialidades"] = especialidades;
            return View();
        }

        public async Task<IActionResult> SubirSalarioEF()
        {
            List<string> especialidades = await repo.GetEspecialidadesAsync();
            ViewData["Especialidades"] = especialidades;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Insert(string apellido, string direccion, string sexo, string nss)
        {
            await repo.InsertEnfermoAsync(apellido, direccion, DateTime.Now, sexo, nss);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SubirSalario(string especialidad, int cantidad)
        {
            List<string> especialidades = await repo.GetEspecialidadesAsync();
            ViewData["Especialidades"] = especialidades;

            await repo.UpdateSalarioEspecialidadAsync(especialidad, cantidad);
            List<Doctor> doctores = await repo.GetDoctoresEspecialidadAsync(especialidad);
            return View(doctores);
        }

        [HttpPost]
        public async Task<IActionResult> SubirSalarioEF(string especialidad, int cantidad)
        {
            List<string> especialidades = await repo.GetEspecialidadesAsync();
            ViewData["Especialidades"] = especialidades;

            await repo.UpdateSalarioEspecialidadEF(especialidad, cantidad);
            List<Doctor> doctores = await repo.GetDoctoresEspecialidadAsync(especialidad);
            return View(doctores);
        }
    }
}
