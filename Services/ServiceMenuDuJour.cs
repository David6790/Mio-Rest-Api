using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.Entities;

namespace Mio_Rest_Api.Services
{

    public interface IServiceMenuDuJour
    {
        Task<List<MenuEntity>> GetMenus();
    }


    public class ServiceMenuDuJour : IServiceMenuDuJour
    {

        private readonly ContextApplication _contexte;

        public ServiceMenuDuJour(ContextApplication contexte)
        {
            _contexte = contexte;
        }

        public async Task<List<MenuEntity>> GetMenus()
        {
            return await _contexte.MenuDuJour.ToListAsync();
        }
    }
}
