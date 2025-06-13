namespace Medicines.Application.Queries.Medicines
{
    public class GetPagedMedicinesQuery
    {
        public string Name { get; set; }
        public string Laboratory { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
