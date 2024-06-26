﻿using Mio_Rest_Api.Data;
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
        Task<OccupationStatus> UpdateOccupationStatus(int id, string newOccStatus);
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
            if (!IsValidOccupationStatus(occupationDTO.OccStatus))
            {
                throw new ArgumentException("Invalid occupation status value.");
            }

            var dateOfEffect = DateOnly.ParseExact(occupationDTO.DateOfEffect, "yyyy-MM-dd");
            var existingStatus = await _context.OccupationStatus
                                               .FirstOrDefaultAsync(os => os.DateOfEffect == dateOfEffect);

            if (existingStatus != null)
            {
                return (new List<OccupationStatus> { existingStatus }, true); // Conflict detected
            }

            var newOccupationStatus = new OccupationStatus
            {
                DateOfEffect = dateOfEffect,
                OccStatus = occupationDTO.OccStatus
            };

            _context.OccupationStatus.Add(newOccupationStatus);
            await _context.SaveChangesAsync();

            return (new List<OccupationStatus> { newOccupationStatus }, false);
        }


        public async Task<OccupationStatusDetailDTO> GetOccupationStatusByDate(DateOnly date)
        {
            var occupationStatus = await _context.OccupationStatus
                                                 .FirstOrDefaultAsync(os => os.DateOfEffect == date);

            var timeSlots = FilterTimeSlots(occupationStatus?.OccStatus ?? "RAS");

            return new OccupationStatusDetailDTO
            {
                DateOfEffect = date,
                OccStatus = occupationStatus?.OccStatus ?? "RAS",
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

        public async Task<OccupationStatus> UpdateOccupationStatus(int id, string newOccStatus)
        {
            if (!IsValidOccupationStatus(newOccStatus))
            {
                throw new ArgumentException("Invalid occupation status value.");
            }

            var occupationStatus = await _context.OccupationStatus.FindAsync(id);
            if (occupationStatus == null)
            {
                return null; // OccupationStatus not found
            }

            occupationStatus.OccStatus = newOccStatus;
            await _context.SaveChangesAsync();

            return occupationStatus;
        }


        private List<string> FilterTimeSlots(string occStatus)
        {
            switch (occStatus)
            {
                case "RAS":
                    // Retourne tous les TimeSlots disponibles
                    return TimeSlotConfig.TimeSlots;

                case "FreeTable21":
                    // Retourne tous les créneaux de midi (12:00 à 13:45) et les créneaux sélectionnés du soir (19:00 et après 21:00)
                    return TimeSlotConfig.TimeSlots.Where(ts =>
                        (ts.StartsWith("12:") || ts.StartsWith("13:") || ts == "19:00" || ts.CompareTo("21:00") > 0)
                    ).ToList();

                case "Service1Complet":
                    // Retourne les créneaux de 12:00 à 13:45 et de 21:00 à 21:45
                    return TimeSlotConfig.TimeSlots.Where(ts =>
                        (ts.StartsWith("12:") || ts.StartsWith("13:") || (ts.CompareTo("21:00") >= 0 && ts.CompareTo("21:45") <= 0))
                    ).ToList();

                case "Service2Complet":
                    // Retourne les créneaux de 12:00 à 13:45 et 19:00 uniquement
                    return TimeSlotConfig.TimeSlots.Where(ts =>
                        (ts.StartsWith("12:") || ts.StartsWith("13:") || ts == "19:00")
                    ).ToList();

                case "Complet":
                    // Retourne uniquement les créneaux de 12:00 à 13:45
                    return TimeSlotConfig.TimeSlots.Where(ts =>
                        (ts.StartsWith("12:") || ts.StartsWith("13:"))
                    ).ToList();

                case "MidiComplet":
                    // Supprime tout les horaires allant de 12:00 à 13:45
                    return TimeSlotConfig.TimeSlots.Where(ts =>
                        !ts.StartsWith("12:") && !ts.StartsWith("13:")
                    ).ToList();

                default:
                    // Aucun créneau disponible
                    return new List<string>();
            }
        }

        private bool IsValidOccupationStatus(string occStatus)
        {
            var validStatuses = new HashSet<string>
            {
                "RAS",
                "FreeTable21",
                "Service1Complet",
                "Service2Complet",
                "Complet",
                "MidiComplet"
            };

            return validStatuses.Contains(occStatus);
        }

    }
}
