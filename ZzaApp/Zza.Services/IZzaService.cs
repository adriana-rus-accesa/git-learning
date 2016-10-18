using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Zza.Services
{
    [ServiceContract]
    public interface IZzaService
    {
        [OperationContract]
        List<Product> GetProducts();
        [OperationContract]
        List<Customer> GetCustomer();
        [OperationContract]
        void SubmitOrder(Order order);
    }
}
