using ClinicService.Data;
using ClinicService.Data.Context;
using ClinicServiceNamespace;
using Grpc.Core;
using static ClinicServiceNamespace.ClinicService;

namespace ClinicServiceV2.Services.Impl
{
    public class ClinicService : ClinicServiceBase
    {
        private readonly ClinicServiceDbContext _context;

        public ClinicService(ClinicServiceDbContext context)
        {
            _context = context;
        }

        public override Task<CreateClientResponse> CreateClient(CreateClientRequest request, ServerCallContext context)
        {
            CreateClientResponse response;
            try
            {
                Client client = new Client()
                {
                    Document = request.Document,
                    Surname = request.Surname,
                    FirstName = request.FirstName,
                    Patronymic = request.Patronymic,
                };

                _context.Clients.Add(client);
                _context.SaveChanges();

                response = new CreateClientResponse()
                {
                    ClientId = client.Id,
                    ErrCode = 0,
                    ErrMessage = string.Empty,
                };

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {

                response = new CreateClientResponse
                {
                    ErrCode = 1000,
                    ErrMessage = $"Ошибка создания клиента: {ex.Message}",
                };

                return Task.FromResult(response);
            }
        }

        public override Task<GetClientsResponse> GetClients(GetClientsRequest request, ServerCallContext context)
        {
            GetClientsResponse response;
            try
            {
                response = new GetClientsResponse();

                response.Clients.AddRange(_context.Clients.Select(c => new ClientResponse
                {
                    ClientId = c.Id,
                    Document = c.Document,
                    Surname = c.Surname,
                    FirstName = c.FirstName,
                    Patronymic = c.Patronymic,
                }));

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                response = new GetClientsResponse
                {
                    ErrCode = 1005,
                    ErrMessage = $"Ошибка получения списка клиентов: {ex.Message}",
                };

                return Task.FromResult(response);
            }
        }
    }
}
