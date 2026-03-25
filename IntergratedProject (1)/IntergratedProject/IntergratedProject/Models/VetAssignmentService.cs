using System;
using System.Collections.Generic;
using System.Linq;
using IntergratedProject.Models;
using IntergratedProject.Data;
using Microsoft.EntityFrameworkCore;

public class VetAssignmentService
{
    private readonly IntergratedProjectDatabase _context;

    public VetAssignmentService(IntergratedProjectDatabase context)
    {
        _context = context;
    }

    public int? AssignVet(int appointmentTypeId, DateTime date, TimeSpan time, int durationMinutes, int? preferredVetId = null, int? petId = null)
    {
        DateTime startTime = date.Date + time;
        DateTime endTime = startTime.AddMinutes(durationMinutes);

        // 1. Preferred vet check
        if (preferredVetId.HasValue)
        {
            var preferredVet = _context.Veterinarian.Find(preferredVetId.Value);
            if (preferredVet != null && IsVetAvailable(preferredVetId.Value, startTime, endTime))
                return preferredVetId;
        }

        // 2. Get vets matching specialization
        var appointmentType = _context.AppointmentTypes
            .Include(at => at.Specialization)
            .FirstOrDefault(at => at.AppointmentTypeId == appointmentTypeId);

        if (appointmentType == null)
            return null;

        var qualifiedVets = _context.Veterinarian
            .Include(v => v.Specializations)
                .ThenInclude(vs => vs.Specialization)
            .ToList()
            .Where(v => v.Specializations.Any(vs => vs.SpecializationId == appointmentType.SpecializationId))
            .ToList();

        // 3. Filter available vets
        qualifiedVets = qualifiedVets
            .Where(v => IsVetAvailable(v.VeterinarianId, startTime, endTime))
            .ToList();

        if (!qualifiedVets.Any())
            return null; // No one fits → Pending Vet Assignment

        // 4. Pick vet with least appointments that day
        var vetLoads = qualifiedVets
            .Select(v => new
            {
                VetId = v.VeterinarianId,
                AppointmentCount = _context.Appointment
                    .Count(a => a.VeterinarianId == v.VeterinarianId &&
                                (a.Date.Date + a.Time) >= startTime &&
                                (a.Date.Date + a.Time) < endTime)
            })
            .OrderBy(x => x.AppointmentCount)
            .ToList();

        return vetLoads.First().VetId;
    }

    private bool IsVetAvailable(int vetId, DateTime startTime, DateTime endTime)
    {
        return !_context.Appointment.Any(a =>
            a.VeterinarianId == vetId &&
            ((startTime >= a.Date.Date + a.Time && startTime < (a.Date.Date + a.Time).AddMinutes(a.AppointmentType.DurationMinutes)) ||
             (endTime > a.Date.Date + a.Time && endTime <= (a.Date.Date + a.Time).AddMinutes(a.AppointmentType.DurationMinutes))));
    }
}
