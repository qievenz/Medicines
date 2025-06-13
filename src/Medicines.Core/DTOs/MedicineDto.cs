using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicines.Core.DTOs
{
    public class MedicineDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Laboratory { get; set; }
        public string ActiveIngredient { get; set; }
        public string Concentration { get; set; }
        public string Presentation { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
