var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin();
var postgresdb = postgres.AddDatabase("postgresdb");

var bankSystemAPI = builder.AddProject<Projects.BankSystem_Api>("BankSystem-Api")
                            .WaitFor(postgresdb)
                            .WithReference(postgresdb)
                            .WithUrlForEndpoint("https", a => a.Url = "/scalar");


builder.Build().Run();

//TODO: Revisar Testes
//TODO: Adicionar autenticacao e autorizacao