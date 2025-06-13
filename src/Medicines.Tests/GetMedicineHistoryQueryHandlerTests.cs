using Medicines.Application.Handlers.Audit;
using Medicines.Application.Queries.Audit;
using Medicines.Core.Entities;
using Medicines.Core.Repositories;
using Moq;

namespace Medicines.Tests
{
    [TestFixture]
    public class GetMedicineHistoryQueryHandlerTests
    {
        private Mock<IAuditRepository> _auditRepositoryMock;
        private GetMedicineHistoryQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _auditRepositoryMock = new Mock<IAuditRepository>();
            _handler = new GetMedicineHistoryQueryHandler(_auditRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ReturnsMappedAuditEntryDtos()
        {
            // Arrange
            var medicineId = Guid.NewGuid();
            var auditEntries = new List<AuditEntry>
            {
                new AuditEntry
                {
                    Id = Guid.NewGuid(),
                    EntityId = medicineId,
                    FieldName = "Name",
                    OldValue = "A",
                    NewValue = "B",
                    ChangedAt = DateTime.UtcNow.AddDays(-1),
                    ChangedBy = "user1",
                    SourceFile = "file1.csv",
                    SourceFileTimestamp = DateTime.UtcNow.AddDays(-2)
                },
                new AuditEntry
                {
                    Id = Guid.NewGuid(),
                    EntityId = medicineId,
                    FieldName = "Description",
                    OldValue = "desc1",
                    NewValue = "desc2",
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = "user2",
                    SourceFile = "file2.csv",
                    SourceFileTimestamp = DateTime.UtcNow.AddDays(-3)
                }
            };

            _auditRepositoryMock
                .Setup(r => r.GetHistoryByEntityId(medicineId))
                .ReturnsAsync(auditEntries);

            var query = new GetMedicineHistoryQuery { MedicineId = medicineId };

            // Act
            var result = (await _handler.Handle(query)).ToList();

            // Assert
            Assert.AreEqual(auditEntries.Count, result.Count);
            for (int i = 0; i < auditEntries.Count; i++)
            {
                Assert.AreEqual(auditEntries[i].Id, result[i].Id);
                Assert.AreEqual(auditEntries[i].EntityId, result[i].EntityId);
                Assert.AreEqual(auditEntries[i].FieldName, result[i].FieldName);
                Assert.AreEqual(auditEntries[i].OldValue, result[i].OldValue);
                Assert.AreEqual(auditEntries[i].NewValue, result[i].NewValue);
                Assert.AreEqual(auditEntries[i].ChangedAt, result[i].ChangedAt);
                Assert.AreEqual(auditEntries[i].ChangedBy, result[i].ChangedBy);
                Assert.AreEqual(auditEntries[i].SourceFile, result[i].SourceFile);
                Assert.AreEqual(auditEntries[i].SourceFileTimestamp, result[i].SourceFileTimestamp);
            }
        }

        [Test]
        public async Task Handle_ReturnsEmptyList_WhenNoAuditEntries()
        {
            // Arrange
            var medicineId = Guid.NewGuid();
            _auditRepositoryMock
                .Setup(r => r.GetHistoryByEntityId(medicineId))
                .ReturnsAsync(new List<AuditEntry>());

            var query = new GetMedicineHistoryQuery { MedicineId = medicineId };

            // Act
            var result = await _handler.Handle(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}
