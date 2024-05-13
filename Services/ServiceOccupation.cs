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
        Task<OccupationStatus?> GetOccupationStatusByDate(DateOnly date);
        Task<List<TimeSlot>> GetDefaultTimeSlots();
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
            // Utilisez Include pour charger les TimeSlots associés à chaque OccupationStatus
            return await _contexte.OccupationStatus
                                  .Include(os => os.TimeSlots) // Charge les TimeSlots associés
                                  .ToListAsync();
        }




        public async Task<(List<OccupationStatus>, bool)> AddOccupationStatus(OccupationDTO occupationDTO)
        {
            var dateOfEffect = DateOnly.ParseExact(occupationDTO.DateOfEffect, "yyyy-MM-dd");
            var existingStatus = await _contexte.OccupationStatus
                                                .Include(os => os.TimeSlots) // Assurez-vous d'inclure les TimeSlots
                                                .FirstOrDefaultAsync(os => os.DateOfEffect == dateOfEffect);

            if (existingStatus != null)
            {
                return (new List<OccupationStatus> { existingStatus }, true); // Conflit détecté
            }

            var newOccupationStatus = new OccupationStatus
            {
                DateOfEffect = dateOfEffect,
                OccStatus = occupationDTO.OccStatus,
                TimeSlots = new List<TimeSlot>() // Préparez une liste vide de TimeSlots
            };

            switch (occupationDTO.OccStatus)
            {
                case "RAS":
                    newOccupationStatus.TimeSlots.AddRange(
                        _contexte.TimeSlots.Where(ts => ts.Id >= 1 && ts.Id <= 20).ToList());
                    break;
                
                case "FreeTable21":
                    newOccupationStatus.TimeSlots.AddRange(
                        _contexte.TimeSlots.Where(ts => (ts.Id >= 1 && ts.Id <= 9) || (ts.Id >= 17 && ts.Id <= 20)).ToList());
                    break;
                    

                case "service1Complet":
                    newOccupationStatus.TimeSlots.AddRange(
                        _contexte.TimeSlots.Where(ts => (ts.Id >= 1 && ts.Id <= 8) || (ts.Id >= 17 && ts.Id <= 20)).ToList());
                    break;
                case "service2Complet":
                    newOccupationStatus.TimeSlots.AddRange(
                         _contexte.TimeSlots.Where(ts => (ts.Id >= 1 && ts.Id <= 9) || (ts.Id >= 17 && ts.Id <= 20)).ToList());
                    break;
                    
                default:
                    // Peut-être traiter un cas par défaut ou rien faire si le statut n'est pas reconnu
                    break;
            }

            _contexte.OccupationStatus.Add(newOccupationStatus);
            await _contexte.SaveChangesAsync();

            return (new List<OccupationStatus> { newOccupationStatus }, false); // Pas de conflit, retourner la nouvelle occupation
        }

        public async Task<OccupationStatus?> GetOccupationStatusByDate(DateOnly date)
        {
            return await _contexte.OccupationStatus
                                  .Include(os => os.TimeSlots)
                                  .FirstOrDefaultAsync(os => os.DateOfEffect == date);
        }


        public async Task<List<TimeSlot>> GetDefaultTimeSlots()
        {
            return await _contexte.TimeSlots.Where(ts => ts.Id >= 1 && ts.Id <= 20).ToListAsync();
        }



    }


}
