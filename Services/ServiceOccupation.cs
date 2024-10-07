using Mio_Rest_Api.Data;
using Mio_Rest_Api.DTO;
using Mio_Rest_Api.Entities;
using Mio_Rest_Api.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mio_Rest_Api.Services
{
    public interface IServiceOccupation
    {
        Task<List<OccupationStatus>> GetAllOccupationStatus();
        Task<(List<OccupationStatus>, bool)> AddOccupationStatus(OccupationDTO occupationDTO);
        Task<OccupationStatusDetailDTO> GetOccupationStatusByDate(DateOnly date);
        Task<OccupationStatus> DeleteOccupationStatus(int id);
        Task<OccupationStatus> UpdateOccupationStatus(int id, string newOccStatusMidi, string newOccStatusDiner);
    }

    public class ServiceOccupation : IServiceOccupation
    {
        private readonly ContextApplication _context;

        public ServiceOccupation(ContextApplication context)
        {
            _context = context;
        }

        public async Task<List<OccupationStatus>> GetAllOccupationStatus()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            // Filter the OccupationStatuses to only include those with a DateOfEffect equal to or greater than today
            return await _context.OccupationStatus
                                 .Where(os => os.DateOfEffect >= today)
                                 .ToListAsync();
        }

        public async Task<(List<OccupationStatus>, bool)> AddOccupationStatus(OccupationDTO occupationDTO)
        {
            try
            {
                // Assigner "RAS" par défaut si le statut du midi est null ou vide (même si c'est une chaîne d'espaces)
                occupationDTO.OccStatusMidi = string.IsNullOrWhiteSpace(occupationDTO.OccStatusMidi) ? "RAS" : occupationDTO.OccStatusMidi;
                occupationDTO.OccStatusDiner = string.IsNullOrWhiteSpace(occupationDTO.OccStatusDiner) ? "RAS" : occupationDTO.OccStatusDiner;

                // Validation des statuts pour midi et dîner
                if (!IsValidOccupationStatus(occupationDTO.OccStatusMidi) || !IsValidOccupationStatus(occupationDTO.OccStatusDiner))
                {
                    throw new ArgumentException("Invalid occupation status value.");
                }

                var dateOfEffect = DateOnly.ParseExact(occupationDTO.DateOfEffect, "yyyy-MM-dd");
                var existingStatus = await _context.OccupationStatus
                                                   .FirstOrDefaultAsync(os => os.DateOfEffect == dateOfEffect);

                if (existingStatus != null)
                {
                    return (new List<OccupationStatus> { existingStatus }, true); // Conflit détecté
                }

                var newOccupationStatus = new OccupationStatus
                {
                    DateOfEffect = dateOfEffect,
                    OccStatusMidi = occupationDTO.OccStatusMidi,
                    OccStatusDiner = occupationDTO.OccStatusDiner
                };

                _context.OccupationStatus.Add(newOccupationStatus);
                await _context.SaveChangesAsync();

                return (new List<OccupationStatus> { newOccupationStatus }, false);
            }
            catch (Exception ex)
            {
                // Log the error if needed
                throw new Exception($"Erreur interne : {ex.Message}");
            }
        }




        public async Task<OccupationStatusDetailDTO> GetOccupationStatusByDate(DateOnly date)
        {
            var occupationStatus = await _context.OccupationStatus
                                                 .FirstOrDefaultAsync(os => os.DateOfEffect == date);

            // Filtrage des créneaux pour midi et dîner
            var timeSlotsMidi = FilterTimeSlots(occupationStatus?.OccStatusMidi ?? "RAS", true); // True pour midi
            var timeSlotsDiner = FilterTimeSlots(occupationStatus?.OccStatusDiner ?? "RAS", false); // False pour dîner

            // Fusion des créneaux horaires
            var timeSlots = timeSlotsMidi.Concat(timeSlotsDiner).ToList();

            return new OccupationStatusDetailDTO
            {
                DateOfEffect = date,
                OccStatusMidi = occupationStatus?.OccStatusMidi ?? "RAS",
                OccStatusDiner = occupationStatus?.OccStatusDiner ?? "RAS",
                TimeSlots = timeSlots
            };
        }


        public async Task<OccupationStatus> DeleteOccupationStatus(int id)
        {
            var occupationStatus = await _context.OccupationStatus.FindAsync(id);
            if (occupationStatus == null)
            {
                return null; // OccupationStatus not found
            }

            _context.OccupationStatus.Remove(occupationStatus);
            await _context.SaveChangesAsync();
            return occupationStatus; // Return the deleted occupation status
        }

        public async Task<OccupationStatus> UpdateOccupationStatus(int id, string newOccStatusMidi, string newOccStatusDiner)
        {
            newOccStatusMidi = string.IsNullOrWhiteSpace(newOccStatusMidi) ? "RAS" : newOccStatusMidi;
            newOccStatusDiner = string.IsNullOrWhiteSpace(newOccStatusDiner) ? "RAS" : newOccStatusDiner;
            // Validation des statuts
            if (!IsValidOccupationStatus(newOccStatusMidi) || !IsValidOccupationStatus(newOccStatusDiner))
            {
                throw new ArgumentException("Invalid occupation status value.");
            }

            var occupationStatus = await _context.OccupationStatus.FindAsync(id);
            if (occupationStatus == null)
            {
                return null; // OccupationStatus introuvable
            }

            // Mise à jour des statuts pour midi et dîner
            occupationStatus.OccStatusMidi = newOccStatusMidi;
            occupationStatus.OccStatusDiner = newOccStatusDiner;
            await _context.SaveChangesAsync();

            return occupationStatus;
        }



        private List<string> FilterTimeSlots(string occStatus, bool isMidi)
        {
            // Filtrage pour les créneaux de midi
            if (isMidi)
            {
                switch (occStatus)
                {
                    case "RAS":
                        // Retourne les créneaux standard disponibles pour le midi (12:00 à 13:45)
                        return TimeSlotConfig.TimeSlots.Where(ts => ts.CompareTo("12:00") >= 0 && ts.CompareTo("13:45") <= 0).ToList();

                    case "MidiComplet":
                        // Aucun créneau disponible pour le midi
                        return new List<string>();

                    case "MidiEtendu":
                        // Créneaux supplémentaires pour "MidiEtendu" sans modifier TimeSlotConfig
                        var timeSlotsMidiEtendu = new List<string>
                        {
                            "11:15", "11:30", "11:45", "14:00", "14:15", "14:30", "14:45"
                        };

                        // Combiner les créneaux standards (12:00 - 13:45) avec les créneaux supplémentaires
                        var allTimeSlotsMidiEtendu = TimeSlotConfig.TimeSlots
                            .Where(ts => ts.CompareTo("12:00") >= 0 && ts.CompareTo("13:45") <= 0)
                            .Concat(timeSlotsMidiEtendu)
                            .OrderBy(ts => ts)  // Assurer un tri par ordre croissant
                            .ToList();

                        return allTimeSlotsMidiEtendu;

                    case "MidiDoubleService":
                        // Créneaux supplémentaires pour "MidiDoubleService" sans les créneaux entre 12:15 et 13:15
                        var timeSlotsMidiDoubleService = new List<string>
                        {
                            "11:15", "11:30", "11:45", "14:00", "14:15", "14:30", "14:45"
                        };

                        // Filtrer les créneaux standards (12:00 à 13:45) mais exclure ceux entre 12:15 et 13:15
                        var allTimeSlotsMidiDoubleService = TimeSlotConfig.TimeSlots
                            .Where(ts => (ts.CompareTo("12:00") >= 0 && ts.CompareTo("12:15") < 0) || (ts.CompareTo("13:15") > 0 && ts.CompareTo("13:45") <= 0))
                            .Concat(timeSlotsMidiDoubleService)
                            .OrderBy(ts => ts)  // Assurer un tri par ordre croissant
                            .ToList();

                        return allTimeSlotsMidiDoubleService;

                    default:
                        // Aucun créneau disponible pour un statut inconnu
                        return new List<string>();
                }

            }
            // Filtrage pour les créneaux du soir
            else
            {
                switch (occStatus)
                {
                    case "RAS":
                        // Retourne tous les créneaux disponibles pour le dîner (19:00 à 21:45)
                        return TimeSlotConfig.TimeSlots.Where(ts => ts.CompareTo("19:00") >= 0 && ts.CompareTo("21:45") <= 0).ToList();

                    case "Complet":
                        // Aucun créneau disponible pour le dîner
                        return new List<string>();

                    case "FreeTable21":
                        // Retourne les créneaux du soir (19:00 et après 21:00)
                        return TimeSlotConfig.TimeSlots.Where(ts =>
                            ts == "19:00" || ts.CompareTo("21:00") > 0
                        ).ToList();

                    case "Service1Complet":
                        // Retourne les créneaux de 21:00 à 21:45
                        return TimeSlotConfig.TimeSlots.Where(ts =>
                            ts.CompareTo("21:00") >= 0 && ts.CompareTo("21:45") <= 0
                        ).ToList();

                    case "Service2Complet":
                        // Retourne uniquement le créneau de 19:00
                        return TimeSlotConfig.TimeSlots.Where(ts => ts == "19:00").ToList();

                    default:
                        // Aucun créneau disponible pour un statut inconnu
                        return new List<string>();
                }
            }
        }



        private bool IsValidOccupationStatus(string OccStatusDiner)
        {
            var validStatuses = new HashSet<string>
            {
                "RAS",
                "FreeTable21",
                "Service1Complet",
                "Service2Complet",
                "Complet",
                "MidiComplet",
                "MidiEtendu", 
                "MidiDoubleService"
            };

            return validStatuses.Contains(OccStatusDiner);
        }

    }
}
