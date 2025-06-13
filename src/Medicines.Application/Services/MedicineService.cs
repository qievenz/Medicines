using Medicines.Core.Entities;
using Medicines.Core.Exceptions;
using Medicines.Core.Repositories;
using Medicines.Core.Services;
using Serilog;

namespace Medicines.Application.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _medicineRepository;

        public MedicineService(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<IEnumerable<Medicine>> GetAllDatasAsync()
        {
            return await _medicineRepository.GetAll();
        }

        public async Task<Medicine?> GetDataByIdAsync(Guid id)
        {
            var data = await _medicineRepository.GetById(id);

            if (data == null)
            {
                Log.Warning("Registro Data con ID '{Id}' no encontrado.", id);
                throw new ResourceNotFoundException($"Data record with ID '{id}' was not found.");
            }

            return data;
        }

        public async Task<Medicine> CreateDataAsync(Medicine data)
        {
            if (data == null)
            {
                Log.Error("Se intentó crear un objeto Data nulo.");
                throw new InvalidInputException("El objeto Data no puede ser nulo.");
            }

            await _medicineRepository.Add(data);

            return data;
        }

        public async Task<bool> UpdateDataAsync(Guid id, Medicine data)
        {
            if (data == null || id != data.Id)
            {
                Log.Warning("Intentando actualizar Data con ID inválido '{Id}' o ID de objeto '{DataId}' no coincide.", id, data?.Id);
                throw new InvalidInputException("ID inválido o ID de objeto no coincide para la actualización.");
            }

            var existingData = await _medicineRepository.GetById(id);

            if (existingData == null)
            {
                Log.Warning("Registro Data con ID '{Id}' no encontrado para actualización.", id);
                throw new ResourceNotFoundException($"Data record with ID '{id}' not found for update.");
            }

            await _medicineRepository.Update(existingData);

            return true;
        }

        public async Task<bool> DeleteDataAsync(Guid id)
        {
            var existingData = await _medicineRepository.GetById(id);
            if (existingData == null)
            {
                Log.Warning("Registro Data con ID '{Id}' no encontrado para eliminación.", id);
                throw new ResourceNotFoundException($"Data record with ID '{id}' not found for deletion.");
            }

            await _medicineRepository.Delete(id);

            return true;
        }
    }
}
