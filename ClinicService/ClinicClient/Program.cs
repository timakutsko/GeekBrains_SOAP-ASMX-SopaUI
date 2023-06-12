using ClinicServiceNamespace;
using Grpc.Net.Client;
using System;
using static ClinicServiceNamespace.ClinicService;

namespace ClinicClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using (GrpcChannel chanel = GrpcChannel.ForAddress("http://localhost:5001"))
            { 
                ClinicServiceClient clinicServiceClient = new ClinicServiceClient(chanel);

                CreateClientResponse createClientResponse = clinicServiceClient.CreateClient(new CreateClientRequest
                {
                    Document = "Passport ~ MP7777777",
                    FirstName = "Tsimafei",
                    Patronymic = "Syargeevich",
                    Surname = "Kutsko",
                });
                if (createClientResponse.ErrCode == 0)
                {
                    Console.WriteLine("[Новый клиент]");
                    Console.WriteLine($"Клиент успешно создан! Id: {createClientResponse.ClientId}");
                }
                else
                    Console.WriteLine($"Ошибка! Код: {createClientResponse.ErrCode}\n Описание: {createClientResponse.ErrMessage}");

                GetClientsResponse getClientsResponse = clinicServiceClient.GetClients(new GetClientsRequest());
                if (getClientsResponse.ErrCode == 0)
                {
                    Console.WriteLine("[Список клиентов клиники]");
                    foreach (var client in getClientsResponse.Clients)
                        Console.WriteLine($"Данные по клиенту:" +
                            $"\nId: {client.ClientId}" +
                            $"\nDocument: {client.Document}" +
                            $"\nFirstName: {client.FirstName}" +
                            $"\nPatronymic: {client.Patronymic}" +
                            $"\nSurname: {client.Surname}");
                }
                else
                    Console.WriteLine($"Ошибка! Код: {getClientsResponse.ErrCode}\n Описание: {getClientsResponse.ErrMessage}");

                Console.ReadKey();
            }
        }
    }
}
