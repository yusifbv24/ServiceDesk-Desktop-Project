using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDesk.Dto
{
    public record ProductDto
    {
        public int Id { get; set; }
        public int InventoryCode { get; set; }
        public string Model { get; set; }
        public string Vendor { get; set; }
        public string Worker { get; set; }
        public string Description { get; set; }
        public bool IsWorking { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}