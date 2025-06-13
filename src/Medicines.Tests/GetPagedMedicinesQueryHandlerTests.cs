using Medicines.Application.Handlers.Medicines;
using Medicines.Application.Queries.Medicines;
using Medicines.Core.Entities;
using Medicines.Core.Repositories;
using Moq;

namespace Medicines.Tests
{
    [TestFixture]
    public class GetPagedMedicinesQueryHandlerTests
    {
        private Mock<IMedicineRepository> _medicineRepositoryMock;
        private GetPagedMedicinesQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _handler = new GetPagedMedicinesQueryHandler(_medicineRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ReturnsPagedResultWithCorrectData()
        {
            // Arrange
            var query = new GetPagedMedicinesQuery
            {
                Name = "Ibuprofeno",
                Laboratory = "LabX",
                PageNumber = 1,
                PageSize = 2
            };

            var medicines = new List<Medicine>
            {
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    Code = "IBU001",
                    Name = "Ibuprofeno",
                    Laboratory = "LabX",
                    ActiveIngredient = "Ibuprofeno",
                    Concentration = "400mg",
                    Presentation = "Tableta",
                    ExpirationDate = DateTime.UtcNow.AddYears(1)
                },
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    Code = "IBU002",
                    Name = "Ibuprofeno Forte",
                    Laboratory = "LabX",
                    ActiveIngredient = "Ibuprofeno",
                    Concentration = "600mg",
                    Presentation = "Tableta",
                    ExpirationDate = DateTime.UtcNow.AddYears(2)
                }
            };

            _medicineRepositoryMock
                .Setup(r => r.CountMedicinesAsync(query.Name, query.Laboratory))
                .ReturnsAsync(2);

            _medicineRepositoryMock
                .Setup(r => r.GetPagedMedicinesAsync(query.Name, query.Laboratory, query.PageNumber, query.PageSize))
                .ReturnsAsync(medicines);

            // Act
            var result = await _handler.Handle(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(2, result.PageSize);
            var items = result.Items.ToList();
            Assert.AreEqual(2, items.Count);
            Assert.AreEqual("IBU001", items[0].Code);
            Assert.AreEqual("IBU002", items[1].Code);
        }
    }
}
