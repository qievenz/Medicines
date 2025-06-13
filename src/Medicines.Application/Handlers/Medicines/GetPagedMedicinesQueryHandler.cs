using Medicines.Application.Queries.Medicines;
using Medicines.Core.DTOs;
using Medicines.Core.DTOs.Common;
using Medicines.Core.Repositories;

namespace Medicines.Application.Handlers.Medicines
{
    public class GetPagedMedicinesQueryHandler
    {
        private readonly IMedicineRepository _medicineRepository;

        public GetPagedMedicinesQueryHandler(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<PagedResult<MedicineDto>> Handle(GetPagedMedicinesQuery request)
        {
            var totalCount = await _medicineRepository.CountMedicinesAsync(request.Name, request.Laboratory);
            var medicines = await _medicineRepository.GetPagedMedicinesAsync(request.Name, request.Laboratory, request.PageNumber, request.PageSize);

            var medicineDtos = medicines.Select(m => new MedicineDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                Laboratory = m.Laboratory,
                ActiveIngredient = m.ActiveIngredient,
                Concentration = m.Concentration,
                Presentation = m.Presentation,
                ExpirationDate = m.ExpirationDate
            });

            return new PagedResult<MedicineDto>
            {
                Items = medicineDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
