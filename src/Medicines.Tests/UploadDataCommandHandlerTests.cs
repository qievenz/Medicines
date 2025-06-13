using Medicines.Application.Commands.Ingestion;
using Medicines.Application.Handlers.Ingestion;
using Medicines.Core.Entities;
using Medicines.Core.Enums;
using Medicines.Core.Repositories;
using Medicines.Core.Services;
using Moq;
using System.Text;

namespace Medicines.Tests
{
    [TestFixture]
    public class UploadDataCommandHandlerTests
    {
        private Mock<IIngestionProcessRepository> _ingestionProcessRepositoryMock;
        private Mock<IMedicineRepository> _medicineRepositoryMock;
        private Mock<IAuditRepository> _auditRepositoryMock;
        private Mock<IMedicineValidationService> _medicineValidationServiceMock;
        private UploadDataCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _ingestionProcessRepositoryMock = new Mock<IIngestionProcessRepository>();
            _medicineRepositoryMock = new Mock<IMedicineRepository>();
            _auditRepositoryMock = new Mock<IAuditRepository>();
            _medicineValidationServiceMock = new Mock<IMedicineValidationService>();
            _handler = new UploadDataCommandHandler(
                _ingestionProcessRepositoryMock.Object,
                _medicineRepositoryMock.Object,
                _auditRepositoryMock.Object,
                _medicineValidationServiceMock.Object
            );
        }

        [Test]
        public async Task Handle_WithInvalidFileType_ShouldReturnFailedResult()
        {
            // Arrange
            var fileBytes = Encoding.UTF8.GetBytes("dummy content");
            var command = new UploadDataCommand
            {
                FileName = "medicines.txt",
                FileContent = fileBytes,
                ContentType = "text/plain"
            };

            _ingestionProcessRepositoryMock.Setup(x => x.Add(It.IsAny<IngestionProcess>())).Returns(Task.CompletedTask);
            _ingestionProcessRepositoryMock.Setup(x => x.Update(It.IsAny<IngestionProcess>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(IngestionStatus.Failed, result.Status);
            Assert.AreEqual("Data ingestion failed.", result.Message);
            Assert.IsNotNull(result.ErrorDetails);
        }
    }

    // Dummy DTOs for test compilation
    public class MedicineJsonDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Laboratory { get; set; }
        public string ActiveIngredient { get; set; }
        public string Concentration { get; set; }
        public string Presentation { get; set; }
        public string ExpirationDate { get; set; }
    }

    public class JsonIngestionInput
    {
        public List<MedicineJsonDto> Medicines { get; set; }
    }
}
