﻿using Core.Repositories;
using Egabinet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Egabinet.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository patientRepository;
        private readonly IUserRepository userRepository;
        private readonly ITimesheetRepository timesheetRepository;


        public PatientService(IPatientRepository patientRepository, IUserRepository userRepository, ITimesheetRepository timesheetRepository)
        {
            this.patientRepository = patientRepository;
            this.userRepository = userRepository;
            this.timesheetRepository = timesheetRepository;
        }

        public async Task DeleteVisitAsync(string id)
        {
            var visitToDelete = await timesheetRepository.GetByIdAsync(id);

            if (visitToDelete == null)
            {
                throw new Exception("Visit to delete does not exist");
            }
            await timesheetRepository.RemoveAsync(id);
        }

        public async Task<UpdatePatientViewModel> GetUpdatePatientViewModel(string name)
        {
            var patient = await patientRepository.GetByNameAsync(name);

            if (patient == null)
            {
                throw new Exception("Doctor does not exist");

            }

            UpdatePatientViewModel viewModel = new UpdatePatientViewModel()
            {
                Id = patient.Id,
                Address = patient.Address,
            };

            return viewModel;
        }

        public async Task<IdentityUser> GetUser(string name)
        {
            return await userRepository.GetByNameAsync(name);

        }

        public async Task<List<TimeSheetViewModel>> ShowTimesheet(string name)
        {
            var patient = await GetUser(name);
            var viewModel = await timesheetRepository.GetAllByPatientIdAsync(patient.Id).Select(t => new TimeSheetViewModel { Patient = t.Patient.Name, Doctor = t.Doctor.Name, Room = t.Room.Number, Date = t.Data, Id = t.Id }).ToListAsync();
            return viewModel;
        }

        public async Task UpdatePatientAsync(UpdatePatientViewModel model)
        {
            var patient = await patientRepository.GetByIdAsync(model.Id);
            patient.Address = model.Address;

            await patientRepository.UpdateAsync(patient);
        }


        public async Task<PatientViewModel> GetPatientAsync(string name)
        {
            var patient = await patientRepository.GetByNameAsync(name);

            return new PatientViewModel { Address = patient.Address, Name = patient.Name, Id = patient.Id, Surname = patient.Surname };
        }
    }
}
