using Dsw2026Ej15.Api.Models;
using Dsw2026Ej15.Domain.Entities;
using Dsw2026Ej15.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2026Ej15.Api.Controllers
{
    [ApiController]
    [Route("api")]

    public class DoctorsController : ControllerBase
    {
        private readonly IPersistence _persistence;

        public DoctorsController(IPersistence persistence)
        {
            _persistence = persistence;
        }

        [HttpPost("doctors")]
        public async Task<IActionResult> CreateDoctor(DoctorModel.Request request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.LicenseNumber))
            {
                return BadRequest("Nombre y Matricula son requeridos");
            }

            var speciality = _persistence.GetSpecialityById(request.SpecialityId);
            if (speciality is null)
            {
                return BadRequest("Especialidad no encontrada");
            }

            var doctor = new Doctor(request.Name, request.LicenseNumber, speciality);
            _persistence.SaveDoctor(doctor);

            return Created();
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            
            var doctors = await _persistence.GetAllDoctorsAsync();

            return Ok(doctors);
        }

        [HttpGet("doctors/{id:guid}")]
        public async Task<IActionResult> GetDoctorById(Guid id)
        {
           
            var doctor = await _persistence.GetDoctorByIdAsync(id);
          
            if (doctor is null)
            {
                return NotFound("El doctor solicitado no existe o no está disponible.");
            }
           
            var response = new DoctorModel.ResponseTE(
                doctor.Name,
                doctor.LicenseNumber,
                doctor.Speciality.Name
            );

            return Ok(response);
        }

        [HttpDelete("doctors/{id:guid}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
          
            var doctor = await _persistence.GetDoctorByIdAsync(id);

            if (doctor is null)
            {
                return NotFound("El doctor solicitado no existe o no está disponible.");
            }

            doctor.Desactivar();

            return NoContent();
        }
    }
}
