using System.Reactive.Linq;
using System.Reactive.Subjects;
using com.pedidoaberto.pedidos;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using ProcessadorPedidos.Application.Ouvintes;
using ProcessadorPedidos.Domain.Entities;

namespace ProcessadorPedidos.Infrastructure.Ouvintes
{
    public class OuvintePedidos : IOuvintePedidos
    {
        private ISubject<Pedido> _subjectPedido {get; set;}
        private readonly ILogger<OuvintePedidos> _logger;

        public OuvintePedidos(ILogger<OuvintePedidos> logger)
        {
            _subjectPedido = new Subject<Pedido>();
            _logger = logger;
        }

        public async Task Iniciar(CancellationToken ctoken)
        {
            await Consumir(ctoken);
        }

        private async Task Consumir(CancellationToken ctoken)
        {
 
            string bootstrapServers = "broker.local:19092";
            string nomeTopic = "teste";
            
            var files = Directory.EnumerateFileSystemEntries("./Cert");
            foreach (var f in files)
            {
                _logger.LogInformation(f);
            }
            


            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = $"{nomeTopic}-group-0",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SslKeystoreLocation = "./Cert/kafka.dotnet-app.keystore.jks",
                SslCaLocation = "./Cert/fake-ca-1.crt",
                SslKeystorePassword = "awesomekafka",
                SecurityProtocol = SecurityProtocol.Ssl,
                SaslMechanism = SaslMechanism.Plain,
                Debug="consumer,cgrp,topic,fetch"
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                Url = "schema-registry.local:8081"
            };

            try
            {
                using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
                using (var consumer = new ConsumerBuilder<string, NovoPedido>(config)
                                                .SetValueDeserializer(new AvroDeserializer<NovoPedido>(schemaRegistry).AsSyncOverAsync())
                                                .Build())
                {
                    consumer.Subscribe(nomeTopic);

                    while (!ctoken.IsCancellationRequested)
                    {
                        Observable.Start(
                            ()=> consumer.Consume(ctoken)
                        )
                        .Catch<ConsumeResult<string, NovoPedido>, OperationCanceledException>(
                            ex => {
                                consumer.Close();
                                _logger.LogWarning("Cancelada a execução do Consumer...");
                                return null;
                            }
                        )
                        .Do(cr => {
                                _logger.LogInformation( $"Mensagem lida: {cr.Message.Value}");
                            }
                        )
                        .Subscribe( 
                            cr => 
                                _subjectPedido.OnNext(
                                    new Pedido {
                                        Fornecedor = cr.Message.Value.fornecedor,
                                        Nome = cr.Message.Value.nome,
                                        SKU = cr.Message.Value.sku,
                                        Valor = 50.0M,
                                        Vendedor = cr.Message.Value.vendedor
                                    }
                                )
                        );
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exceção: {ex.GetType().FullName} | " +
                             $"Mensagem: {ex.Message}");
            }
        
        }


        public ISubject<Pedido> ObterSubject()
        {
            return _subjectPedido;
        }
    }
}