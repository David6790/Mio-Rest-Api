using Microsoft.EntityFrameworkCore;
using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using System.Runtime.InteropServices;

namespace Mio_Rest_Api.Services
{
    public interface IServiceOccupation
    {
        Task<List<OccupationStatus>> GetAllOccupationStatus();
        Task<(List<OccupationStatus>, bool)> AddOccupationStatus(OccupationDTO occupationDTO);
    }

    public class ServiceOccupation : IServiceOccupation
    {
        private readonly ContextApplication _contexte;

        public ServiceOccupation(ContextApplication contexte)
        {
            _contexte = contexte;
        }

        public async Task<List<OccupationStatus>> GetAllOccupationStatus()
        {
            return await _contexte.OccupationStatus.ToListAsync();
        }



        public async Task<(List<OccupationStatus>, bool)> AddOccupationStatus(OccupationDTO occupationDTO)
        {
            var dateOfEffect = DateOnly.ParseExact(occupationDTO.DateOfEffect, "yyyy-MM-dd");
            var existingStatus = await _contexte.OccupationStatus
                                             .FirstOrDefaultAsync(os => os.DateOfEffect == dateOfEffect);

            if (existingStatus != null)
            {
                // Renvoie l'occupation existante avec un indicateur de conflit
                return (new List<OccupationStatus> { existingStatus }, true);
            }

            // Création d'un nouveau statut d'occupation si aucun conflit n'a été trouvé
            var newOccupationStatus = new OccupationStatus
            {
                DateOfEffect = dateOfEffect,
                OccStatus = occupationDTO.OccStatus
            };

            _contexte.OccupationStatus.Add(newOccupationStatus);
            await _contexte.SaveChangesAsync();

            // Renvoie la nouvelle occupation sans indicateur de conflit
            return (new List<OccupationStatus> { newOccupationStatus }, false);
        }
    }

    
}
