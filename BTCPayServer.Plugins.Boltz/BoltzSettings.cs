#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using Autoswaprpc;
using Newtonsoft.Json;
using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Services;
using BTCPayServer.Data;
using BTCPayServer.Plugins.Shopify.Models;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NBitcoin;
using NBXplorer;
using Newtonsoft.Json.Linq;

namespace BTCPayServer.Plugins.Boltz;

public enum BoltzMode
{
    Rebalance,
    Standalone
}

public class BoltzSettings
{
    [Display(Name = "GRPC Url")] public Uri GrpcUrl { get; set; }

    [Display(Name = "Macaroon")] public string Macaroon { get; set; }

    [Display(Name = "Version")] public string DaemonVersion { get; set; }

    public BoltzMode? Mode { get; set; }

    public ulong TenantId { get; set; }

    public class Wallet
    {
        public ulong Id { get; set; }
        public string? Name { get; set; }
    }

    public Wallet? StandaloneWallet { get; set; }

    public bool CredentialsPopulated()
    {
        return
            !string.IsNullOrWhiteSpace(GrpcUrl.ToString()) &&
            !string.IsNullOrWhiteSpace(Macaroon);
    }

    public DateTimeOffset? IntegratedAt { get; set; }

    [JsonIgnore] public BoltzClient Client => new(GrpcUrl, Macaroon);
}

public class ProtoConverter<T> : JsonConverter<T> where T : IMessage, new()
{
    public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
    {
        writer.WriteValue(JsonFormatter.Default.Format(value!));
    }

    public override T? ReadJson(JsonReader reader, Type objectType, T? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        return JsonParser.Default.Parse<T>((string)reader.Value!);
    }
}

/*
public class BoltzSetup
{
    public BoltzSettings Settings { get; set; }

    [JsonConverter(typeof(ProtoConverter<LightningConfig>))]
    public LightningConfig LightningConfig { get; set; }

    [JsonConverter(typeof(ProtoConverter<ChainConfig>))]
    public ChainConfig ChainConfig { get; set; }
}
*/