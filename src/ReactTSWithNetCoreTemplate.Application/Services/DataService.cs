using ReactTSWithNetCoreTemplate.Core.Entities;
using ReactTSWithNetCoreTemplate.Core.Exceptions;
using ReactTSWithNetCoreTemplate.Core.Repositories;
using ReactTSWithNetCoreTemplate.Core.Services;
using Serilog;

namespace ReactTSWithNetCoreTemplate.Application.Services
{
    public class DataService : IDataService
    {
        private readonly IDataRepository _dataRepository;

        public DataService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<IEnumerable<Data>> GetAllDatasAsync()
        {
            return await _dataRepository.GetAll();
        }

        public async Task<Data?> GetDataByIdAsync(int id)
        {
            if (id <= 0)
            {
                Log.Warning("Intentando obtener un registro Data con ID inválido: {Id}", id);
                throw new InvalidInputException($"El ID de producto '{id}' debe ser un número positivo.", nameof(id));
            }

            var data = await _dataRepository.GetById(id);

            if (data == null)
            {
                Log.Warning("Registro Data con ID '{Id}' no encontrado.", id);
                throw new ResourceNotFoundException($"Data record with ID '{id}' was not found.");
            }

            return data;
        }

        public async Task<Data> CreateDataAsync(Data data)
        {
            if (data == null)
            {
                Log.Error("Se intentó crear un objeto Data nulo.");
                throw new InvalidInputException("El objeto Data no puede ser nulo.");
            }

            await _dataRepository.Add(data);

            return data;
        }

        public async Task<bool> UpdateDataAsync(int id, Data data)
        {
            if (id <= 0 || data == null || id != data.Id)
            {
                Log.Warning("Intentando actualizar Data con ID inválido '{Id}' o ID de objeto '{DataId}' no coincide.", id, data?.Id);
                throw new InvalidInputException("ID inválido o ID de objeto no coincide para la actualización.");
            }

            var existingData = await _dataRepository.GetById(id);

            if (existingData == null)
            {
                Log.Warning("Registro Data con ID '{Id}' no encontrado para actualización.", id);
                throw new ResourceNotFoundException($"Data record with ID '{id}' not found for update.");
            }

            await _dataRepository.Update(existingData);

            return true;
        }

        public async Task<bool> DeleteDataAsync(int id)
        {
            if (id <= 0)
            {
                Log.Warning("Intentando eliminar Data con ID inválido: {Id}", id);
                throw new InvalidInputException($"El ID de eliminación '{id}' debe ser un número positivo.", nameof(id));
            }

            var existingData = await _dataRepository.GetById(id);
            if (existingData == null)
            {
                Log.Warning("Registro Data con ID '{Id}' no encontrado para eliminación.", id);
                throw new ResourceNotFoundException($"Data record with ID '{id}' not found for deletion.");
            }

            await _dataRepository.Delete(id);

            return true;
        }
    }
}
