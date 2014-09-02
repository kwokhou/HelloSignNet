HelloSignNet
============

Task based .NET 4.0+ wrapper for the HelloSign API using HttpClient

## Usage

All HelloSign API requests can be made using `HelloSignClient`. This class must be initialized with your [API key](https://www.hellosign.com/home/myAccount/current_tab/integrations#api).

```csharp
var client = new HelloSignClient("YOUR-HELLOSIGN-API-KEY-HERE");
```

You can alternatively initialize `HelloSignClient` with cusom configuration using `HelloSignConfig`. 

This is particullary useful when you want to change the underlying HelloSign uri or http timeout.

```csharp
var exampleConfig = new HelloSignConfig("YOUR-HELLOSIGN-API-KEY-HERE", "http://path/to/api/endpoing", 10000);
var client = new HelloSignClient(exampleConfig);
```

In case you need even more granular control over the underlying HttpClient instant, just initialize `HelloSignClient` with your custom HttpClient, example:

```csharp
var httpClient = FakeClientWithJsonResponse("TestData\\GetAccount-OK.json");
var client = new HelloSignClient(httpClient);
```
** NOTE: Objects in HelloSignNet are intentionally prefixed with `HS` **

### Creating a Signature Request

Signature Request data in HelloSignNet is wrapped in `HSSendSignatureRequestData`

```csharp
var requestData = new HSSendSignatureRequestData
{
    Title = "NDA for Project X",
    Subject = "NDA We Talk about",
    Message = "Bla Bla Bla",
    Signers = new List<HSSigner> {new HSSigner {Name = "John", EmailAddress = "john@example.com"}},
    Files = new List<FileInfo> {new FileInfo("TestData\\pdf-sample.pdf")}
};

...

var t = client.SendSignatureRequest(requestData); // This will return a Task which will contains the response object.
t.Wait(); // Wait response from HelloSign API

// or alternatively continue with other Tasks

client.SendSignatureRequest(requestData).ContinueWith(t => 
{
  // Do something with the response HSSignatureRequestResponse object
});

```

### Get Signature Request Status

```csharp
client.GetSignatureRequest("fa5c8a0b0f492d768749333ad6fcc214c111e967")).ContinueWith(t => 
{
  // Do something with the response HSSignatureRequestResponse object
});
```

__Please refer unit tests codes for more examples of using HelloSignNet.__
