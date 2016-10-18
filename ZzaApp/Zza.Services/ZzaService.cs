using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zza.Data;
using Zza.Entities;

namespace Zza.Services
{
    public class ZzaService : IZzaService
    {
        private ZzaDbContext _Context = new ZzaDbContext();
        public List<Customer> GetCustomer()
        {
            return _Context.Customers.ToList();
        }

        public List<Product> GetProducts()
        {
            return _Context.Products.ToList();
        }

        public void SubmitOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
