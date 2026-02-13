using Microsoft.AspNetCore.Mvc;
using MvcCoreProceduresEF.Models;
using MvcCoreProceduresEF.Repositories;

namespace MvcCoreProceduresEF.Controllers
{
    public class TrabajadoresController : Controller
    {
        private RepositoryEmpleados repo;

        public TrabajadoresController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<string> oficios = await repo.GetOficiosAsync();
            for (int i = 0; i < oficios.Count; ++i)
            {
                oficios[i] = oficios[i].ToUpper();
            }
            ViewData["Oficios"] = oficios;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string oficio)
        {
            TrabajadoresModel model = await repo.GetTrabajadoresModelOficioAsync(oficio);
            List<string> oficios = await repo.GetOficiosAsync();
            for (int i = 0; i < oficios.Count; ++i)
            {
                oficios[i] = oficios[i].ToUpper();
            }
            ViewData["Oficios"] = oficios;
            return View(model);
        }
    }
}
