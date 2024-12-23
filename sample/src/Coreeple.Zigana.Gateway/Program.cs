using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddZigana(builder.Configuration)
    .UseNpgsql();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//app.MapZiganaApi();

app.UseZigana();


DiagnosticListener.AllListeners.Subscribe(new Subscriber());

app.Run();


class Subscriber : IObserver<DiagnosticListener>
{
    public void OnCompleted() { }
    public void OnError(Exception error) { }

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "Coreeple.Zigana1")
        {
            listener.Subscribe(new MyLibraryListener());
        }
    }
}

class MyLibraryListener : IObserver<KeyValuePair<string, object?>>
{
    public void OnCompleted() { }
    public void OnError(Exception error) { }

    public void OnNext(KeyValuePair<string, object?> keyValue)
    {
        switch (keyValue.Key)
        {
            case "ZiganaEndpoint.Start":
                Console.WriteLine($"ZiganaEndpoint.Start - activity id: {Activity.Current?.Id}");
                break;
            case "ZiganaEndpoint.Stop":
                Console.WriteLine($"ZiganaEndpoint.Stop - activity id: {Activity.Current?.Id}");
                break;
            case "HttpRequestActionExecutor.Start":
                Console.WriteLine($"HttpRequestActionExecutor.Start - activity id: {Activity.Current?.Id}");
                break;
            case "HttpRequestActionExecutor.Stop":
                Console.WriteLine($"HttpRequestActionExecutor.Stop - activity id: {Activity.Current?.Id}");
                break;
        }

        //switch (keyValue.Key)
        //{
        //    case "DoThingAsync.Start":
        //        Console.WriteLine($"DoThingAsync.Start - activity id: {Activity.Current?.Id}");
        //        break;
        //    case "DoThingAsync.Stop":
        //        Console.WriteLine("DoThingAsync.Stop");

        //        if (Activity.Current != null)
        //        {
        //            foreach (var tag in Activity.Current.Tags)
        //            {
        //                Console.WriteLine($"{tag.Key} - {tag.Value}");
        //            }
        //        }
        //        break;
        //    case "DiagnosticSourceSample.MySampleLibrary.StartGenerateRandom":
        //        Console.WriteLine("StartGenerateRandom");
        //        break;
        //    case "DiagnosticSourceSample.MySampleLibrary.EndGenerateRandom":
        //        var randomValue = keyValue.Value?.GetType().GetTypeInfo().GetDeclaredProperty("RandomNumber")?.
        //            GetValue(keyValue.Value);

        //        Console.WriteLine($"StopGenerateRandom Generated random value: {randomValue}");
        //        break;
        //    default:
        //        break;
        //}
    }
}