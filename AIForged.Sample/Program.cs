using AIForged;
using AIForged.API;

Console.WriteLine("Hello, AIForged!");

Console.WriteLine($"OS: {Environment.OSVersion} 64bit {Environment.Is64BitOperatingSystem}");
Console.WriteLine($"Net: {Environment.Version} 64bit {Environment.Is64BitProcess} Interactive: {Environment.UserInteractive}");

//username and password used to register with AIForged 
const string username = "<your username>";
const string password = "<your password>";
//use project id and service id received when service is created and trained
const int projectId = 80;
const int serviceId = 42784;

var cfg = new AIForged.API.Config("https://dev.aiforged.com", username, password, "Demo");
await cfg.Init(allowAutoRedirect: true);
var ctx = new AIForged.API.Context(cfg);
var user = await ctx.LoginAsync();

var project = await ctx.GetProjectAsync(user.Id, projectId);
var service = await ctx.GetServiceAsync(serviceId);

Console.WriteLine($"User {user.DisplayName} Id: {user.Id}");
Console.WriteLine($"Project {project.Name} Id: {project.Id}");
Console.WriteLine($"Service {service.Name} Id: {service.Id}");

var docsresp = await ctx.DocumentClient.GetExtendedAsync(user.Id, 
    project.Id, 
    service.Id, 
    UsageType.Outbox, 
    new List<DocumentStatus> { DocumentStatus.Processed },
    null, null, null, 
    new DateTime(2021, 6, 1), DateTime.UtcNow, 
    null, null, 
    pageNo: 0, pageSize:10, SortField.Date, SortDirection.Descending, 
    null, null, null, null, null, null);

int? docId = docsresp.Result.FirstOrDefault()?.Id;

Console.WriteLine($"Docs {docsresp.Result.Count}, use doc {docId}");

var extarctresp = await ctx.ParametersClient.ExtractAsync(docId);
var results = extarctresp.Result
    .Where(r => r.Category == ParameterDefinitionCategory.Results && r.Grouping != GroupingType.Word)
    .ToList();

foreach (var r in results)
{
    Console.WriteLine($"{r.Id} {r.Name}[{r.Index}] {r.Value}");
}

//var json = Newtonsoft.Json.JsonConvert.SerializeObject(results);
//json.Display();